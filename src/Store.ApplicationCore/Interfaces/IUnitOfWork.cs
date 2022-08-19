using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.ApplicationCore.Interfaces
{
    public interface IUnitOfWork
    {
        ICategoryRepository Categories { get; }
        IProductRepository Products { get; }

        ////IUserRepository Users { get; }


        //   Prefiero que se haga el SaveChange del context en cada metodo del Repository
        //   asi mantenemos que cada cosa se haga en su respectivo lugar, por ejemplo si
        //   hay que actualizar mas de un cosa en el la bbdd, lo hacemos en el metodo del
        //   repository y asi toda la logica se queda en el metodo
        //Task<int> SaveAsync();
    }
}
