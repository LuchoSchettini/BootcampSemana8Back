using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Store.ApplicationCore.Entities.Ctx01;
using Store.ApplicationCore.Interfaces;
using Store.Infrastructure.Persistence.Contexts.Ctx01;
using Store.Infrastructure.Persistence.Repositories;
//using Store.Infrastructure.Persistence.Seeds.Ctx01;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Store.Infrastructure
{
    public static class DependencyInjectionInfrastructure
    {
        public static async Task<IServiceCollection> AddInjectionInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {

            //conexion a BBDD
            //***************************************************>>>
            //antes lo tenia (ahoro lo cambie)
            //var defaultConnectionString = configuration.GetConnectionString("DefaultConnection");
            //services.AddDbContext<StoreContext>(options =>
            //   options.UseSqlServer(defaultConnectionString));


            services.AddDbContext<Ctx01_Store>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
            x => x.MigrationsHistoryTable("__MigracionesStoreContext", "dbo")));

            ////SI HAY MAS CONTEXTO DESDE AQUI LOS PUEDO CARGAR:
            //services.AddDbContext<StoreContext2>(options =>
            //options.UseSqlServer(configuration.GetConnectionString("DefaultConnection2"),
            //x => x.MigrationsHistoryTable("__MigracionesStoreContext2", "dbo")));
            //***************************************************<<<


            //Repositorios
            //***************************************************>>>
            ////services.AddScoped<IProductRepository, ProductRepository>();
            ////services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();


            //***************************************************<<<



            ////    YO NO LO USO (AHORA UTILIZO UNA MIA EN namespace Store.ApplicationCore)
            ////    -> Ya esta injectada en services en el proyecto Core.
            //// Repositorios
            ////***************************************************>>>
            //// Podria utilizar alguna clase mia, pero por ahora usare la de Identity
            //services.AddScoped<IPasswordHasher<Usuario> , PasswordHasher<Usuario>>();
            ////***************************************************<<<






            //OJO OJO OJO OJO OJO OJO OJO OJO
            //ESTO NO PUEDE CORRER AQUI, YA QUE CREA LA TABLA CUANDO SE ESTA CREANDO LA MIGRACION Y DA ERROR 
            //AL INSERTAR LOS DATOS DE LOS SEEDS (YA QUE LA TABLA AUN NO ESTA CREADA)
            //ADEMAS NO PUEDE CREAR LA BBDD CUANDO SE CREA LA MIGRACION (ESTO SE TIENE QUE HACER CUANDO
            //SE LEVANTE LA APLICACION) ESTO SE HACE EN LA CLASE PROGRAM, YA ESTA PUESTO ALLI (COMO LOS PODRAS VER)
            ////Migracion Automatica
            ////y puedo tambien ejecutar el seed inicial aqui como hago.
            ////***************************************************>>>
            //var serviceProvider = services.BuildServiceProvider();
            //var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();

            ////puedo hacer esto por cada cotexto de mas que tenga:
            ////***************************>>>
            //try
            //{
            //    var dbContext = serviceProvider.GetRequiredService<StoreContext>();
            //    await dbContext.Database.MigrateAsync();

            //    //añado datos para para hacer pruebas:
            //    await DataSeedDesdeJson.DataSeedAsync(dbContext, loggerFactory);

            //    //añado roles predeterminados:
            //    await DataSeedRoles.DataSeedRolesAsync(dbContext, loggerFactory);
            //}
            //catch (Exception ex)
            //{
            //    var logger = loggerFactory.CreateLogger<StoreContext>();
            //    logger.LogError(ex, "Ocurrió un error durante la migración en la clase DependencyInjection");
            //}
            ////***************************<<<
            ////***************************************************<<<


            return services;
        }
    }
}