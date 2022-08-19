using Microsoft.Extensions.DependencyInjection;
using Store.ApplicationCore.Sevices.EncriptPassword;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text;

namespace Store.ApplicationCore
{
    [ExcludeFromCodeCoverage]
    public static class DependencyInjectionCore
    {
        public static IServiceCollection AddInjectionCore(this IServiceCollection services)
        {
            //Injecta todas las clases configuracion que hay en el ensamblado:
            //***************************************************>>>
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            //***************************************************<<<

            //Repositorios
            //***************************************************>>>
            //Podria utilizar alguna clase mia, pero por ahora usare la de Identity
            services.AddScoped<IEncriptPasswordService, EncriptPasswordService>();
            //***************************************************<<<


            return services;
        }
    }
}
