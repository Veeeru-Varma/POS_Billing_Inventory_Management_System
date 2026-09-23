using POS.BLL.Interfaces;
using POS.Models.Common;
using POS.Models.DTOs;
using POS.Models.Entities;
using POS.DAL.Interfaces;

namespace POS.BLL.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _productRepository.GetAllAsync();
        }

        public async Task<Product?> GetByIdAsync(int productId)
        {
            return await _productRepository.GetByIdAsync(productId);
        }

        public async Task<IEnumerable<Product>> SearchAsync(string searchText)
        {
            return await _productRepository.SearchAsync(searchText);
        }

        public async Task<ApiResponse<int>> AddAsync(ProductRequestDto model)
        {
            if (string.IsNullOrWhiteSpace(model.Name))
            {
                return new ApiResponse<int>
                {
                    Success = false,
                    Message = "Product name is required."
                };
            }

            if (string.IsNullOrWhiteSpace(model.SKU))
            {
                return new ApiResponse<int>
                {
                    Success = false,
                    Message = "SKU is required."
                };
            }

            if (model.Price < 0)
            {
                return new ApiResponse<int>
                {
                    Success = false,
                    Message = "Price cannot be negative."
                };
            }

            if (model.StockQuantity < 0)
            {
                return new ApiResponse<int>
                {
                    Success = false,
                    Message = "Stock quantity cannot be negative."
                };
            }

            if (model.LowStockThreshold < 0)
            {
                return new ApiResponse<int>
                {
                    Success = false,
                    Message = "Low stock threshold cannot be negative."
                };
            }

            return await _productRepository.InsertAsync(model);
        }

        public async Task<ApiResponse> UpdateAsync(
            int productId,
            ProductRequestDto model)
        {
            if (productId <= 0)
            {
                return new ApiResponse
                {
                    Success = false,
                    Message = "Invalid product ID."
                };
            }

            if (string.IsNullOrWhiteSpace(model.Name))
            {
                return new ApiResponse
                {
                    Success = false,
                    Message = "Product name is required."
                };
            }

            if (string.IsNullOrWhiteSpace(model.SKU))
            {
                return new ApiResponse
                {
                    Success = false,
                    Message = "SKU is required."
                };
            }

            return await _productRepository.UpdateAsync(productId, model);
        }

        public async Task<ApiResponse> DeleteAsync(int productId)
        {
            if (productId <= 0)
            {
                return new ApiResponse
                {
                    Success = false,
                    Message = "Invalid product ID."
                };
            }

            return await _productRepository.DeleteAsync(productId);
        }

        public async Task<IEnumerable<Product>> GetLowStockAsync()
        {
            return await _productRepository.GetLowStockAsync();
        }
    }
}
