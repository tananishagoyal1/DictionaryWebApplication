using DictionaryApp.DataAccess;
using DictionaryApp.Handler;
using DictionaryApp.Helper;
using DictionaryApp.Models;
using DictionaryApp.Services;
using DictionaryApp.Validator;
using DictionaryApp.ViewModels;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Refit;
using System.Security.Claims;

namespace DictionaryProjectMVC
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllersWithViews();
            builder.Services.AddFluentValidationAutoValidation();
            builder.Services.AddFluentValidationClientsideAdapters();
        

            builder.Services.AddScoped<ITokenService,TokenService>();
            builder.Services.AddTransient<HttpHeaderHandler>();
            builder.Services.AddHttpContextAccessor();  

            builder.Services.AddRefitClient<IDictionaryApiClient>()
           .ConfigureHttpClient(c => c.BaseAddress = new Uri(builder.Configuration[Constants.DictionaryApiConfigurations__ApiBaseUrl]))
           .AddHttpMessageHandler<HttpHeaderHandler>();

            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    
                    options.LoginPath = "/user/Login";

                });

            builder.Services.AddScoped<IValidator<RegisterUserViewModel>, RegistrationValidator>();
            builder.Services.AddScoped<IValidator<InputWordViewModel>, InputWordValidator>();
            builder.Services.AddScoped<IValidator<LoginUser>, LoginValidator>();

            var app = builder.Build();
            
            if (!app.Environment.IsDevelopment())
            {
                app.UseStatusCodePagesWithReExecute("/Error/{0}");
                app.UseExceptionHandler("/Error");
                
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}