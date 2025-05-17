namespace LanceCerto.WebApp.Models
{
    public class ImovelFavorito
    {
        // Chave composta: (UsuarioId, ImovelId)
        public int UsuarioId { get; set; }

        // Navegação obrigatória (muitos para um)
        public Usuario Usuario { get; set; } = null!;

        public int ImovelId { get; set; }

        public Imovel Imovel { get; set; } = null!;
    }
}