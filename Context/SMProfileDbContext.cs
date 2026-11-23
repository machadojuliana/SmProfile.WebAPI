using Microsoft.EntityFrameworkCore;
using SMProfile.WebAPI.Models;

namespace SMProfile.WebAPI.Context;

public class SMProfileDbContext : DbContext
{
    public SMProfileDbContext(DbContextOptions<SMProfileDbContext> options) : base(options) { }

    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Grupo> Grupos => Set<Grupo>();
    public DbSet<Comeback> Comebacks => Set<Comeback>();
    public DbSet<AlbumFisico> AlbunsFisicos => Set<AlbumFisico>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.ToTable("Usuario", tb => tb.HasTrigger("TR_UsuarioSoftDelete"));
            entity.HasIndex(u => u.Email).IsUnique();
            entity.HasQueryFilter(u => u.Ativo);
        });

        modelBuilder.Entity<Grupo>(entity =>
        {
            entity.ToTable("Grupo", tb => tb.HasTrigger("TR_GrupoSoftDelete"));
            entity.HasIndex(g => g.NomeGrupo).IsUnique();
            entity.HasQueryFilter(g => g.AtivoGrupo);
        });

        modelBuilder.Entity<AlbumFisico>(entity =>
        {
            entity.HasOne(af => af.Comeback)
                  .WithMany(c => c.AlbunsFisicos)
                  .HasForeignKey(af => af.IdComeback)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(af => af.Grupo)
                  .WithMany(g => g.AlbunsFisicos)
                  .HasForeignKey(af => af.IdGrupo)
                  .OnDelete(DeleteBehavior.Restrict);
        });
    }
}