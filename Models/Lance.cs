using System.ComponentModel.DataAnnotations;

namespace LanceCerto.WebApp.Models
{
    public class Lance
    {
        public int LanceId { get; set; }

        // Relacionamento com Leilão (obrigatório)
        [Required]
        public int LeilaoId { get; set; }

        public Leilao Leilao { get; set; } = null!;

        // Relacionamento com Usuário (obrigatório)
        [Required]
        public int UsuarioId { get; set; }

        public Usuario Usuario { get; set; } = null!;

        // Valor do lance
        [Required(ErrorMessage = "O valor do lance é obrigatório.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "O valor do lance deve ser maior que zero.")]
        public decimal ValorLance { get; set; }

        // Momento em que o lance foi feito
        [Required]
        [Display(Name = "Data e Hora do Lance")]
        public DateTime MomentoLance { get; set; }
    }
}