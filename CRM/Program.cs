using CRM.Core;
using CRM.Core.Filters;
using CRM.Core.Helpers;
using CRM.Core.Models;
using CRM.Core.Settings;
using CRM.Extentions;
using CRM.Infrastructure;
using CRM.Infrastructure.Data;
using Hangfire;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

namespace CRM
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddDbContext<ApplicationDbContext>(
            options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            }
           );
            builder.Services.AddControllers(options =>
            {
                options.Filters.Add<CustomValidationResultFilter>();
            });

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                // Swagger mainly is used as the documentation for the api
                options.SwaggerDoc("v1", new OpenApiInfo //Open API Info Object, it provides the metadata about the Open API.
                {
                    Version = "v1",
                    Title = "CRM_APIs",
                    Description = "CRM APIs",
                    //TermsOfService = new Uri("https://www.google.com"),
                    License = new OpenApiLicense
                    {
                        Name = "License",
                        Url = new Uri("https://github.com/Mahmoud-Mshrf/CRM")
                    }

                });
                // Here we add authorization for all endpoints in one time
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Scheme = "Bearer",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter Your JWT Key",

                });
                // Here we add aauthorization on each endpoint 
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                    new OpenApiSecurityScheme
                    {
                        Name= "Bearer",
                        Reference = new OpenApiReference
                        {
                            Id= "Bearer",
                            Type = ReferenceType.SecurityScheme
                        },
                        In = ParameterLocation.Header,
                        BearerFormat="JWT",
                        Scheme="Bearer"
                    },
                    new List<string>()
                    }
                });
            });
            builder.Services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            //builder.Services.AddCors();
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigins", builder =>
                {
                    builder.WithOrigins("http://localhost:3000", "http://localhost:3001", "http://localhost:3002")
                           .AllowAnyHeader()
                           .AllowAnyMethod()
                           .AllowCredentials()
                           .SetIsOriginAllowed((host) => true);// Allow any other origin, but handle in the controller
                });

                options.AddPolicy("AllowAnyOrigin", builder =>
                {
                    builder.AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader();
                });
            });
            // Add Hangfire to the container
            builder.Services.AddHangfire(x => x.UseSqlServerStorage(builder.Configuration.GetConnectionString("HangfireConnection")));
            builder.Services.AddHangfireServer();


            builder.Services.Configure<JWT>(builder.Configuration.GetSection("JWT"));// Add JWT configuration to the container
            builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));// Add MailSettings configuration to the container
            builder.Services.AddApplicationServices();

            //builder.Services.AddTransient(typeof(IBaseRepository<>), typeof(BaseRepository<>));
            builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();// Add IUnitOfWork to the container
            builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
            {
                options.TokenLifespan = TimeSpan.FromMinutes(10);// Sets the expiry for the generated token to 10 minutes  
            });

            // Add Identity DbContext to the container
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();
            var DefaultConnection = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(DefaultConnection, b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));
            // Add JWT Authentication to the container
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }
            ).AddJwtBearer(opt =>
            {
                opt.RequireHttpsMetadata = false;
                opt.SaveToken = false;
                opt.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidAudience = builder.Configuration["JWT:Audience"],
                    ValidIssuer = builder.Configuration["JWT:Issuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"])),
                    ClockSkew = TimeSpan.Zero
                };
            });

            builder.Services.Configure<IdentityOptions>(options =>
            {
                // Default Password settings.
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 8;
                options.Password.RequiredUniqueChars = 1;
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.

            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseHttpsRedirection();
            //app.UseCors(builder =>
            //{
            //    builder.WithOrigins("http://localhost:3000","http://localhost:3001","http://localhost:3002")
            //           .AllowAnyHeader()
            //           .AllowAnyMethod()
            //           .AllowCredentials()
            //           .SetIsOriginAllowed(host=>true) // Allow any other origin, but handle in the controller
            //           .Build();
            //});
            app.UseCors("AllowSpecificOrigins");
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseHangfireDashboard("/HangfireDashboard");

            app.MapControllers();

            app.Run();
        }
    }
}