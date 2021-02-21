using DevIO.Business.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DevIO.App.ViewModels
{
    public class SupplierViewModel
    {
        [Key]
        public Guid Id { get; set; }

        [DisplayName("Nome"), 
         StringLength(200, ErrorMessage = "O campo {0} precisa ter entre {2} e {1} caracteres", MinimumLength = 20)]
        public string Name { get; set; }

        [DisplayName("Documento"), 
         Required(ErrorMessage = "O campo {0} é obrigatório"), 
         StringLength(14, ErrorMessage = "O campo {0} precisa ter entre {2} e {1} caracteres", MinimumLength = 7)]
        public string IdentificationNumber { get; set; }

        [DisplayName("Tipo")]
        public SupplierType SupplierType { get; set; } = SupplierType.PhysicalPerson;

        public AddressViewModel Address { get; set; }

        [DisplayName("Ativo?")]
        public bool Active { get; set; }

        public IEnumerable<ProductViewModel> Products { get; set; }
    }
}
