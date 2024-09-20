using UserAuth.Data;
using UserAuth.Models;
using System.Threading.Tasks;

namespace UserAuth.Services;

public class RevokedTokenService
{
    private readonly ApplicationDbContext _context;

    public RevokedTokenService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task RevokeTokenAsync(string token)
    {
        // For Revoke the token in the app.
        var revokedToken = new RevokedToken
        {
            Token = token,
            RevokedAt = DateTime.UtcNow
        };

        _context.RevokedTokens.Add(revokedToken);
        await _context.SaveChangesAsync();
    }

    public bool IsTokenRevoked(string token)
    {
        return _context.RevokedTokens.Any(rt => rt.Token == token);
    }
}
