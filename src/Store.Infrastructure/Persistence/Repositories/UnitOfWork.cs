using AutoMapper;
using Store.ApplicationCore.Interfaces;
using Store.Infrastructure.Persistence.Contexts.Ctx01;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Infrastructure.Persistence.Repositories
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {

        private readonly Ctx01_Store _context;
        private readonly IMapper mapper;

        private ICategoryRepository _categories;
        private IProductRepository _products;



        public UnitOfWork(Ctx01_Store context, IMapper mapper)
        {
            _context = context;
            this.mapper = mapper;
        }


        //////voy en video 109 minuto 1:35 
        ////public IUserRepository Users
        ////{
        ////    get
        ////    {
        ////        if (_users == null)
        ////        {
        ////            _users = new ProductRepository(_context, mapper);
        ////        }
        ////        return _users;
        ////    }
        ////}

        public ICategoryRepository Categories
        {
            get
            {
                if (_categories == null)
                {
                    _categories = new CategoryRepository(_context, mapper);
                }
                return _categories;
            }
        }

        public IProductRepository Products
        {
            get
            {
                if (_products == null)
                {
                    _products = new ProductRepository(_context, mapper);
                }
                return _products;
            }
        }



        //   Prefiero que se haga el SaveChange del context en cada metodo del Repository
        //   asi mantenemos que cada cosa se haga en su respectivo lugar, por ejemplo si
        //   hay que actualizar mas de un cosa en el la bbdd, lo hacemos en el metodo del
        //   repository y asi toda la logica se queda en el metodo
        //public async Task<int>  SaveAsync()
        //{
        //    return await _context.SaveChangesAsync();
        //}

        public void Dispose()
        {
            _context.Dispose();
        }


    }
}
