using Dapper;
using System.Data;
using Microsoft.Data.SqlClient;
using CapstoneAPI.Model;

namespace CapstoneAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddAuthorization();

            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            var defaultConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddScoped<IDbConnection>(x => new SqlConnection(defaultConnectionString));

            var app = builder.Build();
            

            

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

           

           


            app.MapPost("/api/usercreation", async (IDbConnection db, UserLoginModal user) =>
            {
                var result = await db.ExecuteAsync("INSERT INTO InventoryDb.dbo.Users (username, user_password) VALUES (@UserName, @UserPassword)", user);
                //return Results.Created($"/api/photos/", user);
                return Results.Created();
            });

            app.Run();
        }
    }
}
