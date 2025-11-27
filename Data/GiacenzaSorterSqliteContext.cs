using Microsoft.EntityFrameworkCore;
using GiacenzaSorterRm.Models.Database;
using GiacenzaSorterRm.Models;
using System;
using System.Globalization;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace GiacenzaSorterRm.Data
{
    /// <summary>
    /// DbContext per database SQLite locale
    /// Usa la stessa struttura di GiacenzaSorterRmTestContext ma senza ereditarietà
    /// per evitare problemi con i tipi generici DbContextOptions
    /// </summary>
    public class GiacenzaSorterSqliteContext : DbContext, IAppDbContext
    {
        // Stessi DbSet del context SQL Server
        public virtual DbSet<Bancali> Bancalis { get; set; }
        public virtual DbSet<BancaliDispacci> BancaliDispaccis { get; set; }
        public virtual DbSet<CentriLav> CentriLavs { get; set; }
        public virtual DbSet<CommessaTipologiaContenitore> CommessaTipologiaContenitores { get; set; }
        public virtual DbSet<Commesse> Commesses { get; set; }
        public virtual DbSet<Contenitori> Contenitoris { get; set; }
        public virtual DbSet<Operatori> Operatoris { get; set; }
        public virtual DbSet<Piattaforme> Piattaformes { get; set; }
        public virtual DbSet<Scatole> Scatoles { get; set; }
        public virtual DbSet<Stati> Statis { get; set; }
        public virtual DbSet<TipiNormalizzazione> TipiNormalizzaziones { get; set; }
        public virtual DbSet<Tipologie> Tipologies { get; set; }
        
        // Keyless entities per query raw SQL
        public virtual DbSet<MaceroView> MaceroView { get; set; }
        public virtual DbSet<GiacenzaView> GiacenzaView { get; set; }
        public virtual DbSet<BancaleFuoriSlaView> BancaleFuoriSlaView { get; set; }

        public GiacenzaSorterSqliteContext()
        {
        }

        public GiacenzaSorterSqliteContext(DbContextOptions<GiacenzaSorterSqliteContext> options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite("Data Source=giacenza_sorter_local.db");
            }
        }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            // Configurazione globale per DateTime con supporto formati italiani
            configurationBuilder
                .Properties<DateTime>()
                .HaveConversion<DateTimeToStringConverter>();

            configurationBuilder
                .Properties<DateTime?>()
                .HaveConversion<NullableDateTimeToStringConverter>();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Copia la stessa configurazione del context SQL Server
            // ma adattata per SQLite
            
            modelBuilder.Entity<Bancali>(entity =>
            {
                entity.HasKey(e => e.IdBancale).HasName("PK_Table_1");
                entity.ToTable("Bancali");
                entity.HasIndex(e => e.Bancale, "IX_Table_1").IsUnique();

                entity.Property(e => e.Bancale).IsRequired().HasMaxLength(100);
                entity.Property(e => e.DataAccettazioneBancale).HasColumnType("TEXT");
                entity.Property(e => e.DataInvioAltroCentro).HasColumnType("TEXT");
                entity.Property(e => e.DataSorter).HasColumnType("TEXT");
                entity.Property(e => e.Note).HasMaxLength(300);
                entity.Property(e => e.OperatoreAccettazione).IsRequired().HasMaxLength(100);
                entity.Property(e => e.OperatoreInvioAltroCentro).HasMaxLength(100);

                entity.HasOne(d => d.IdCentroArrivoNavigation).WithMany(p => p.Bancalis)
                    .HasForeignKey(d => d.IdCentroArrivo)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Bancali_CentriLav");

                entity.HasOne(d => d.IdCommessaNavigation).WithMany(p => p.Bancalis)
                    .HasForeignKey(d => d.IdCommessa)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Bancali_Commesse");

                entity.HasOne(d => d.IdOperatoreAccettazioneNavigation).WithMany(p => p.Bancalis)
                    .HasForeignKey(d => d.IdOperatoreAccettazione)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Bancali_Operatori");
            });

            modelBuilder.Entity<BancaliDispacci>(entity =>
            {
                entity.HasKey(e => e.IdAssociazione).HasName("PK_Table_1_1");
                entity.ToTable("BancaliDispacci");

                entity.Property(e => e.Bancale).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Centro).IsRequired().HasMaxLength(50);
                entity.Property(e => e.DataAssociazione).HasColumnType("TEXT");
                entity.Property(e => e.Dispaccio).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Operatore).IsRequired().HasMaxLength(50);

                entity.HasOne(d => d.IdBancaleNavigation).WithMany(p => p.BancaliDispaccis)
                    .HasForeignKey(d => d.IdBancale)
                    .HasConstraintName("FK_BancaliDispacci_Bancali");
            });

            modelBuilder.Entity<CentriLav>(entity =>
            {
                entity.HasKey(e => e.IdCentroLavorazione);
                entity.ToTable("CentriLav");
                entity.Property(e => e.CentroLavDesc).HasMaxLength(50);
                entity.Property(e => e.Sigla).HasMaxLength(2);
            });

            modelBuilder.Entity<CommessaTipologiaContenitore>(entity =>
            {
                entity.HasKey(e => e.IdRiepilogo).HasName("PK_Riepilogo");
                entity.ToTable("CommessaTipologiaContenitore");
                entity.HasIndex(e => new { e.IdCommessa, e.IdTipologia, e.IdContenitore }, "IX_CommessaTipologiaContenitore").IsUnique();

                entity.Property(e => e.DescCommessa).HasMaxLength(100);
                entity.Property(e => e.DescContenitore).HasMaxLength(100);
                entity.Property(e => e.DescTipologia).HasMaxLength(100);

                entity.HasOne(d => d.IdCommessaNavigation).WithMany(p => p.CommessaTipologiaContenitores)
                    .HasForeignKey(d => d.IdCommessa)
                    .HasConstraintName("FK_Riepilogo_Commesse");

                entity.HasOne(d => d.IdContenitoreNavigation).WithMany(p => p.CommessaTipologiaContenitores)
                    .HasForeignKey(d => d.IdContenitore)
                    .HasConstraintName("FK_Riepilogo_Contenitori");

                entity.HasOne(d => d.IdTipologiaNavigation).WithMany(p => p.CommessaTipologiaContenitores)
                    .HasForeignKey(d => d.IdTipologia)
                    .HasConstraintName("FK_Riepilogo_Tipologie");
            });

            modelBuilder.Entity<Commesse>(entity =>
            {
                entity.HasKey(e => e.IdCommessa);
                entity.ToTable("Commesse");
                entity.HasIndex(e => e.Commessa, "IX_Commesse").IsUnique();

                entity.Property(e => e.Commessa).HasMaxLength(200);
                entity.Property(e => e.DataCreazione).HasColumnType("TEXT");
                entity.Property(e => e.Note).HasMaxLength(200);

                entity.HasOne(d => d.IdOperatoreNavigation).WithMany(p => p.Commesses)
                    .HasForeignKey(d => d.IdOperatore)
                    .HasConstraintName("FK_Commesse_Operatori");

                entity.HasOne(d => d.IdPiattaformaNavigation).WithMany(p => p.Commesses)
                    .HasForeignKey(d => d.IdPiattaforma)
                    .HasConstraintName("FK_Commesse_Piattaforme");
            });

            modelBuilder.Entity<Contenitori>(entity =>
            {
                entity.HasKey(e => e.IdContenitore);
                entity.ToTable("Contenitori");
                entity.HasIndex(e => e.Contenitore, "IX_Contenitori").IsUnique();

                entity.Property(e => e.Contenitore).HasMaxLength(100);
                entity.Property(e => e.DataCreazione).HasColumnType("TEXT");

                entity.HasOne(d => d.IdOperatoreCreazioneNavigation).WithMany(p => p.Contenitoris)
                    .HasForeignKey(d => d.IdOperatoreCreazione)
                    .HasConstraintName("FK_Contenitori_Operatori");
            });

            modelBuilder.Entity<Operatori>(entity =>
            {
                entity.HasKey(e => e.IdOperatore);
                entity.ToTable("Operatori");

                entity.Property(e => e.Azienda).HasMaxLength(50);
                entity.Property(e => e.Operatore).HasMaxLength(50);
                entity.Property(e => e.Password);
                entity.Property(e => e.Ruolo).HasMaxLength(50);

                entity.HasOne(d => d.IdCentroLavNavigation).WithMany(p => p.Operatoris)
                    .HasForeignKey(d => d.IdCentroLav)
                    .HasConstraintName("FK_Operatori_CentriLav");
            });

            modelBuilder.Entity<Piattaforme>(entity =>
            {
                entity.HasKey(e => e.IdPiattaforma);
                entity.ToTable("Piattaforme");

                entity.Property(e => e.DataCreazione).HasColumnType("TEXT");
                entity.Property(e => e.Note);
                entity.Property(e => e.Piattaforma).HasMaxLength(50);
            });

            modelBuilder.Entity<Scatole>(entity =>
            {
                entity.HasKey(e => e.IdScatola);
                entity.ToTable("Scatole");
                entity.HasIndex(e => e.Scatola, "IX_Scatole").IsUnique();

                entity.Property(e => e.DataCambioGiacenza).HasColumnType("TEXT");
                entity.Property(e => e.DataMacero).HasColumnType("TEXT");
                entity.Property(e => e.DataNormalizzazione).HasColumnType("TEXT");
                entity.Property(e => e.DataSorter).HasColumnType("TEXT");
                entity.Property(e => e.Note);
                entity.Property(e => e.NoteCambioGiacenza).HasMaxLength(300);
                entity.Property(e => e.OperatoreCambioGiacenza).HasMaxLength(100);
                entity.Property(e => e.OperatoreMacero).HasMaxLength(100);
                entity.Property(e => e.OperatoreNormalizzazione).HasMaxLength(100);
                entity.Property(e => e.OperatoreSorter).HasMaxLength(100);
                entity.Property(e => e.Scatola).IsRequired().HasMaxLength(100);


                entity.HasOne(d => d.IdCentroGiacenzaNavigation).WithMany(p => p.ScatoleIdCentroGiacenzaNavigations)
                    .HasForeignKey(d => d.IdCentroGiacenza)
                    .HasConstraintName("FK_Scatole_CentriLav2");

                entity.HasOne(d => d.IdCentroNormalizzazioneNavigation).WithMany(p => p.ScatoleIdCentroNormalizzazioneNavigations)
                    .HasForeignKey(d => d.IdCentroNormalizzazione)
                    .HasConstraintName("FK_Scatole_CentriLav");

                entity.HasOne(d => d.IdCentroSorterizzazioneNavigation).WithMany(p => p.ScatoleIdCentroSorterizzazioneNavigations)
                    .HasForeignKey(d => d.IdCentroSorterizzazione)
                    .HasConstraintName("FK_Scatole_CentriLav1");

                entity.HasOne(d => d.IdCommessaNavigation).WithMany(p => p.Scatoles)
                    .HasForeignKey(d => d.IdCommessa)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Scatole_Commesse");

                entity.HasOne(d => d.IdContenitoreNavigation).WithMany(p => p.Scatoles)
                    .HasForeignKey(d => d.IdContenitore)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Scatole_Contenitori");

                entity.HasOne(d => d.IdStatoNavigation).WithMany(p => p.Scatoles)
                    .HasForeignKey(d => d.IdStato)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Scatole_Stati");

                entity.HasOne(d => d.IdTipoNormalizzazioneNavigation).WithMany(p => p.Scatoles)
                    .HasForeignKey(d => d.IdTipoNormalizzazione)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Scatole_TipiNormalizzazione");

                entity.HasOne(d => d.IdTipologiaNavigation).WithMany(p => p.Scatoles)
                    .HasForeignKey(d => d.IdTipologia)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Scatole_Tipologie");
            });

            modelBuilder.Entity<Stati>(entity =>
            {
                entity.HasKey(e => e.IdStato);
                entity.ToTable("Stati");
                entity.Property(e => e.Stato).HasMaxLength(100);
            });

            modelBuilder.Entity<TipiNormalizzazione>(entity =>
            {
                entity.HasKey(e => e.IdTipoNormalizzazione).HasName("PK_TipiNormalizzazzione");
                entity.ToTable("TipiNormalizzazione");
                entity.Property(e => e.TipoNormalizzazione).IsRequired().HasMaxLength(100);
            });

            modelBuilder.Entity<Tipologie>(entity =>
            {
                entity.HasKey(e => e.IdTipologia);
                entity.ToTable("Tipologie");
                entity.HasIndex(e => e.Tipologia, "IX_Tipologie").IsUnique();

                entity.Property(e => e.DataCreazione).HasColumnType("TEXT");
                entity.Property(e => e.Note);
                entity.Property(e => e.Tipologia).HasMaxLength(100);

                entity.HasOne(d => d.IdOperatoreCreazioneNavigation).WithMany(p => p.Tipologies)
                    .HasForeignKey(d => d.IdOperatoreCreazione)
                    .HasConstraintName("FK_Tipologie_Operatori");
            });
            
            // Keyless entities per query raw SQL
            modelBuilder.Entity<MaceroView>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<GiacenzaView>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<BancaleFuoriSlaView>(entity =>
            {
                entity.HasNoKey();
            });
        }
    }

    /// <summary>
    /// Converter per DateTime che supporta formati multipli inclusi quelli italiani
    /// </summary>
    public class DateTimeToStringConverter : ValueConverter<DateTime, string>
    {
        private static readonly string[] DateFormats = new[]
        {
            "yyyy-MM-dd HH:mm:ss",           // ISO 8601 (SQLite default)
            "yyyy-MM-dd HH:mm:ss.fff",       // ISO 8601 con millisecondi
            "dd/MM/yyyy HH:mm:ss",           // Formato italiano
            "dd/MM/yyyy",                     // Formato italiano corto
            "yyyy-MM-dd",                     // ISO corto
        };

        public DateTimeToStringConverter()
            : base(
                v => v.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
                v => ParseDateTime(v))
        {
        }

        private static DateTime ParseDateTime(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return DateTime.MinValue;

            // Prova prima il parsing con cultura italiana
            if (DateTime.TryParse(value, new CultureInfo("it-IT"), DateTimeStyles.None, out var dateItalian))
                return dateItalian;

            // Poi prova con invariant culture
            if (DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dateInvariant))
                return dateInvariant;

            // Infine prova con formati espliciti
            if (DateTime.TryParseExact(value, DateFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dateExact))
                return dateExact;

            throw new FormatException($"Unable to parse DateTime value: '{value}'");
        }
    }

    /// <summary>
    /// Converter per DateTime nullable che supporta formati multipli inclusi quelli italiani
    /// </summary>
    public class NullableDateTimeToStringConverter : ValueConverter<DateTime?, string>
    {
        private static readonly string[] DateFormats = new[]
        {
            "yyyy-MM-dd HH:mm:ss",
            "yyyy-MM-dd HH:mm:ss.fff",
            "dd/MM/yyyy HH:mm:ss",
            "dd/MM/yyyy",
            "yyyy-MM-dd",
        };

        public NullableDateTimeToStringConverter()
            : base(
                v => v.HasValue ? v.Value.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture) : null,
                v => ParseNullableDateTime(v))
        {
        }

        private static DateTime? ParseNullableDateTime(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            if (DateTime.TryParse(value, new CultureInfo("it-IT"), DateTimeStyles.None, out var dateItalian))
                return dateItalian;

            if (DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dateInvariant))
                return dateInvariant;

            if (DateTime.TryParseExact(value, DateFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dateExact))
                return dateExact;

            throw new FormatException($"Unable to parse DateTime value: '{value}'");
        }
    }
}
