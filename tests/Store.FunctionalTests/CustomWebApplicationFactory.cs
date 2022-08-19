using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Store.Infrastructure.Persistence.Contexts.Ctx01;
using Store.SharedDatabaseSetup;
using System;
using System.Linq;

namespace Store.FunctionalTests
{
    //public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<Program> 
    {

        //si yo solo hago peticiones Get, es decir que no realizo niguna modificacion en la bbdd
        //me basta con este metodo.
        //Por eso este metodo elimina el dbcontext y lo vuelve a crear con los registros que solo tiene el Seed
        //y asi en este metodo solo tenemos los registros que se necesite que hayan sin registros adicionales que no deberian estar.
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                //ESTO POR CADA CONTEXT
                //asi eliminamos si hay algun context ya creado por otro proyecto
                //Remove the app's StoreContext registration.
                //*******************************************************
                // Busca el context especifico
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType ==
                        typeof(DbContextOptions<Ctx01_Store>));
                //si lo encuentra lo elimina
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }
                //Añado el context a una InMemory database:
                // Add StoreContext using an in-memory database for testing.
                services.AddDbContext<Ctx01_Store>(options =>
                {
                    options.UseInMemoryDatabase("InMemoryDbForFunctionalTesting");
                });
                //*******************************************************


                //Procdemos a obtener el Proveedor de Servicios:
                // Get service provider.
                var serviceProvider = services.BuildServiceProvider();

                using (var scope = serviceProvider.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;

                    var logger = scopedServices.GetRequiredService<ILogger<CustomWebApplicationFactory<TStartup>>>();


                    //Esto por cada Context:
                    //(creamos la BBDD si no existe y hacemos el Seeds de los datos
                    //*******************************************************
                    var storeDbContext = scopedServices.GetRequiredService<Ctx01_Store>();
                    storeDbContext.Database.EnsureCreated();
                    try
                    {
                        DatabaseSetup.SeedData(storeDbContext);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, $"An error occurred seeding the Store database with Funtional test messages. Error: {ex.Message}");
                    }
                    //*******************************************************
                }
            });
        }



        //Metodo -> si hago peticiones Post, Put... y estos van a modificar y a crear registros.
        //como ves la diferencia de este metodo con el anterior ->
        //(es que en el anterior se elinina el context anterior y se vuelve a crear (en este no))
        public void CustomConfigureServices(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Get service provider.
                var serviceProvider = services.BuildServiceProvider();

                using (var scope = serviceProvider.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;

                    var logger = scopedServices.GetRequiredService<ILogger<CustomWebApplicationFactory<TStartup>>>();


                    //Esto por cada Context:
                    //(creamos la BBDD si no existe y hacemos el Seeds de los datos
                    //*******************************************************
                    var storeDbContext = scopedServices.GetRequiredService<Ctx01_Store>();
                    try
                    {
                        DatabaseSetup.SeedData(storeDbContext);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, $"An error occurred seeding the Store database with Funtional test messages. Error: {ex.Message}");
                    }
                    //*******************************************************
                }
            });
        }
    }
}