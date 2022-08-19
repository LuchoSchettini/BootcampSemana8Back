using AutoMapper;
using Store.ApplicationCore.Exceptions;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Store.ApplicationCore.Helpers.Utils;
using Store.ApplicationCore.Helpers.Pager;
using Store.ApplicationCore.Entities.Ctx01;
using Store.ApplicationCore.DTOs.Ctx01;
using Store.Infrastructure.Persistence.Contexts.Ctx01;
using Store.ApplicationCore.Interfaces;

namespace Store.Infrastructure.Persistence.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly Ctx01_Store storeContext;
        private readonly IMapper mapper;

        public ProductRepository(Ctx01_Store storeContext, IMapper mapper)
        {
            this.storeContext = storeContext;
            this.mapper = mapper;
        }

        public ProductResponse Create(CreateProductRequest request)
        {

            var product = mapper.Map<Product>(request);
            product.CreatedAt = product.UpdatedAt = DateUtil.GetCurrentDate();

            //comprobaciones extras
            if (storeContext.Products.AsNoTracking().Any(x => x.Name.ToLower() == product.Name.ToLower()))
            {
                throw new ConflictDbException("El nombre del producto ya existe");
            }
            if (!storeContext.Categories.Any(x => x.Id == product.CategoryId))
            {
                throw new ConflictDbException("El Codigo de Categoria no existe");
            }

            storeContext.Products.Add(product);
            storeContext.SaveChanges();

            return mapper.Map<ProductResponse>(product);
        }


        public void DeleteById(int productId)
        {
            var product = storeContext.Products.Find(productId);
            if (product != null)
            {
                storeContext.Products.Remove(product);
                storeContext.SaveChanges();
            }
            else
            {
                throw new NotFoundException();
            }
        }

        public ProductResponse GetById(int productId)
        {
            var product = storeContext.Products.Find(productId);
            if (product != null)
            {
                return mapper.Map<ProductResponse>(product);
            }

            throw new NotFoundException("El Codigo de Producto no existe");
        }

        public IReadOnlyList<ProductResponse> GetAll()
        {
            return storeContext.Products.AsNoTracking().Select(p => mapper.Map<ProductResponse>(p)).ToList();
        }

        public ProductResponse UpdateById(int productId, UpdateProductRequest request)
        {

            //comprobaciones extras
            if (request.Name != null) //ya que si es null le estamos dejando lo que ya habia en BBDD
            {
                if (storeContext.Products.AsNoTracking().Any(x => x.Name.ToLower() == request.Name.ToLower() && x.Id != productId))
                {
                    throw new ConflictDbException("El nombre de la categoria ya existe");
                }
            }
            if (request.CategoryId != null) //ya que si es null le estamos dejando lo que ya habia en BBDD
            {
                if (!storeContext.Categories.AsNoTracking().Any(x => x.Id == request.CategoryId))
                {
                    throw new ConflictDbException("El Codigo de Producto no existe");
                }
            }



            var productDb = storeContext.Products.Find(productId);
            if (productDb != null)
            {

                //ASI ERA ANTES:
                //**************************************************
                //product.Name = request.Name;
                //product.Description = request.Description;
                //product.Price = request.Price;
                //product.Stock = request.Stock;
                //product.UpdatedAt = DateUtil.GetCurrentDate();
                //**************************************************


                ////                AHORA:
                ////************************************************
                ////     Truco para actualializar con automaper. 
                ////     Esto lo que hace es actalizar los campos que la variable Request que es de tipo
                ////     UpdateProductRequest en la variable product que es de tipo Product
                //product = mapper.Map(request, product);
                ////     y luego podemos actualizar otros campos esto:
                //product.UpdatedAt = DateUtil.GetCurrentDate();





                request.Name = request.Name == null ? productDb.Name : request.Name;
                request.Description = request.Description == null ? productDb.Description : request.Description;
                request.Price = request.Price == null ? productDb.Price : request.Price;
                request.Stock = request.Stock == null ? productDb.Stock : request.Stock;
                request.CategoryId = request.CategoryId == null ? productDb.CategoryId : request.CategoryId;


                productDb = mapper.Map(request, productDb);
                productDb.UpdatedAt = DateUtil.GetCurrentDate();
                ////************************************************


                storeContext.Products.Update(productDb);
                storeContext.SaveChanges();

                return mapper.Map<ProductResponse>(productDb);
            }

            throw new NotFoundException();
        }



        public async Task<PagerResponse<ProductResponse>> GetAllFiltringAsync(
    PagerRequest pagerRequest)
        {
            var consulta = storeContext.Products as IQueryable<Product>;

            if (!string.IsNullOrEmpty(pagerRequest.Search))
            {
                consulta = consulta.Where(p => p.Name.ToLower().Contains(pagerRequest.Search.ToLower()));
            }

            var totalRegistros = await consulta.CountAsync();

            var registros = await consulta.AsNoTracking()
               .Skip((pagerRequest.PageIndex - 1) * pagerRequest.PageSize)
               .Take(pagerRequest.PageSize)
               .Select(p => mapper.Map<ProductResponse>(p))
               .ToListAsync();

            var res = new PagerResponse<ProductResponse>(registros, totalRegistros, pagerRequest.PageIndex, pagerRequest.PageSize, pagerRequest.Search);
            return res;
        }



    }
}