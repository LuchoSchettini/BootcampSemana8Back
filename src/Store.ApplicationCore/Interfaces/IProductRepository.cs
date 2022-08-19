using Store.ApplicationCore.DTOs.Ctx01;
using Store.ApplicationCore.Helpers.Pager;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Store.ApplicationCore.Interfaces
{
    public interface IProductRepository
    {
        IReadOnlyList<ProductResponse> GetAll();

        ProductResponse GetById(int productId);

        void DeleteById(int productId);

        ProductResponse Create(CreateProductRequest request);

        ProductResponse UpdateById(int productId, UpdateProductRequest request);

        Task<PagerResponse<ProductResponse>> GetAllFiltringAsync(PagerRequest pagerRequest);
    }
}