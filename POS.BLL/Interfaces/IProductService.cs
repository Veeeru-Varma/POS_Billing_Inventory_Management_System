using POS.Models.Common;
using POS.Models.DTOs;
using POS.Models.Entities;

namespace POS.BLL.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<Product>> GetAllAsync();

        Task<Product?> GetByIdAsync(int productId);

        Task<IEnumerable<Product>> SearchAsync(string searchText);

        Task<ApiResponse<int>> AddAsync(ProductRequestDto model);

        Task<ApiResponse> UpdateAsync(int productId, ProductRequestDto model);

        Task<ApiResponse> DeleteAsync(int productId);

        Task<IEnumerable<Product>> GetLowStockAsync();
    }
}
