using System.ComponentModel.DataAnnotations;

namespace LanceCerto.WebApp.Models
{
    public class Imovel
    {
        public int ImovelId { get; set; }

        [Required(ErrorMessage = "O título é obrigatório.")]
        [StringLength(100, ErrorMessage = "O título pode ter no máximo 100 caracteres.")]
        [Display(Name = "Título")]
        public string Titulo { get; set; } = string.Empty;

        [Required(ErrorMessage = "A descrição é obrigatória.")]
        [Display(Name = "Descrição")]
        public string Descricao { get; set; } = string.Empty;

        [Required(ErrorMessage = "O tipo do imóvel é obrigatório.")]
        [Display(Name = "Tipo do Imóvel")]
        public string Tipo { get; set; } = string.Empty;

        [Required(ErrorMessage = "O endereço é obrigatório.")]
        [Display(Name = "Endereço")]
        public string Endereco { get; set; } = string.Empty;

        [Required(ErrorMessage = "A cidade é obrigatória.")]
        [Display(Name = "Cidade")]
        public string Cidade { get; set; } = string.Empty;

        [Required(ErrorMessage = "O estado é obrigatório.")]
        [StringLength(2, ErrorMessage = "Informe a sigla do estado com 2 letras.")]
        [Display(Name = "Estado (UF)")]
        public string Estado { get; set; } = string.Empty;

        [Required(ErrorMessage = "O preço mínimo é obrigatório.")]
        [Range(1, double.MaxValue, ErrorMessage = "Informe um valor maior que zero.")]
        [Display(Name = "Preço Mínimo")]
        public decimal PrecoMinimo { get; set; }

        [Required(ErrorMessage = "O status do imóvel é obrigatório.")]
        [Display(Name = "Status")]
        public string Status { get; set; } = string.Empty;

        [Display(Name = "Imagem (URL)")]
        [DataType(DataType.ImageUrl)]
        public string? ImagemUrl { get; set; }

        // Relacionamento com o usuário (proprietário ou anunciante)
        public int? UsuarioId { get; set; }

        [Display(Name = "Anunciante")]
        public Usuario? Usuario { get; set; }

        // Navegação reversa: favoritos relacionados a este imóvel
        public ICollection<ImovelFavorito> ImoveisFavoritos { get; set; } = new List<ImovelFavorito>();
    }
}