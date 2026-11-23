using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using SMProfile.WebAPI.Context;
using SMProfile.WebAPI.Interfaces;
using SMProfile.WebAPI.Models;
using SMProfile.WebAPI.Repositories;
using SMProfile.WebAPI.Services;
using SMProfile.WebAPI.Utils;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<SMProfileDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("Configure a ConnectionString"),
        sqlOptions => sqlOptions.EnableRetryOnFailure()));

builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IGrupoRepository, GrupoRepository>();
builder.Services.AddScoped<IComebackRepository, ComebackRepository>();
builder.Services.AddScoped<IAlbumFisicoRepository, AlbumFisicoRepository>();

builder.Services.Configure<CloudinarySettings>(
    builder.Configuration.GetSection(CloudinarySettings.SectionName));
builder.Services.AddScoped<IImageService, CloudinaryService>();

builder.Services.AddControllers();

var jwt = builder.Configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>()
    ?? throw new InvalidOperationException("Configure a seção Jwt.");

if (string.IsNullOrWhiteSpace(jwt.Issuer) || string.IsNullOrWhiteSpace(jwt.Audience)
    || Encoding.UTF8.GetByteCount(jwt.Key) < 32 || jwt.ExpirationMinutes is < 1 or > 1440)
    throw new InvalidOperationException("Configure Jwt:Issuer, Jwt:Audience, Jwt:ExpirationMinutes e Jwt:Key (mínimo de 32 bytes)");

builder.Services.AddSingleton(jwt);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.MapInboundClaims = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwt.Issuer,
            ValidateAudience = true,
            ValidAudience = jwt.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key)),
            RequireSignedTokens = true,
            RequireExpirationTime = true,
            ClockSkew = TimeSpan.Zero,
            NameClaimType = JwtRegisteredClaimNames.Sub,
            RoleClaimType = ClaimTypes.Role
        };

        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = async context =>
            {
                var idTexto = context.Principal?.FindFirstValue(JwtRegisteredClaimNames.Sub);
                if (!int.TryParse(idTexto, out var id))
                {
                    context.Fail("Token sem ID de usuário");
                    return;
                }

                var repositorio = context.HttpContext.RequestServices.GetRequiredService<IUsuarioRepository>();
                var usuario = await repositorio.BuscarPorId(id);
                if (usuario is null || !usuario.Ativo)
                    context.Fail("Usuário inativo ou inexistente.");
            }
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "SMProfile.WebAPI",
        Version = "v1",
        Description = "API de gerenciamento de grupos, comebacks e álbuns físicos da SM Entertainment."
    });

    options.InferSecuritySchemes();
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontEnd", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<SMProfileDbContext>();
    if (!await db.Usuarios.IgnoreQueryFilters().AnyAsync())
    {
        db.Usuarios.Add(new Usuario
        {
            Nome = "Ana Carol",
            Email = "anacarol@email.com",
            Senha = BCrypt.Net.BCrypt.HashPassword("admin123"),
            Ativo = true
        });
        await db.SaveChangesAsync();
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "SMProfile.WebAPI v1");
        options.RoutePrefix = string.Empty;
    });
    app.MapGet("/swagger", () => Results.Redirect("/"));
}

app.UseHttpsRedirection();
app.UseCors("FrontEnd");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
