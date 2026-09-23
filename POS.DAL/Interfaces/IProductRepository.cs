using POS.Models.Common;
using POS.Models.DTOs;
using POS.Models.Entities;

namespace POS.DAL.Interfaces
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetAllAsync();

        Task<Product?> GetByIdAsync(int productId);

        Task<IEnumerable<Product>> SearchAsync(string searchText);

        Task<ApiResponse<int>> InsertAsync(ProductRequestDto model);

        Task<ApiResponse> UpdateAsync(int productId, ProductRequestDto model);

        Task<ApiResponse> DeleteAsync(int productId);

        Task<IEnumerable<Product>> GetLowStockAsync();
    }
}
