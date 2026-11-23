using Microsoft.EntityFrameworkCore;
using SMProfile.WebAPI.Context;
using SMProfile.WebAPI.Interfaces;
using SMProfile.WebAPI.Models;

namespace SMProfile.WebAPI.Repositories;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly SMProfileDbContext _context;
    public UsuarioRepository(SMProfileDbContext context) => _context = context;

    public async Task<Usuario?> BuscarPorEmail(string email) =>
        await _context.Usuarios.FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());

    public async Task<Usuario?> BuscarPorId(int id) =>
        await _context.Usuarios.FindAsync(id);
}