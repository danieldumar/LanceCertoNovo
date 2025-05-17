using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LanceCerto.WebApp.Models
{
    public class Usuario : IdentityUser<int> // PK int
    {
        [PersonalData]
        [Required(ErrorMessage = "O nome completo é obrigatório.")]
        [StringLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres.")]
        [Display(Name = "Nome Completo")]
        public string Nome { get; set; } = string.Empty;

        [PersonalData]
        [Required(ErrorMessage = "A data de nascimento é obrigatória.")]
        [DataType(DataType.Date)]
        [Display(Name = "Data de Nascimento")]
        public DateTime DataNascimento { get; set; }

        [StringLength(50, ErrorMessage = "O campo Estado deve ter no máximo 50 caracteres.")]
        [Display(Name = "Estado (UF)")]
        public string? Estado { get; set; }

        [Display(Name = "É Vendedor?")]
        public bool EhVendedor { get; set; }

        [Display(Name = "É Corretor?")]
        public bool EhCorretor { get; set; }

        [StringLength(20, ErrorMessage = "O CRECI deve ter no máximo 20 caracteres.")]
        [Display(Name = "CRECI (opcional)")]
        public string? Creci { get; set; }

        // Navegações opcionais – podem ser habilitadas conforme necessidade
        // public ICollection<Lance>? Lances { get; set; }
        // public ICollection<Mensagem>? MensagensEnviadas { get; set; }
        // public ICollection<Mensagem>? MensagensRecebidas { get; set; }
    }
}