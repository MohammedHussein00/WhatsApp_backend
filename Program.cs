using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using WhatsApp_Clone.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using WhatsApp_Clone.Services;
using WhatsApp_Clone;

using System.Collections.Generic;
using Music_Aditor.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IStudentService, StudentService>();





builder.Services.AddCors();
//
builder.Services.Configure<JWT>(builder.Configuration.GetSection("JWT"));
//

builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();


builder.Services.AddDbContext<ApplicationDbContext>(o => o.UseSqlServer(
    builder.Configuration.GetConnectionString("DefaultConnetion")
    ));


builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
})
          .AddCookie()
          .AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
          {
              options.ClientId = "";
              options.ClientSecret = "";
              options.ClaimActions.MapJsonKey("urn:google:picture", "picture", "url");
              options.ClaimActions.MapJsonKey("urn:google:locale", "locale", "string");

              options.SaveTokens = true;

              options.Events.OnCreatingTicket = ctx =>
              {
                  List<AuthenticationToken> tokens = ctx.Properties.GetTokens().ToList();

                  tokens.Add(new AuthenticationToken()
                  {
                      Name = "TicketCreated",
                      Value = DateTime.UtcNow.ToString()
                  });

                  ctx.Properties.StoreTokens(tokens);

                  return Task.CompletedTask;
              };

          });

builder.Services.AddControllersWithViews();
builder.Services.Configure<IISServerOptions>(options =>
{
    options.MaxRequestBodySize = 52428800;
});

builder.Services.AddAuthentication(o =>
{
    o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    o.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(o =>
{
    o.SaveToken = true;
    o.RequireHttpsMetadata = false;
    o.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidAudience = builder.Configuration["JWT:ValidAudience"],
        ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"]))
    };

});
// Add services to the container.


builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
    {
        policy
            .WithOrigins("http://localhost:4200")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});



builder.Services.AddSignalR();
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



var app = builder.Build();


app.UseSwagger();
app.UseSwaggerUI();

app.UseWebSockets();


app.UseStaticFiles();
app.UseStaticFiles(new StaticFileOptions()
{

});



app.UseCors("CorsPolicy");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapHub<ChatHub>("/chat");


app.MapControllers();
app.Run();
