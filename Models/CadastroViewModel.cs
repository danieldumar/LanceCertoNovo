using System;
using System.ComponentModel.DataAnnotations;

namespace LanceCerto.WebApp.Models
{
    public class CadastroViewModel
    {
        [Required(ErrorMessage = "O nome completo é obrigatório.")]
        [StringLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres.")]
        [Display(Name = "Nome Completo")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "O e-mail é obrigatório.")]
        [EmailAddress(ErrorMessage = "E-mail inválido.")]
        [StringLength(256, ErrorMessage = "O e-mail deve ter no máximo 256 caracteres.")]
        [Display(Name = "E-mail")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "A senha é obrigatória.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "A senha deve ter entre 6 e 100 caracteres.")]
        [DataType(DataType.Password)]
        [Display(Name = "Senha")]
        public string Senha { get; set; } = string.Empty;

        [Required(ErrorMessage = "A data de nascimento é obrigatória.")]
        [DataType(DataType.Date)]
        [Display(Name = "Data de Nascimento")]
        public DateTime DataNascimento { get; set; }

        [Required(ErrorMessage = "O estado (UF) é obrigatório.")]
        [StringLength(2, MinimumLength = 2, ErrorMessage = "O UF deve ter 2 caracteres.")]
        [Display(Name = "Estado (UF)")]
        public string Estado { get; set; } = string.Empty;

        [Display(Name = "Sou Vendedor")]
        public bool EhVendedor { get; set; }

        [Display(Name = "Sou Corretor")]
        public bool EhCorretor { get; set; }

        [StringLength(20, ErrorMessage = "O CRECI deve ter no máximo 20 caracteres.")]
        [Display(Name = "CRECI (se aplicável)")]
        public string? Creci { get; set; }
    }
}