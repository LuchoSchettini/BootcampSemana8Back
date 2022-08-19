using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Store.ApplicationCore.Entities.Ctx01;
using Store.Infrastructure.Persistence.Contexts.Ctx01;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Store.Infrastructure.Persistence.Seeds.Ctx01;

public class DataSeedDesdeJson
{
    public static async Task DataSeedAsync(Ctx01_Store context, ILoggerFactory loggerFactory)
    {
        try
        {
            var ruta = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            await context.Database.OpenConnectionAsync();

            if (!context.Categories.Any())
            {
                context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Categories ON");

                var categoryData = File.ReadAllText(ruta + @"/Persistence/Seeds/Ctx01/JsonData/categorias.json");
                var categorias = JsonSerializer.Deserialize<List<Category>>(categoryData);

                await context.Categories.AddRangeAsync(categorias);
                await context.SaveChangesAsync();

                context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Categories OFF");
            }


            if (!context.Products.Any())
            {
                context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Products ON");

                var productosData = File.ReadAllText(ruta + @"/Persistence/Seeds/Ctx01/JsonData/productos.json");
                var productos = JsonSerializer.Deserialize<List<Product>>(productosData);

                await context.Products.AddRangeAsync(productos);
                await context.SaveChangesAsync();

                context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Products OFF");
            }

            await context.Database.CloseConnectionAsync();
        }
        catch (Exception ex)
        {
            var logger = loggerFactory.CreateLogger<DataSeedDesdeJson>();
            logger.LogError(ex.Message);
        }


    }//fin metodo

}//fin clase
