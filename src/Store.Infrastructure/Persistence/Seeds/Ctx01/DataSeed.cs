using Microsoft.Extensions.Logging;
using Store.ApplicationCore.Entities.Ctx01;
//using Store.Infrastructure.Persistence.Contexts;
using Store.Infrastructure.Persistence.Contexts.Ctx01;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Infrastructure.Persistence.Seeds.Ctx01;

public class DataSeed
{
    public static async Task DataSeedAsync(Ctx01_Store context, ILoggerFactory loggerFactory)
    {
        try
        {

            if (!context.Categories.Any())
            {
                DateTime miDateTime;

                List<Category> Categorias = new List<Category>();
                for (int i = 0; i < 10; i++)
                {
                    miDateTime = DateTime.Now;
                    Category category = new Category();
                    category.Name = $"Categoria {i + 1}";
                    category.Description = $"Descripcion categoria {i + 1}";
                    category.CreatedAt = miDateTime;
                    category.UpdatedAt = miDateTime;
                    Categorias.Add(category);
                }
                await context.Categories.AddRangeAsync(Categorias);
                await context.SaveChangesAsync();

            }

            if (!context.Products.Any())
            {
                DateTime miDateTime;

                List<Product> Productos = new List<Product>();
                for (int i = 0; i < 10; i++)
                {
                    miDateTime = DateTime.Now;
                    Product product = new Product();
                    product.Name = $"Producto {i + 1}";
                    product.Description = $"Descripcion producto {i + 1}";
                    var categoria = context.Categories.Where(x => x.Name == $"Categoria {i + 1}").First();
                    product.CategoryId = categoria.Id;
                    product.Stock = 5 + i;
                    product.Price = 25 + i;
                    product.CreatedAt = miDateTime;
                    product.UpdatedAt = miDateTime;
                    Productos.Add(product);
                }
                await context.Products.AddRangeAsync(Productos);
                await context.SaveChangesAsync();
            }

        }
        catch (Exception ex)
        {
            var logger = loggerFactory.CreateLogger<DataSeed>();
            logger.LogError(ex.Message);
        }


    }//fin metodo

}//fin clase
