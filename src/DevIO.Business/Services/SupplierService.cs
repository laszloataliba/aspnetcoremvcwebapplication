using DevIO.Business.Interfaces;
using DevIO.Business.Models;
using DevIO.Business.Models.Validations;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DevIO.Business.Services
{
    public class SupplierService : BaseService, ISupplierService
    {
        private readonly ISupplierRepository _supplierRepository;
        private readonly IAddressRepository _addressRepository;

        public SupplierService(
                ISupplierRepository supplierRepository,
                IAddressRepository addressRepository,
                INotifier notifier) :
            base(notifier)
        {
            _supplierRepository = supplierRepository;
            _addressRepository = addressRepository;
        }

        public async Task Add(Supplier supplier)
        {
            if (
                    (!ExecuteValidation(new SupplierValidation(), supplier)) 
                        || 
                    (!ExecuteValidation(new AddressValidation(), supplier.Address))
               ) return;

            if (_supplierRepository.Get(sup => sup.IdentificationNumber == supplier.IdentificationNumber).Result.Any())
            {
                Notify("Já existe um fornecedor com este documento informado.");
                return;
            }

            await _supplierRepository.Add(supplier);
        }

        public async Task Update(Supplier supplier)
        {
            if (!ExecuteValidation(new SupplierValidation(), supplier)) return;

            if (_supplierRepository.Get(sup => sup.IdentificationNumber == supplier.IdentificationNumber && sup.Id != supplier.Id).Result.Any())
            {
                Notify("Já existe um fornecedor com este documento informado.");
                return;
            }

            await _supplierRepository.Update(supplier);
        }

        public async Task UpdateAddress(Address address)
        {
            if (!ExecuteValidation(new AddressValidation(), address)) return;

            await _addressRepository.Update(address);
        }

        public async Task Remove(Guid id)
        {
            if (_supplierRepository.GetSupplierProductsAddress(id).Result.Products.Any())
            {
                Notify("O fornecedor possui produtos cadastrados!");
                return;
            }

            await _supplierRepository.Remove(id);
        }

        public void Dispose()
        {
            _supplierRepository?.Dispose();
            _addressRepository?.Dispose();
        }
    }
}
