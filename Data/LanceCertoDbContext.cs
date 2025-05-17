using System;
using System.Linq;
using LanceCerto.WebApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LanceCerto.WebApp.Data
{
    public class LanceCertoDbContext : IdentityDbContext<Usuario, IdentityRole<int>, int>
    {
        public LanceCertoDbContext(DbContextOptions<LanceCertoDbContext> options)
            : base(options)
        {
        }

        // DbSets para entidades principais do projeto
        public DbSet<Imovel> Imoveis { get; set; } = null!;
        public DbSet<Leilao> Leiloes { get; set; } = null!;
        public DbSet<Lance> Lances { get; set; } = null!;
        public DbSet<Mensagem> Mensagens { get; set; } = null!;
        public DbSet<ImovelFavorito> ImoveisFavoritos { get; set; } = null!;
        public DbSet<Usuario> Usuarios { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // Chamada essencial para configurações do Identity
            base.OnModelCreating(builder);

            // Renomear tabelas padrão do Identity
            builder.Entity<Usuario>().ToTable("Usuarios");
            builder.Entity<IdentityRole<int>>().ToTable("Roles");
            builder.Entity<IdentityUserRole<int>>().ToTable("UsuarioRoles");
            builder.Entity<IdentityUserClaim<int>>().ToTable("UsuarioClaims");
            builder.Entity<IdentityUserLogin<int>>().ToTable("UsuarioLogins");
            builder.Entity<IdentityRoleClaim<int>>().ToTable("RoleClaims");
            builder.Entity<IdentityUserToken<int>>().ToTable("UsuarioTokens");

            // Configuração para ImovelFavorito (chave composta)
            builder.Entity<ImovelFavorito>(entity =>
            {
                entity.HasKey(e => new { e.UsuarioId, e.ImovelId });

                entity.HasOne(e => e.Imovel)
                      .WithMany(i => i.ImoveisFavoritos)
                      .HasForeignKey(e => e.ImovelId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Usuario)
                      .WithMany()
                      .HasForeignKey(e => e.UsuarioId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Correção de múltiplos caminhos de deleção em Mensagem
            builder.Entity<Mensagem>(entity =>
            {
                entity.HasOne(m => m.Remetente)
                      .WithMany()
                      .HasForeignKey(m => m.RemetenteId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(m => m.Destinatario)
                      .WithMany()
                      .HasForeignKey(m => m.DestinatarioId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(m => m.ImovelRelacionado)
                      .WithMany()
                      .HasForeignKey(m => m.ImovelRelacionadoId)
                      .OnDelete(DeleteBehavior.SetNull);
            });

            // Configuração para lances e valores padrão mantidos no código de domínio, mas o tipo será definido genericamente abaixo

            // Aplica REAL a todas as propriedades decimal do modelo para SQLite
            var decimalProperties = builder.Model
                .GetEntityTypes()
                .SelectMany(t => t.GetProperties())
                .Where(p => p.ClrType == typeof(decimal));

            foreach (var property in decimalProperties)
            {
                property.SetColumnType("REAL");
            }
        }
    }
}