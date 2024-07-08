using BallastLane.BookVault.Api.Endpoints.Auth;
using BallastLane.BookVault.Api.Endpoints.Books;
using BallastLane.BookVault.Api.Jwt;
using BallastLane.BookVault.Api.Middlewares;
using BallastLane.BookVault.Application.Common;
using BallastLane.BookVault.Application.Features.Books.AddBook;
using BallastLane.BookVault.Application.Features.Books.DeleteBook;
using BallastLane.BookVault.Application.Features.Books.GetAllBooks;
using BallastLane.BookVault.Application.Features.Books.GetBook;
using BallastLane.BookVault.Application.Features.Books.UpdateBook;
using BallastLane.BookVault.Application.Features.Users.SignIn;
using BallastLane.BookVault.Application.Features.Users.SignUp;
using BallastLane.BookVault.Domain.Repositories;
using BallastLane.BookVault.Infrastructure.Database;
using BallastLane.BookVault.Infrastructure.Database.Repositories;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Npgsql;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace BallastLane.BookVault.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var appConfig = builder.Configuration.GetSection("AppConfig").Get<AppConfig>();

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = !builder.Environment.IsDevelopment();
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(appConfig!.JwtSecret)),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

            builder.Services.AddSingleton(x => new JwtTokenGenerator(appConfig!.JwtSecret, new JwtSecurityTokenHandler()));
            builder.Services.AddAuthorization();

            builder.Services.AddValidatorsFromAssemblyContaining<AddBookValidator>();

            builder.Services.AddScoped<ConnectionStringAccessor>(x => new ConnectionStringAccessor(appConfig!.ConnectionString));

            builder.Services.AddScoped<IBookRepository, BookRepository>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();

            builder.Services.AddScoped<IRequestHandler<SignInCommand, SignInResponse>, SignInHandler>();
            builder.Services.AddScoped<IRequestHandler<SignUpCommand>, SignUpHandler>();

            builder.Services.AddScoped<IRequestHandler<GetBookQuery, GetBookResponse>, GetBookHandler>();
            builder.Services.AddScoped<IRequestHandler<GetAllBooksQuery, GetAllBooksResponse>, GetAllBooksHandler>();
            builder.Services.AddScoped<IRequestHandler<AddBookCommand, AddBookResponse>, AddBookHandler>();
            builder.Services.AddScoped<IRequestHandler<UpdateBookCommand>, UpdateBookHandler>();
            builder.Services.AddScoped<IRequestHandler<DeleteBookCommand>, DeleteBookHandler>();

            builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddSwaggerGen(opt =>
            {
                opt.SwaggerDoc("v1", new OpenApiInfo { Title = "BallastLane BookVault", Version = "v1" });
                opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "bearer"
                });

                opt.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type=ReferenceType.SecurityScheme,
                                Id="Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseExceptionHandler("/error");

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            AuthApiEndpoints.ConfigureEndpoints(app);
            BooksApiEndpoints.ConfigureEndpoints(app);

            RunDbScripts(appConfig!.ConnectionString);

            app.Run();
        }

        private sealed class AppConfig
        {
            public string JwtSecret { get; set; } = default!;
            public string ConnectionString { get; set; } = default!;
        }

        private static void RunDbScripts(string connectionString)
        {
            var scriptsFolder = Path.Combine(AppContext.BaseDirectory, "Database", "Scripts");
            var scripts = new List<string>();
            var scriptFiles = Directory.GetFiles(scriptsFolder, "*.sql");

            foreach (var file in scriptFiles)
            {
                var script =  File.ReadAllText(file);
                scripts.Add(script);
            }


            using var connection = new NpgsqlConnection(connectionString);
            connection.Open();

            foreach (var script in scripts)
            {
                using var command = new NpgsqlCommand(script, connection);
                command.ExecuteNonQuery();
            }
        }
    }
}
