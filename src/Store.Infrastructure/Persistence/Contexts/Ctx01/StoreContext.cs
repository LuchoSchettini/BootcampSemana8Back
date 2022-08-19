using Microsoft.EntityFrameworkCore;
using Store.ApplicationCore.Entities.Ctx01;
using System.Reflection;

namespace Store.Infrastructure.Persistence.Contexts.Ctx01
{
    //Context01_Store
    //Ctx01_Store
    public class Ctx01_Store : DbContext
    {
        public Ctx01_Store(DbContextOptions<Ctx01_Store> options) : base(options)
        {
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            //Para injecte todos IEntityTypeConfiguration que tenga el esamblado
            //estos los Api Fluente que tengo (hay cosas que no se pueden hacer por DataAnotations:
            //***************************************************>>>
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            //***************************************************<<<
        }


        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }


    }
}

//Add-Migration InitialCreate -Context StoreContext


//OJO solo si queremos que la crpeta de migraciones las ponga en un carpeta llamada Data:

//Add-Migration InitialCreate -Context Ctx01_Store -OutputDir Migrations\Ctx01

//Add - Migration AñadirCamposPasswordHashToUsuarioBD - Context Ctx01_Store - OutputDir Migrations\Ctx01


//Update-Database