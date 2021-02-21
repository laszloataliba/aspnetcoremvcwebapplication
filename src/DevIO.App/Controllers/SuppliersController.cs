using AutoMapper;
using DevIO.App.Extensions;
using DevIO.App.ViewModels;
using DevIO.Business.Interfaces;
using DevIO.Business.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevIO.App.Controllers
{
    [Authorize]
    public class SuppliersController : BaseController
    {
        private readonly ISupplierRepository _supplierRepository;
        private readonly ISupplierService _supplierService;
        private readonly IMapper _mapper;

        public SuppliersController(
                ISupplierRepository context, 
                ISupplierService supplierService, 
                IMapper mapper, 
                INotifier notifier
            ) : 
            base(notifier)
        {
            _supplierRepository = context;
            _supplierService = supplierService;
            _mapper = mapper;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            return View(_mapper.Map<IEnumerable<SupplierViewModel>>(await _supplierRepository.GetAll()));
        }

        [AllowAnonymous]
        public async Task<IActionResult> Details(Guid id)
        {
            var supplierViewModel = await GetSupplierAddress(id);

            if (supplierViewModel == null)
                return NotFound();

            return View(supplierViewModel);
        }

        [ClaimsAuthorizer("Fornecedor", "Adicionar")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost,
         ValidateAntiForgeryToken, 
         ClaimsAuthorizer("Fornecedor", "Adicionar")]
        public async Task<IActionResult> Create(SupplierViewModel supplierViewModel)
        {
            if (!ModelState.IsValid) return View(supplierViewModel);

            var supplier = _mapper.Map<Supplier>(supplierViewModel);

            await _supplierService.Add(supplier);

            if (!IsValidOperation()) return View(supplierViewModel);

            return RedirectToAction(nameof(Index));
        }

        [ClaimsAuthorizer("Fornecedor", "Editar")]
        public async Task<IActionResult> Edit(Guid id)
        {
            var supplierViewModel = await GetSupplierProductsAddress(id);

            if (supplierViewModel == null) return NotFound();

            return View(supplierViewModel);
        }

        [HttpPost,
         ValidateAntiForgeryToken, 
         ClaimsAuthorizer("Fornecedor", "Editar")]
        public async Task<IActionResult> Edit(Guid id, SupplierViewModel supplierViewModel)
        {
            if (id != supplierViewModel.Id) return NotFound();

            if (!ModelState.IsValid) return View(supplierViewModel);

            var suppplier = _mapper.Map<Supplier>(supplierViewModel);

            await _supplierService.Update(suppplier);

            if (!IsValidOperation()) return View(await GetSupplierProductsAddress(id));

            return RedirectToAction(nameof(Index));
        }

        [ClaimsAuthorizer("Fornecedor", "Excluir")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var supplierViewModel = await GetSupplierAddress(id);

            if (supplierViewModel == null) return NotFound();

            return View(supplierViewModel);
        }

        [HttpPost,
         ActionName("Delete"),
         ValidateAntiForgeryToken, 
         ClaimsAuthorizer("Fornecedor", "Excluir")]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var supplierViewModel = await GetSupplierAddress(id);

            if (supplierViewModel == null) return NotFound();

            await _supplierService.Remove(id);

            if (!IsValidOperation()) return View(supplierViewModel);
            
            return RedirectToAction(nameof(Index));
        }

        [ClaimsAuthorizer("Fornecedor", "Editar")]
        public async Task<IActionResult> UpdateAddress(Guid id)
        {
            var supplier = await GetSupplierAddress(id);

            if (supplier == null) return NotFound();

            return PartialView("_UpdateAddress", new SupplierViewModel { Address = supplier.Address });
        }

        [HttpPost,
         ValidateAntiForgeryToken, 
         ClaimsAuthorizer("Fornecedor", "Editar")]
        public async Task<IActionResult> UpdateAddress(SupplierViewModel supplierViewModel)
        {
            ModelState.Remove(nameof(SupplierViewModel.Name));
            ModelState.Remove(nameof(SupplierViewModel.IdentificationNumber));

            if (!ModelState.IsValid) return PartialView("_UpdateAddress", supplierViewModel);

            await _supplierService.UpdateAddress(_mapper.Map<Address>(supplierViewModel.Address));

            if (!IsValidOperation()) return PartialView("_UpdateAddress", supplierViewModel);

            var url = Url.Action("GetAddress", "Suppliers", new { id = supplierViewModel.Address.SupplierId });

            return Json(new { success = true, url });
        }

        [AllowAnonymous]
        public async Task<IActionResult> GetAddress(Guid id)
        {
            var supplier = await GetSupplierAddress(id);

            if (supplier == null) return NotFound();

            return PartialView("_DetailsAddress", supplier);
        }

        private async Task<SupplierViewModel> GetSupplierAddress(Guid id)
        {
            return _mapper.Map<SupplierViewModel>(await _supplierRepository.GetSupplierAddress(id));
        }

        private async Task<SupplierViewModel> GetSupplierProductsAddress(Guid id)
        {
            return _mapper.Map<SupplierViewModel>(await _supplierRepository.GetSupplierProductsAddress(id));
        }
    }
}
