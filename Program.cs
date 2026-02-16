using CapstoneAPI.Model;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Swashbuckle;
using System.Data;
using System.Text.RegularExpressions;
using System.Web.Http;


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
            //builder.Services.AddOpenApi();
            builder.Services.AddSwaggerGen();
            builder.Services.AddEndpointsApiExplorer();

            var defaultConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddScoped<IDbConnection>(x => new SqlConnection(defaultConnectionString));

            var app = builder.Build();




            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                //app.MapOpenApi();
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();






            app.MapPost("/api/usercreation", async (IDbConnection db, UserLoginModal user) =>
            {

                var result = await db.ExecuteAsync("INSERT INTO Inventorydatabase.dbo.Users (username, user_password, user_isactive) VALUES (@UserName, @UserPassword,@IsActive)", user);

                return Results.Created();
            });


            //uses fromquery which is from url that is being sent from calling function.
            app.MapGet("/api/userlogin", async ([FromServices] IDbConnection db, [FromQuery] string username, [FromQuery] string password) =>
            {
                var result = await db.QueryFirstOrDefaultAsync(
                    "SELECT * FROM Inventorydatabase.dbo.Users WHERE username = @username AND user_password = @password",
                    new { username, password }
                );



                return result is not null
                    ? Results.Ok()
                    : Results.NotFound();
            });

            //Uses GET to retrive specified items from the database and returns them as a list of items.
            // Get all component items for a specific Bill of Materials (parent item)
            app.MapGet("/api/getitems", async ([FromServices] IDbConnection db, [FromQuery] int searchedpartnumber) =>
            {
                var result = await db.QueryAsync<ItemModel>(
                    @"SELECT 
                      i.item_id        AS ItemId,
                      i.item_description AS ItemDescription,
                      i.item_quantity  AS ItemQuantity,
                      i.item_partnumber AS ItemPartNumber
                        FROM Inventorydatabase.dbo.Item i
                        INNER JOIN Inventorydatabase.dbo.BillOfMaterials bom 
                      ON i.item_id = bom.Item_Id
                         WHERE bom.FinishedGood_Id = @ParentItemId",
                    new { ParentItemId = searchedpartnumber }
                );

                return Results.Ok(result);
            });


            app.MapGet("/api/getmaxbuildqty", async (
                [FromServices] IDbConnection db,
                 [FromQuery] int parentItemId) =>
                {
                var result = await db.QueryFirstOrDefaultAsync<int?>(
                    @"SELECT
                    MIN(FLOOR(p.item_quantity / b.QtyRequiredPerFG)) AS MaxBuildableQty
                     FROM BillOfMaterials b
                    JOIN Item p ON p.item_id = b.Item_Id
                     WHERE b.FinishedGood_Id = @ParentItemId
                 GROUP BY b.FinishedGood_Id;",
                    new { ParentItemId = parentItemId });

                return Results.Ok(result ?? 0);
            });


            app.Run();
        }
    }
}
