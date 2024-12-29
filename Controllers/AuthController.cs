
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using WhatsApp_Clone.Models;
using Microsoft.AspNetCore.Identity;
using WhatsApp_Clone.Services;
namespace WhatsApp_Clone.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {



        private readonly UserManager<IdentityUser> _userManager;
        private readonly IAuthService _authService;
        private readonly ApplicationDbContext _Context;
        private readonly IWebHostEnvironment _hostingEnvironment;



        public AuthController(IWebHostEnvironment hostingEnvironment, IAuthService authService, ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _authService = authService;
            _hostingEnvironment = hostingEnvironment;
            _Context = context;
            _userManager = userManager;
        }




        [HttpPost("register")]

        public async Task<IActionResult> RegisterStudentAsync([FromForm]RegisterUser model)
        {

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.RegisterUserAsync(model);


            return Ok(result);
        }

        [HttpPost("load")]

        public async Task<IActionResult> LoadUserDataIfHeRegisterBefore(string email)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _Context.Users.Where(u => u.UserName == email).Select(u => new
            {
                u.Name,
                u.About,
                u.ImageURL
            }).FirstOrDefaultAsync();

            return Ok(result);
        }





        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(string token, string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                return Ok("Email confirmed successfully.");
            }
            else
            {
                return BadRequest("Error confirming email.");
            }
        }


        [HttpPost("verify")]
        public async Task<IActionResult> SendEmailAsync(string email)
        {
            Random random = new Random();
            string code = "";
            for (int i = 0; i < 6; i++)
            {
                int digit = random.Next(0, 10); // Generates a random number between 0 and 9
                code += digit.ToString();

            }

            if (_authService.SendEmail("test", email, "verify your email", $"<h1>Welcome to our Whatsapp colne app </h1><div>Please verify your email</div><div>Use next verification code to verify your email</div><a href=''>{code}</a>").IsCompletedSuccessfully)

                return Ok(new stringClass { data = code });
            else
                return Ok("there an error send again");

        }
        [HttpGet("verify")]
        public async Task<IActionResult> VerifyEmailAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                user.EmailConfirmed = true;
                _Context.Update(user);
                await _Context.SaveChangesAsync();
                return Ok(new stringClass { data = "Your email verified successuflly" });
            }
            else
            {
                return Ok(new stringClass { data = "There is a problem try again" });
            }
        }

    }

    public class UserRegistrationModel
    {
        public string Name { get; set; }
        public string Picture { get; set; }
        public string Email { get; set; }
    }


}

