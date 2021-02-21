using AutoMapper;
using DevIO.App.Extensions;
using DevIO.App.ViewModels;
using DevIO.Business.Interfaces;
using DevIO.Business.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace DevIO.App.Controllers
{
    [Authorize]
    public class ProductsController : BaseController
    {
        private readonly IProductRepository _productRepository;
        private readonly ISupplierRepository _supplierRepository;
        private readonly IProductService _productService;
        private readonly IMapper _mapper;

        public ProductsController(
                IProductRepository productRepository,
                ISupplierRepository supplierRepository,
                IMapper mapper,
                IProductService productService,
                INotifier notifier
            ) :
            base(notifier)
        {
            _productRepository = productRepository;
            _supplierRepository = supplierRepository;
            _productService = productService;
            _mapper = mapper;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            return View(_mapper.Map<IEnumerable<ProductViewModel>>(await _productRepository.GetProductsSuppliers()));
        }

        [AllowAnonymous]
        public async Task<IActionResult> Details(Guid id)
        {
            var productViewModel = await GetProduct(id);

            if (productViewModel == null) return NotFound();

            return View(productViewModel);
        }

        [ClaimsAuthorizer("Produto", "Adicionar")]
        public async Task<IActionResult> Create()
        {
            var productViewModel = await PopulateSuppliers(new ProductViewModel());

            return View(productViewModel);
        }

        [HttpPost,
         ValidateAntiForgeryToken,
         ClaimsAuthorizer("Produto", "Adicionar")]
        public async Task<IActionResult> Create(ProductViewModel productViewModel)
        {
            productViewModel = await PopulateSuppliers(productViewModel);

            if (!ModelState.IsValid) return View(productViewModel);

            var imgPrefix = $"{Guid.NewGuid()}_";

            if (!await UploadFile(productViewModel.ImageUpload, imgPrefix))
            {
                return View(productViewModel);
            }

            productViewModel.Image = $"{imgPrefix}{productViewModel.ImageUpload.FileName}";

            await _productService.Add(_mapper.Map<Product>(productViewModel));

            if (!IsValidOperation()) return View(productViewModel);

            return RedirectToAction(nameof(Index));
        }

        [ClaimsAuthorizer("Produto", "Editar")]
        public async Task<IActionResult> Edit(Guid id)
        {
            var productViewModel = await GetProduct(id);

            if (productViewModel == null) return NotFound();

            return View(productViewModel);
        }

        [HttpPost,
         ValidateAntiForgeryToken, 
         ClaimsAuthorizer("Produto", "Editar")]
        public async Task<IActionResult> Edit(Guid id, ProductViewModel productViewModel)
        {
            if (id != productViewModel.Id) return NotFound();

            var productActual = await GetProduct(id);

            productViewModel.Supplier = productActual.Supplier;
            productViewModel.Image = productActual.Image;

            if (!ModelState.IsValid) return View(productViewModel);

            if (productViewModel.ImageUpload != null)
            {
                var imgPrefix = $"{Guid.NewGuid()}_";

                if (!await UploadFile(productViewModel.ImageUpload, imgPrefix))
                {
                    return View(productViewModel);
                }

                productActual.Image = $"{imgPrefix}{productViewModel.ImageUpload.FileName}";
            }

            productActual.Name = productViewModel.Name;
            productActual.Description = productViewModel.Description;
            productActual.Value = productViewModel.Value;
            productActual.Active = productViewModel.Active;

            await _productService.Update(_mapper.Map<Product>(productActual));

            if (!IsValidOperation()) return View(productViewModel);

            return RedirectToAction(nameof(Index));
        }

        [ClaimsAuthorizer("Produto", "Excluir")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var product = await GetProduct(id);

            if (product == null) return NotFound();

            return View(product);
        }

        [HttpPost,
         ActionName("Delete"),
         ValidateAntiForgeryToken, 
         ClaimsAuthorizer("Produto", "Excluir")]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var product = await GetProduct(id);

            if (product == null) return NotFound();

            await _productService.Remove(id);

            if (!IsValidOperation()) return View(product);

            TempData["Sucesso"] = "Produto excluido com sucesso.";

            return RedirectToAction(nameof(Index));
        }

        private async Task<ProductViewModel> GetProduct(Guid id)
        {
            var product = _mapper.Map<ProductViewModel>(await _productRepository.GetProductSupplier(id));
            product.Suppliers = _mapper.Map<IEnumerable<SupplierViewModel>>(await _supplierRepository.GetAll());

            return product;
        }

        private async Task<ProductViewModel> PopulateSuppliers(ProductViewModel product)
        {
            product.Suppliers = _mapper.Map<IEnumerable<SupplierViewModel>>(await _supplierRepository.GetAll());

            return product;
        }

        private async Task<bool> UploadFile(IFormFile pFile, string imgPrefix)
        {
            if (pFile.Length <= 0) return false;

            var vPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/imagens", $"{imgPrefix}{pFile.FileName}");

            if (System.IO.File.Exists(vPath))
            {
                ModelState.AddModelError(string.Empty, "Já existe um arquivo com esse nome!");
                return false;
            }

            using (var stream = new FileStream(vPath, FileMode.Create))
            {
                await pFile.CopyToAsync(stream);
            }

            return true;
        }
    }
}
