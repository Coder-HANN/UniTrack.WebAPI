using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

    // Diğer using'ler silindi, artık gerekmiyor

    namespace UniTrack.Persistence.Context
    {
        // Bu fabrika sınıfı, EF Core araçlarına, DI sistemini kullanmadan 
        // DbContext'i nasıl oluşturacaklarını söyler.
        public class UniTrackDbContextFactory : IDesignTimeDbContextFactory<UniTrackDbContext>
        {
            public UniTrackDbContext CreateDbContext(string[] args)
            {
                var optionsBuilder = new DbContextOptionsBuilder<UniTrackDbContext>();
                optionsBuilder.UseSqlServer("Server=DESKTOP-GEU3I6M\\SQLEXPRESS;Database=UniTrackDb;Trusted_Connection=True;TrustServerCertificate=True;");
                return new UniTrackDbContext(optionsBuilder.Options);
            }
        }
    }
