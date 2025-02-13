using ElMagzer.Core.Models.Identity;
using ElMagzer.Core.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;


namespace ElMagzer.Service
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;

        public AuthService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<string> CreateTokenAsync(AppUser user, UserManager<AppUser> userManager)
        {
            //throw new NotImplementedException();
            // Private Claims 
            var authClaims = new List<Claim>()
            {
                new Claim(ClaimTypes.GivenName,user.UserName ?? "Unknown"),
                new Claim(ClaimTypes.Email,user.Email ?? "Unknown")
            };

            var userRoles = await userManager.GetRolesAsync(user);
            foreach (var role in userRoles)
                authClaims.Add(new Claim(ClaimTypes.Role, role));

            // var authKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:SecretKey"]));
            var secretKey = _configuration["JWT:SecretKey"] ?? throw new ArgumentNullException("JWT:SecretKey is missing");

            var authKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var durationStr = _configuration["JWT:DurationInDays"] ?? throw new ArgumentNullException("JWT:DurationInDays is missing");
            var duration = double.Parse(durationStr);
            //Token
            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.UtcNow.AddDays(duration),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authKey, SecurityAlgorithms.HmacSha256Signature)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
