using Microsoft.EntityFrameworkCore;
using LanceCertoNovo.Models;

namespace LanceCertoNovo.Data
{
    public class LanceCertoContext : DbContext
    {
        public LanceCertoContext(DbContextOptions<LanceCertoContext> options)
            : base(options)
        {
        }

        public DbSet<Leilao> Leiloes { get; set; }
    }
}

