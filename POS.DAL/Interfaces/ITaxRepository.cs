using POS.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.DAL.Interfaces
{
    public interface ITaxRepository
    {
        Task<TaxSetting?> GetActiveTaxAsync();

    }
}
