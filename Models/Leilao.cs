using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace LanceCerto.WebApp.Models
{
    public class Leilao
    {
        [Key]
        public int LeilaoId { get; set; }

        // Imóvel vinculado ao leilão (obrigatório)
        [Required(ErrorMessage = "O imóvel é obrigatório.")]
        public int ImovelId { get; set; }

        [BindNever]
        public Imovel Imovel { get; set; } = null!;

        // Usuário vencedor do leilão (opcional)
        public int? VencedorId { get; set; }

        public Usuario? Vencedor { get; set; }

        // Data e hora de início
        [Required(ErrorMessage = "A data de início do leilão é obrigatória.")]
        [Display(Name = "Início do Leilão")]
        public DateTime InicioEm { get; set; }

        // Data e hora de fim
        [Required(ErrorMessage = "A data de término do leilão é obrigatória.")]
        [Display(Name = "Término do Leilão")]
        public DateTime FimEm { get; set; }

        // Status do leilão
        [Required(ErrorMessage = "O status do leilão é obrigatório.")]
        [Display(Name = "Status do Leilão")]
        public string Status { get; set; } = "PENDENTE"; // PENDENTE, ATIVO, ENCERRADO

        // Valor atual mais alto do lance
        [Display(Name = "Maior Lance Atual")]
        [Range(0, double.MaxValue, ErrorMessage = "O valor do lance deve ser zero ou positivo.")]
        public decimal MaiorLanceAtual { get; set; } = 0;
    }
}