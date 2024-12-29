using WhatsApp_Clone.Models;
using WhatsApp_Clone.Tables;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Owin.Security;
using Microsoft.AspNetCore.Hosting;
using sib_api_v3_sdk.Api;
using sib_api_v3_sdk.Model;
using Music_Aditor.Models;


namespace WhatsApp_Clone.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _Context;
        private readonly JWT _Jwt;
        private readonly IWebHostEnvironment _hostingEnvironment;



        public AuthService(IWebHostEnvironment hostingEnvironment, UserManager<IdentityUser> userManager
            , IOptions<JWT> jwt
            , RoleManager<IdentityRole> roleManager
            , ApplicationDbContext Context)
        {
            _Context = Context;
            _userManager = userManager;
            _Jwt = jwt.Value;
            _hostingEnvironment = hostingEnvironment;

            _roleManager = roleManager;

        }


        public User CreateUser()
        {
            try
            {
                return Activator.CreateInstance<User>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(User)}'. " +
                    $"Ensure that '{nameof(User)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }


        public async Task<AuthModel> RegisterUserAsync(RegisterUser model)
        {
            // Check if the email is already registered
            var existingUser = await _Context.Users.FirstOrDefaultAsync(u=>u.UserName==model.Email);
            var user = CreateUser();

            if (existingUser is null)
            {
                // Create a new student user
                user.UserName = model.Email; // Assigning email as the username
                user.Email = model.Email;
                user.EmailConfirmed = true;
                if (model.Name == "" || model.Name is null)
                    user.Name = "~";
                else
                    user.Name = model.Name;
                if (model.About == "" || model.About is null)
                    user.About = "";
                else
                    user.About = model.About;
            }
            if (existingUser is not null)
            {
                existingUser.Name = model.Name;
                existingUser.About = model.About;
                existingUser.EmailConfirmed = true;

                string directoryPath = Path.Combine(_hostingEnvironment.ContentRootPath, $"ProfileImages/{model.Email}");

                if (model.Image is not null)
                {
                    if (!File.Exists($"{directoryPath}\\{model.Image.FileName}"))
                    {
                        var files = Directory.GetFiles(directoryPath);

                        // Delete each file
                        foreach (var file in files)
                        {
                            try
                            {
                                File.Delete(file);
                            }
                            catch (Exception ex)
                            {
                                // Handle any exceptions (e.g., log them)
                                Console.WriteLine($"Error deleting file {file}: {ex.Message}");
                            }
                        }
                        existingUser.ImageURL = $"ProfileImages/{model.Email}/{model.Image.FileName}";
                        SaveFile($"{directoryPath}\\{model.Image.FileName}", model.Image);

                    }



                }
                else
                {
                    if (model.ImageChanged==true)
                    {
                        var files = Directory.GetFiles(directoryPath);

                        // Delete each file
                        foreach (var file in files)
                        {
                            try
                            {
                                File.Delete(file);
                            }
                            catch (Exception ex)
                            {
                                // Handle any exceptions (e.g., log them)
                                Console.WriteLine($"Error deleting file {file}: {ex.Message}");
                            }
                        }
                        existingUser.ImageURL = null;
                    }
                }
                _Context.Users.Update(existingUser);
                await _Context.SaveChangesAsync();


            }
            else
            {

                    if (string.IsNullOrEmpty(_hostingEnvironment.ContentRootPath))
                {
                    throw new InvalidOperationException("ContentRootPath is not configured.");
                }

                // Combine the content root path with your desired directory structure
                string profileImageDirectory = Path.Combine(_hostingEnvironment.ContentRootPath, $"ProfileImages");
                string folderPath = Path.Combine(_hostingEnvironment.ContentRootPath, "ProfileImages");
                if (!Directory.Exists($"{folderPath}\\{model.Email}"))
                    // Check if the directory exists; if not, create it
                {
                    // If it doesn't exist, create the directory
                    Directory.CreateDirectory($"ProfileImages/{model.Email}");
                    if (model.Image is not null)
                    {
                        string directoryPath = Path.Combine(_hostingEnvironment.ContentRootPath, $"ProfileImages/{model.Email}");


                        SaveFile($"{directoryPath}\\{model.Image.FileName}{Path.GetExtension(model.Image.FileName)}", model.Image);
                        user.ImageURL = $"ProfileImages/{model.Email}/{model.Image.FileName}";
                    }

                }

                // Create the user with the specified password
                var result = await _userManager.CreateAsync(user);
                if (!result.Succeeded)
                {
                    var errors = string.Join(",", result.Errors.Select(e => e.Description));
                    return new AuthModel { Message = errors };
                }

                // Assign the "Student" role to the newly created user
                await _userManager.AddToRoleAsync(user, "User");
                return new AuthModel
                {
                    Email = user.UserName,
                    ExpiredOn = (await CreateJwtToken(user)).ValidTo,
                    IsAuthenticated = true,
                    Roles = new List<string> { "User" },
                    Token = new JwtSecurityTokenHandler().WriteToken(await CreateJwtToken(user)),
                    Name = user.Name,
                    Id = user.Id,
                    EmailConfirmed = user.EmailConfirmed,
                    ImgUrl = user.ImageURL,
                    About = user.About


                };
            }
            // Generate JWT token for the user
            return new AuthModel
            {
                Email = existingUser.UserName,
                ExpiredOn = (await CreateJwtToken(existingUser)).ValidTo,
                IsAuthenticated = true,
                Roles = new List<string> { "User" },
                Token = new JwtSecurityTokenHandler().WriteToken(await CreateJwtToken(existingUser)),
                Name = existingUser.Name,
                Id = existingUser.Id,
                EmailConfirmed = existingUser.EmailConfirmed,
                ImgUrl=existingUser.ImageURL,
                About=existingUser.About
            };
            // Return the authentication model
        }




        

 




        public async Task<JwtSecurityToken> CreateJwtToken(IdentityUser user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);
            var roleClaims = new List<Claim>();

            foreach (var role in roles)
                roleClaims.Add(new Claim("role", role));

            var claims = new[]
            {
                  new Claim(JwtRegisteredClaimNames.Sub,user.UserName),
                  new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                  new Claim(JwtRegisteredClaimNames.Email, user.UserName),
                  new Claim("uid",user.Id),

            }.Union(userClaims).Union(roleClaims);

            var symetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_Jwt.Key));

            var signingCredentials = new SigningCredentials(symetricSecurityKey, SecurityAlgorithms.HmacSha256);


            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _Jwt.ValidIssuer,
                audience: _Jwt.ValidAudience,
                claims: claims,
                expires: DateTime.Now.AddDays(_Jwt.DurationInDays),
                signingCredentials: signingCredentials
                );

            return jwtSecurityToken;
        }

        public static async void SaveFile(string path, IFormFile file)
        {
            // Check if the file and path are valid
            if (file == null || file.Length == 0 || string.IsNullOrEmpty(path))
            {
                throw new ArgumentException("Invalid file or path.");
            }

            // Create directory if it doesn't exist

            // Open a stream to write the file content
            using (var stream = new FileStream(path, FileMode.Create))
            {
                // Copy the file content to the stream asynchronously
                await file.CopyToAsync(stream);
            }
        }


        public async Task<bool> SendEmail(string userName, string userEmail, string subject, string textContent)
        {
            sib_api_v3_sdk.Client.Configuration.Default.ApiKey["api-key"] = "";

            // Create an instance of the TransactionalEmailsApi
            var apiInstance = new TransactionalEmailsApi();

            // Define sender details
            string senderName = "Whatsapp Colne";
            string senderEmail = "mh2156@fayoum.edu.eg";
            SendSmtpEmailSender sender = new SendSmtpEmailSender(senderName, senderEmail);

            // Define recipient details
            string toEmail = userEmail;
            string toName = userName;
            SendSmtpEmailTo recipient = new SendSmtpEmailTo(toEmail, toName);
            List<SendSmtpEmailTo> recipients = new List<SendSmtpEmailTo>();
            recipients.Add(recipient);

            try
            {
                var sendSmtpEmail = new SendSmtpEmail(sender, recipients, null, null, textContent, "Verification Email", subject); // Pass an empty list for Bcc

                // Send the email
                CreateSmtpEmail result = apiInstance.SendTransacEmail(sendSmtpEmail);
                return true;


            }
            catch (Exception e)
            {
                return false;
            }
        }

    }
}
