
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TooLiRent.Core.Interfaces;
using TooLiRent.Data;
using TooLiRent.Infrastructure.Data;
using TooLiRent.Infrastructure.Repositories;
using TooLiRent.Services.DTOs;
using TooLiRent.Services.Interfaces;
using TooLiRent.Services.Mapping;
using TooLiRent.Services.Services;
using TooLiRent.Services.Validation;
using Microsoft.OpenApi.Models;

namespace TooLiRent
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddAutoMapper(cfg => { }, typeof(MappingProfile));

            builder.Services.AddControllers();

            //Tools
            builder.Services.AddScoped<IToolService, ToolService>();
            builder.Services.AddScoped<IToolRepository, ToolRepository>();

            //Customer
            builder.Services.AddScoped<ICustomerService, CustomerService>();
            builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();

            //Rental
            builder.Services.AddScoped<IRentalService, RentalService>();
            builder.Services.AddScoped<IRentalRepository, RentalRepository>();

            //Admin Summary
            builder.Services.AddScoped<IAdminSummaryService, AdminSummaryService>();

            //Unit of Work
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            //Auth
            builder.Services.AddScoped<IAuthService, AuthService>();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Put your JWT token down here: "
                });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            builder.Services.AddDbContext<TooLiRentDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
                x => x.MigrationsAssembly("TooLiRent.Infrastructure")));

            builder.Services.AddDbContext<AuthDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
                x => x.MigrationsAssembly("TooLiRent.Infrastructure")));


            // Identity-tjänster (UserManager, RoleManager, etc.)
            builder.Services
                .AddIdentityCore<IdentityUser>(options =>
                {
                    options.User.RequireUniqueEmail = true;
                    options.Password.RequiredLength = 8;

                })
                .AddRoles<IdentityRole>() // Oklart, behövs denna ens? Kanske för rollhantering i framtiden?
                .AddEntityFrameworkStores<AuthDbContext>()
                .AddDefaultTokenProviders();

            //JWT
            var jwt = builder.Configuration.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!));

            builder.Services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateIssuerSigningKey = true,
                        ValidateLifetime = true,
                        ValidIssuer = jwt["Issuer"],
                        ValidAudience = jwt["Audience"],
                        IssuerSigningKey = key,
                        ClockSkew = TimeSpan.FromMinutes(1)
                    };
                });
            builder.Services.AddAuthorization();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });



            // FluentValidation – registrera manuellt
            builder.Services.AddScoped<IValidator<ToolCreateDto>, ToolCreateDtoValidator>();
            builder.Services.AddScoped<IValidator<ToolUpdateDto>, ToolUpdateDtoValidator>();
            builder.Services.AddScoped<IValidator<CustomerCreateDto>, CustomerCreateDtoValidator>();
            builder.Services.AddScoped<IValidator<CustomerUpdateDto>, CustomerUpdateDtoValidator>();
            builder.Services.AddScoped<IValidator<RentalCreateDto>, RentalCreateDtoValidator>();
            builder.Services.AddScoped<IValidator<RentalUpdateDto>, RentalUpdateDtoValidator>();

            var app = builder.Build();

            // Seed Identity data

            using (var scope = app.Services.CreateScope())
            {
                var sp = scope.ServiceProvider;

                Task.Run(async () =>
                {
                    var roleMgr = sp.GetRequiredService<RoleManager<IdentityRole>>();
                    var userMgr = sp.GetRequiredService<UserManager<IdentityUser>>();

                   
                    foreach (var role in new[] { "Admin", "Member" })
                        if (!await roleMgr.RoleExistsAsync(role))
                            await roleMgr.CreateAsync(new IdentityRole(role));

                    // Admin
                    const string adminEmail = "admin@toolirent.local";
                    var admin = await userMgr.FindByEmailAsync(adminEmail);
                    if (admin is null)
                    {
                        var u = new IdentityUser
                        {
                            UserName = adminEmail,
                            Email = adminEmail,
                            EmailConfirmed = true
                        };
                        await userMgr.CreateAsync(u, "Admin123!");
                        await userMgr.AddToRoleAsync(u, "Admin");
                    }

                    // Member
                    const string memberEmail = "member@toolirent.local";
                    var member = await userMgr.FindByEmailAsync(memberEmail);
                    if (member is null)
                    {
                        var u = new IdentityUser
                        {
                            UserName = memberEmail,
                            Email = memberEmail,
                            EmailConfirmed = true
                        };
                        await userMgr.CreateAsync(u, "Member123!");
                        await userMgr.AddToRoleAsync(u, "Member");
                    }
                }).GetAwaiter().GetResult();
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseCors("AllowAll");


            app.UseAuthentication();
            app.UseAuthorization();



            app.Use(async (context, next) =>
            {
                try
                {
                    await next();
                }
                catch (FluentValidation.ValidationException ex)
                {
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    context.Response.ContentType = "application/json";

                    var errors = ex.Errors.Select(e => new
                    {
                        property = e.PropertyName,
                        error = e.ErrorMessage
                    });

                    await context.Response.WriteAsJsonAsync(errors);
                }
            });


            app.MapControllers();



            app.Run();


            // vad göra härnäst: 
        }
    }
}
