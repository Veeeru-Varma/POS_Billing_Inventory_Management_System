using Dapper;
using POS.DAL.Context;
using POS.DAL.Interfaces;
using POS.Models.Common;
using POS.Models.DTOs;
using POS.Models.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.DAL.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly DapperContext _context;

        public ProductRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            using var connection = _context.CreateConnection();

            return await connection.QueryAsync<Product>(
                "sp_Product_GetAll",
                commandType: CommandType.StoredProcedure);
        }
        public async Task<Product?> GetByIdAsync(int productId)
        {
            using var connection = _context.CreateConnection();

            return await connection.QueryFirstOrDefaultAsync<Product>(
                "sp_Product_GetById",
                new
                {
                    ProductId = productId
                },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<Product>> SearchAsync(string searchText)
        {
            using var connection = _context.CreateConnection();

            return await connection.QueryAsync<Product>(
                "sp_Product_Search",
                new
                {
                    SearchText = searchText
                },
                commandType: CommandType.StoredProcedure);
        }
        public async Task<ApiResponse<int>> InsertAsync(ProductRequestDto model)
        {
            using var connection = _context.CreateConnection();

            var result = await connection.QueryFirstAsync<dynamic>(
                "sp_Product_Insert",
                new
                {
                    model.Name,
                    model.SKU,
                    model.Category,
                    model.Price,
                    model.StockQuantity,
                    model.LowStockThreshold
                },
                commandType: CommandType.StoredProcedure);

            return new ApiResponse<int>
            {
                Success = Convert.ToInt32(result.Success) == 1,
                Message = result.Message?.ToString() ?? string.Empty,
                Data = result.ProductId == null
                    ? 0
                    : Convert.ToInt32(result.ProductId)
            };
        }
        public async Task<ApiResponse> UpdateAsync(int productId, ProductRequestDto model)
        {
            using var connection = _context.CreateConnection();

            var result = await connection.QueryFirstAsync<dynamic>(
                "sp_Product_Update",
                new
                {
                    ProductId = productId,
                    model.Name,
                    model.SKU,
                    model.Category,
                    model.Price,
                    model.StockQuantity,
                    model.LowStockThreshold
                },
                commandType: CommandType.StoredProcedure);

            return new ApiResponse
            {
                Success = Convert.ToInt32(result.Success) == 1,
                Message = result.Message?.ToString() ?? string.Empty
            };
        }
        public async Task<ApiResponse> DeleteAsync(int productId)
        {
            using var connection = _context.CreateConnection();

            var result = await connection.QueryFirstAsync<dynamic>(
                "sp_Product_Delete",
                new { ProductId = productId },
                commandType: CommandType.StoredProcedure);

            return new ApiResponse
            {
                Success = Convert.ToInt32(result.Success) == 1,
                Message = result.Message?.ToString() ?? string.Empty
            };
        }

        public async Task<IEnumerable<Product>> GetLowStockAsync()
        {
            using var connection = _context.CreateConnection();

            return await connection.QueryAsync<Product>(
                "sp_Product_GetLowStock",
                commandType: CommandType.StoredProcedure);
        }
    }
}
