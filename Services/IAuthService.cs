using WhatsApp_Clone.Models;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Identity;
using WhatsApp_Clone.Tables;

namespace WhatsApp_Clone.Services
{
    public interface IAuthService
    {
        Task<AuthModel> RegisterUserAsync(RegisterUser model);




        User CreateUser();
        Task<JwtSecurityToken> CreateJwtToken(IdentityUser user);
        Task<bool> SendEmail(string userName, string userEmail, string subject, string textContent);
    }

}
