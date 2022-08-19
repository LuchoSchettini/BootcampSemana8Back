using Bogus;
using Store.ApplicationCore.Entities.Ctx01;
using Store.ApplicationCore.Helpers.Utils;
using Store.Infrastructure.Persistence.Contexts.Ctx01;
using Microsoft.EntityFrameworkCore;

namespace Store.SharedDatabaseSetup
{
    public static class DatabaseSetup
    {
        public static void SeedData(Ctx01_Store context)
        {
            //primero borro todos los datos:
            //*********************************
            context.Products.RemoveRange(context.Products);
            context.Categories.RemoveRange(context.Categories);
            context.Usuarios.RemoveRange(context.Usuarios);
            context.Roles.RemoveRange(context.Roles);
            //*********************************

            var CategoryIds = 1;
            var fakeCategories = new Faker<Category>()
                .RuleFor(o => o.Name, f => $"Category {CategoryIds}")
                .RuleFor(o => o.Description, f => $"Description Category {CategoryIds}")
                .RuleFor(o => o.Id, f => CategoryIds++)
                .RuleFor(o => o.CreatedAt, f => DateUtil.GetCurrentDate())
                .RuleFor(o => o.UpdatedAt, f => DateUtil.GetCurrentDate());

            var categories = fakeCategories.Generate(10);
            context.AddRange(categories);
            context.SaveChanges();



            var productIds = 1;
            var fakeProducts = new Faker<Product>()
                .RuleFor(o => o.Name, f => $"Product {productIds}")
                .RuleFor(o => o.Description, f => $"Description Product {productIds}")
                .RuleFor(o => o.Id, f => productIds++)
                .RuleFor(o => o.Stock, f => f.Random.Number(0, 50))
                .RuleFor(o => o.Price, f => f.Random.Double(0.01, 1000))
                .RuleFor(o => o.CreatedAt, f => DateUtil.GetCurrentDate())
                .RuleFor(o => o.UpdatedAt, f => DateUtil.GetCurrentDate())
                .RuleFor(o => o.CategoryId, f => f.Random.Number(1, 10));

            var products = fakeProducts.Generate(10);
            context.AddRange(products);
            context.SaveChanges();




            //var rolAdministrador = new Rol() { Nombre = "Administrador" };
            //var rolGerente = new Rol() { Nombre = "Gerente" };
            //var rolEmpleado = new Rol() { Nombre = "Empleado" };
            ////var roles = new List<Rol> { rolAdministrador, rolGerente, rolEmpleado };
            ////context.AddRange(roles);
            ////context.SaveChanges();

            ////lo voy hacer asi (por lo que sea que ponga el Id 1, 2, 3 a los registros adecuados)
            //context.Add(rolAdministrador);
            //context.SaveChanges();
            //context.Add(rolGerente);
            //context.SaveChanges();
            //context.Add(rolEmpleado);
            //context.SaveChanges();


            //tanto SqlLite como Microsoft.EntityFrameworkCore.InMemory se le puede asignar el Id directamente anque sean PrimaryKey y Identity
            //Asi que se lo asigno directamente:
            var roles = new List<Rol>()
                        {
                            new Rol{Id=1, Nombre="Administrador"},
                            new Rol{Id=2, Nombre="Gerente"},
                            new Rol{Id=3, Nombre="Empleado"},
                        };
            context.Roles.AddRange(roles);
            context.SaveChanges();


            var usuarios = new List<Usuario>() //La password descodificada en todos los casos es "lucho"
                        {
                            new Usuario{Id=1, Username="adm@adm.com", Email="adm@adm.com", Nombres="admNom",
                                ApellidoPaterno= "admApePadre", ApellidoMaterno= "admApeMama",
                                PasswordHash="NDYCztO1GYxTGRH/Rcsx6aFWfbHUVkEEFvgR76YsUAu2UyL2lpsVz9TJp/RDtiotra9X4LVZTb0yLwnOoO2lxw==",
                                PasswordSalt="klrKhNeKrdY+XNIi+cvYDTzFfA8oUwFT00YgU8MnpHEknvPfY5FqU9w2Wd1AzFHppi51VqYOYyCqV+oObc1E84xy9ETapwDtAQS/mgP/HyahjX0wm5s9CfQQ4Y1UDTPpFtEfNUbCx9GJC8ujbznKp74aYSa8qpiKcQIXQpULtfo="},
                            new Usuario{Id=2, Username="ger@ger.com", Email="ger@ger.com", Nombres="gerNom",
                                ApellidoPaterno= "gerApePadre", ApellidoMaterno= "gerApeMama",
                                PasswordHash="NDYCztO1GYxTGRH/Rcsx6aFWfbHUVkEEFvgR76YsUAu2UyL2lpsVz9TJp/RDtiotra9X4LVZTb0yLwnOoO2lxw==",
                                PasswordSalt="klrKhNeKrdY+XNIi+cvYDTzFfA8oUwFT00YgU8MnpHEknvPfY5FqU9w2Wd1AzFHppi51VqYOYyCqV+oObc1E84xy9ETapwDtAQS/mgP/HyahjX0wm5s9CfQQ4Y1UDTPpFtEfNUbCx9GJC8ujbznKp74aYSa8qpiKcQIXQpULtfo="},
                            new Usuario{Id=3, Username="emp@emp.com", Email="emp@emp.com", Nombres="empNom",
                                ApellidoPaterno= "empApePadre", ApellidoMaterno= "empApeMama",
                                PasswordHash="NDYCztO1GYxTGRH/Rcsx6aFWfbHUVkEEFvgR76YsUAu2UyL2lpsVz9TJp/RDtiotra9X4LVZTb0yLwnOoO2lxw==",
                                PasswordSalt="klrKhNeKrdY+XNIi+cvYDTzFfA8oUwFT00YgU8MnpHEknvPfY5FqU9w2Wd1AzFHppi51VqYOYyCqV+oObc1E84xy9ETapwDtAQS/mgP/HyahjX0wm5s9CfQQ4Y1UDTPpFtEfNUbCx9GJC8ujbznKp74aYSa8qpiKcQIXQpULtfo="}
                        };
            context.Usuarios.AddRange(usuarios);
            context.SaveChangesAsync();


            var usuariosRoles = new List<UsuariosRoles>()
                        {
                            new UsuariosRoles{ UsuarioId =1, RolId = 1 },
                            new UsuariosRoles{ UsuarioId =2, RolId = 2 },
                            new UsuariosRoles{ UsuarioId =3, RolId = 3 }
                        };
            context.UsuariosRoles.AddRange(usuariosRoles);
            context.SaveChanges();

        }
    }
}