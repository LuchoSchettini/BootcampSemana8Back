using Store.ApplicationCore.DTOs.Ctx01;
using Store.ApplicationCore.Helpers.Pager;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Store.ApplicationCore.Interfaces
{
    public interface ICategoryRepository
    {
        IReadOnlyList<CategoryResponse> GetAll();

        CategoryResponse GetById(int categoryId);

        void DeleteById(int categoryId);

        CategoryResponse Create(CreateCategoryRequest request);

        CategoryResponse UpdateById(int categoryId, UpdateCategoryRequest request);

        // Task<PagerResponse<CategoryResponse>> GetAllFiltringAsync(int pageIndex, int pageSize, string search);
        Task<PagerResponse<CategoryResponse>> GetAllFiltringAsync(PagerRequest pagerRequest);
    }
}