using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Store.ApplicationCore.Entities.Ctx01;
using Store.Infrastructure.Persistence.Contexts.Ctx01;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Infrastructure.Persistence.Seeds.Ctx01;

public class DataSeedConIDs
{
    public static async Task DataSeedAsync(Ctx01_Store context, ILoggerFactory loggerFactory)
    {
        try
        {
            await context.Database.OpenConnectionAsync();

            if (!context.Categories.Any())
            {
                context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Categories ON");
                DateTime miDateTime;

                List<Category> Categorias = new List<Category>();
                for (int i = 0; i < 10; i++)
                {
                    miDateTime = DateTime.Now;
                    Category category = new Category();
                    category.Id = i + 1;
                    category.Name = $"Categoria {i + 1}";
                    category.Description = $"Descripcion categoria {i + 1}";
                    category.CreatedAt = miDateTime;
                    category.UpdatedAt = miDateTime;
                    Categorias.Add(category);
                }
                await context.Categories.AddRangeAsync(Categorias);
                await context.SaveChangesAsync();

                context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Categories OFF");
            }

            if (!context.Products.Any())
            {
                context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Products ON");
                DateTime miDateTime;

                List<Product> Productos = new List<Product>();
                for (int i = 0; i < 10; i++)
                {
                    miDateTime = DateTime.Now;
                    Product product = new Product();
                    product.Id = i + 1;
                    product.Name = $"Producto {i + 1}";
                    product.Description = $"Descripcion producto {i + 1}";
                    product.CategoryId = i + 1; ;
                    product.Stock = 5 + i;
                    product.Price = 25 + i;
                    product.CreatedAt = miDateTime;
                    product.UpdatedAt = miDateTime;
                    Productos.Add(product);
                }
                await context.Products.AddRangeAsync(Productos);
                await context.SaveChangesAsync();

                context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Products OFF");
            }

            await context.Database.CloseConnectionAsync();
        }
        catch (Exception ex)
        {
            var logger = loggerFactory.CreateLogger<DataSeedConIDs>();
            logger.LogError(ex.Message);
        }


    }//fin metodo

}//fin clase
