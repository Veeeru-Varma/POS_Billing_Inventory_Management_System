using Dapper;
using POS.DAL.Context;
using POS.DAL.Interfaces;
using POS.Models.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.DAL.Repositories
{
    public class TaxRepository : ITaxRepository
    {
        private readonly DapperContext _context;

        public TaxRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<TaxSetting?> GetActiveTaxAsync()
        {
            using var connection = _context.CreateConnection();

            return await connection.QueryFirstOrDefaultAsync<TaxSetting>(
                "sp_Tax_GetActive",
                commandType: CommandType.StoredProcedure);
        }
    }
}
