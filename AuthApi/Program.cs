
using AuthApi.Models;
using AuthApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using System.Text;

namespace AuthApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<AuthuserContext>(options =>
                options.UseMySQL(builder.Configuration.GetConnectionString("MySql"))
            );

            builder.Services.AddScoped<PasswordHasher>();
            builder.Services.AddScoped<GenerateToken>();
            //builder.Services.AddScoped<IConfiguration>();

            var secret = builder.Configuration.GetSection("Token:secret").Value; ;//Legalább 32 karakter hossz legyen
            var issuer = builder.Configuration.GetSection("Token:issuer").Value;
            var audience = builder.Configuration.GetSection("Token:client").Value;

            var key = Encoding.ASCII.GetBytes(secret);

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(jwt =>
            {
                jwt.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = issuer,
                    ValidAudience = audience,
                    ValidateAudience = true
                };
            });


            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.MapScalarApiReference();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();//ez az első
            app.UseAuthorization();//ez a második a szekvenciában


            app.MapControllers();

            app.Run();
        }
    }
}
