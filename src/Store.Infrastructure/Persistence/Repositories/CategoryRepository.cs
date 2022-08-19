using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Store.ApplicationCore.DTOs.Ctx01;
using Store.ApplicationCore.Entities.Ctx01;
using Store.ApplicationCore.Exceptions;
using Store.ApplicationCore.Helpers.Pager;
using Store.ApplicationCore.Helpers.Utils;
using Store.ApplicationCore.Interfaces;
using Store.Infrastructure.Persistence.Contexts.Ctx01;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Store.Infrastructure.Persistence.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly Ctx01_Store storeContext;
        private readonly IMapper mapper;

        public CategoryRepository(Ctx01_Store storeContext, IMapper mapper)
        {
            this.storeContext = storeContext;
            this.mapper = mapper;
        }

        public CategoryResponse Create(CreateCategoryRequest request)
        {
            var category = mapper.Map<Category>(request);
            category.CreatedAt = category.UpdatedAt = DateUtil.GetCurrentDate();

            //comprobaciones extras
            if (storeContext.Categories.AsNoTracking().Any(x => x.Name.ToLower() == category.Name.ToLower()))
            {
                throw new ConflictDbException("El nombre de la categoria ya existe");
            }

            storeContext.Categories.Add(category);
            storeContext.SaveChanges();

            return mapper.Map<CategoryResponse>(category);
        }


        public void DeleteById(int CategoryId)
        {
            var categoryDb = storeContext.Categories.Find(CategoryId);
            if (categoryDb != null)
            {
                storeContext.Categories.Remove(categoryDb);
                storeContext.SaveChanges();
            }
            else
            {
                throw new NotFoundException("la Categoria a eliminar no existe");
            }
        }

        public CategoryResponse GetById(int categoryId)
        {
            var categoryDb = storeContext.Categories.Find(categoryId);
            if (categoryDb != null)
            {
                return mapper.Map<CategoryResponse>(categoryDb);
            }

            throw new NotFoundException("El Categoria solicitada no existe");
        }

        //public List<CategoryResponse> GetAll()
        //{
        //    return storeContext.Categories.Select(p => mapper.Map<CategoryResponse>(p)).ToList();
        //}
        public IReadOnlyList<CategoryResponse> GetAll()
        {
            return storeContext.Categories.AsNoTracking().Select(p => mapper.Map<CategoryResponse>(p)).ToList();
        }


        public CategoryResponse UpdateById(int categoryId, UpdateCategoryRequest request)
        {

            //comprobaciones extras
            if (request.Name != null) //ya que si es null le estamos dejando lo que ya habia en BBDD
            {
                if (storeContext.Categories.AsNoTracking().Any(x => x.Name.ToLower() == request.Name.ToLower() && x.Id != categoryId))
                {
                    throw new ConflictDbException("El nombre de la categoria ya existe");
                }
            }

            var categoryDb = storeContext.Categories.Find(categoryId);

            if (categoryDb != null)
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

                //solo actualiza los campos que tenemos en UpdateProductRequest
                //si alguno esta a null le ponemos el mismo valor que ya tenia


                request.Name = request.Name == null ? categoryDb.Name : request.Name;
                request.Description = request.Description == null ? categoryDb.Description : request.Description;

                categoryDb = mapper.Map(request, categoryDb);
                categoryDb.UpdatedAt = DateUtil.GetCurrentDate();
            ////************************************************


            categoryDb.Id = categoryId;

                storeContext.Categories.Update(categoryDb);
                storeContext.SaveChanges();

                return mapper.Map<CategoryResponse>(categoryDb);
            }

            throw new NotFoundException("No existe la categoria solicitada");
        }




        //PagerCategoryResponse
        //public async Task<(int totalRegistros, IEnumerable<CategoryResponse> registros)> GetAllFiltringAsync(
        //    int pageIndex, int pageSize, string search)
        // public async Task<PagerCategoryResponse> GetAllFiltringAsync(
        //int pageIndex, int pageSize, string search)

        public async Task<PagerResponse<CategoryResponse>> GetAllFiltringAsync(
            PagerRequest pagerRequest)
        {
            var consulta = storeContext.Categories as IQueryable<Category>;

            if (!string.IsNullOrEmpty(pagerRequest.Search))
            {
                consulta = consulta.Where(p => p.Name.ToLower().Contains(pagerRequest.Search.ToLower()));
            }

            var totalRegistros = await consulta.CountAsync();

            var registros = await consulta.AsNoTracking()
               .Skip((pagerRequest.PageIndex - 1) * pagerRequest.PageSize)
               .Take(pagerRequest.PageSize)
               .Select(c => mapper.Map<CategoryResponse>(c))
               .ToListAsync();

            var res = new PagerResponse<CategoryResponse>(registros, totalRegistros, pagerRequest.PageIndex, pagerRequest.PageSize, pagerRequest.Search);
            return res;
        }

    }
}