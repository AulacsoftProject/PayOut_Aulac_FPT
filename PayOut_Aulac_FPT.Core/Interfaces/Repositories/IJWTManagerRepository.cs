using PayOut_Aulac_FPT.Core.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace PayOut_Aulac_FPT.Core.Interfaces.Repositories
{
    public interface IJWTManagerRepository
    {
        Tokens? GenerateToken(string? id);
        Tokens? GenerateRefreshToken(string? id);
        ClaimsPrincipal GetPrincipalFromExpiredToken(string? token);
    }
}
