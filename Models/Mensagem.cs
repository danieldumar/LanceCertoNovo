using System;
using System.ComponentModel.DataAnnotations;

namespace LanceCerto.WebApp.Models
{
    public class Mensagem
    {
        [Key]
        public int MensagemId { get; set; }

        [Required]
        public int RemetenteId { get; set; }

        [Required]
        public Usuario Remetente { get; set; } = null!;

        [Required]
        public int DestinatarioId { get; set; }

        [Required]
        public Usuario Destinatario { get; set; } = null!;

        public int? ImovelRelacionadoId { get; set; }

        public Imovel? ImovelRelacionado { get; set; }

        [StringLength(2000, ErrorMessage = "A mensagem pode ter no máximo 2000 caracteres.")]
        [Display(Name = "Conteúdo da Mensagem")]
        public string? Conteudo { get; set; }

        [Display(Name = "Data de Envio")]
        public DateTime EnviadaEm { get; set; } = DateTime.UtcNow;
    }
}