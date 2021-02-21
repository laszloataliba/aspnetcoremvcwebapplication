using DevIO.Business.Models;
using System;
using System.Threading.Tasks;

namespace DevIO.Business.Interfaces
{
    public interface ISupplierService : IDisposable
    {
        Task Add(Supplier supplier);
        Task Update(Supplier supplier);
        Task UpdateAddress(Address address);
        Task Remove(Guid id);
    }
}
