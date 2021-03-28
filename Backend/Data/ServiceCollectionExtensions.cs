using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using PortableManager.Web.Server.Data.Repositories.Implementation;
using PortableManager.Web.Server.Data.Repositories.Interface;
using PortableManager.Web.Server.Servicies;
using System;
using System.Collections.Generic;
using System.Text;

namespace PortableManager.Web.Server.Data
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddEfRepositories(this IServiceCollection services, string connectionString, IConfiguration Configuration)
        {
            services.AddDbContext<AppDbContext>(
                option =>
                {
                    option.UseLoggerFactory(LoggerFactory.Create(builder => builder.AddConsole()))
                          .UseSqlServer(connectionString);
                },
                ServiceLifetime.Transient);

            services.AddDefaultIdentity<IdentityUser>()
                    .AddRoles<IdentityRole>()
                    .AddEntityFrameworkStores<AppDbContext>();
                    

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options =>
                    {
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidateAudience = true,
                            ValidateLifetime = true,
                            ValidateIssuerSigningKey = true,
                            ValidIssuer = Configuration["JwtIssuer"],
                            ValidAudience = Configuration["JwtAudience"],
                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JwtSecurityKey"]))
                        };
                    });

            services.AddScoped<Dictionary<Type, AppDbContext>>();
            services.AddSingleton<DbContextFactory>();
            
            services.AddSingleton<ITaskRepository, TaskRepository>();
            services.AddSingleton<ITaskTypeRepository, TaskTypeRepository>();

            services.AddSingleton<TaskTypeService>();
            services.AddSingleton<TaskService>();

            return services;
        }  

    }
}
