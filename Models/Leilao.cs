using System;
using System.ComponentModel.DataAnnotations;

namespace LanceCertoNovo.Models
{
    public class Leilao
    {
        public int Id { get; set; }

        [Required]
        public string Titulo { get; set; }

        public string Descricao { get; set; }

        [DataType(DataType.Currency)]
        public decimal ValorInicial { get; set; }

        public DateTime DataInicio { get; set; }

        public DateTime DataFim { get; set; }
    }
}

