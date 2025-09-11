
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TooLiRent.Core.Interfaces;
using TooLiRent.Data;
using TooLiRent.Infrastructure.Repositories;
using TooLiRent.Services.DTOs;
using TooLiRent.Services.Interfaces;
using TooLiRent.Services.Mapping;
using TooLiRent.Services.Services;
using TooLiRent.Services.Validation;

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
            builder.Services.AddScoped<IToolService, ToolService>();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<IToolRepository, ToolRepository>();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddDbContext<TooLiRentDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
                x => x.MigrationsAssembly("TooLiRent.Infrastructure")));

            // FluentValidation – registrera manuellt
            builder.Services.AddScoped<IValidator<ToolCreateDto>, ToolCreateDtoValidator>();
            builder.Services.AddScoped<IValidator<ToolUpdateDto>, ToolUpdateDtoValidator>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();


            // vad göra härnäst: Lägga till ToolController för API:et!! 
        }
    }
}
