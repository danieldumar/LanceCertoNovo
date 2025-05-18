using System.ComponentModel.DataAnnotations;

namespace LanceCerto.WebApp.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "O e-mail é obrigatório.")]
        [EmailAddress(ErrorMessage = "Informe um e-mail válido.")]
        [StringLength(256, ErrorMessage = "O e-mail pode ter no máximo 256 caracteres.")]
        [Display(Name = "E-mail")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "A senha é obrigatória.")]
        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "A senha pode ter no máximo 100 caracteres.")]
        [Display(Name = "Senha")]
        public string Senha { get; set; } = string.Empty;

        [Display(Name = "Lembrar-me")]
        public bool LembrarMe { get; set; }

        /// <summary>
        /// URL para redirecionar após login bem-sucedido.
        /// </summary>
        public string? ReturnUrl { get; set; }
    }
}