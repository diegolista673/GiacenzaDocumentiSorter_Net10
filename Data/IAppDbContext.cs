using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using GiacenzaSorterRm.Models.Database;
using GiacenzaSorterRm.Models;
using System.Threading;
using System.Threading.Tasks;

namespace GiacenzaSorterRm.Data
{
    /// <summary>
    /// Interfaccia comune per i DbContext (SQL Server e SQLite)
    /// Permette di usare dependency injection indipendentemente dal database
    /// </summary>
    public interface IAppDbContext
    {
        // DbSets esposti - stessi nomi del context originale
        DbSet<Bancali> Bancalis { get; set; }
        DbSet<BancaliDispacci> BancaliDispaccis { get; set; }
        DbSet<CentriLav> CentriLavs { get; set; }
        DbSet<CommessaTipologiaContenitore> CommessaTipologiaContenitores { get; set; }
        DbSet<Commesse> Commesses { get; set; }
        DbSet<Contenitori> Contenitoris { get; set; }
        DbSet<Operatori> Operatoris { get; set; }
        DbSet<Piattaforme> Piattaformes { get; set; }
        DbSet<Scatole> Scatoles { get; set; }
        DbSet<Stati> Statis { get; set; }
        DbSet<TipiNormalizzazione> TipiNormalizzaziones { get; set; }
        DbSet<Tipologie> Tipologies { get; set; }
        
        // Keyless entities per query raw SQL
        DbSet<MaceroView> MaceroView { get; set; }
        DbSet<GiacenzaView> GiacenzaView { get; set; }
        DbSet<BancaleFuoriSlaView> BancaleFuoriSlaView { get; set; }

        // Metodi EF Core essenziali
        int SaveChanges();
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        
        // Accesso a funzionalità EF Core
        ChangeTracker ChangeTracker { get; }
        DatabaseFacade Database { get; }
        
        // Metodo Attach per entity tracking
        EntityEntry<TEntity> Attach<TEntity>(TEntity entity) where TEntity : class;
        
        // Metodo Set per query raw SQL
        DbSet<TEntity> Set<TEntity>() where TEntity : class;
    }
}
