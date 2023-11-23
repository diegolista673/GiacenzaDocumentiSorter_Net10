using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace GiacenzaSorterRm.Models.Database
{
    public partial class GIACENZA_SORTER_RM_TESTContext : DbContext
    {

        public GIACENZA_SORTER_RM_TESTContext(DbContextOptions<GIACENZA_SORTER_RM_TESTContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Bancali> Bancalis { get; set; }
        public virtual DbSet<BancaliDispacci> BancaliDispaccis { get; set; }
        public virtual DbSet<CentriLav> CentriLavs { get; set; }
        public virtual DbSet<CommessaTipologiaContenitore> CommessaTipologiaContenitores { get; set; }
        public virtual DbSet<Commesse> Commesses { get; set; }
        public virtual DbSet<Contenitori> Contenitoris { get; set; }
        public virtual DbSet<Operatori> Operatoris { get; set; }
        public virtual DbSet<Scatole> Scatoles { get; set; }
        public virtual DbSet<SettingsCommesse> SettingsCommesses { get; set; }
        public virtual DbSet<Stati> Statis { get; set; }
        public virtual DbSet<TipiNormalizzazione> TipiNormalizzaziones { get; set; }
        public virtual DbSet<Tipologie> Tipologies { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=SRVR-000EDP02;Database=GIACENZA_SORTER_RM_TEST;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "Latin1_General_CI_AS");

            modelBuilder.Entity<Bancali>(entity =>
            {
                entity.HasKey(e => e.IdBancale)
                    .HasName("PK_Table_1");

                entity.ToTable("Bancali");

                entity.HasIndex(e => e.Bancale, "IX_Table_1")
                    .IsUnique();

                entity.Property(e => e.Bancale)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.DataAccettazioneBancale).HasColumnType("datetime");

                entity.Property(e => e.DataInvioAltroCentro).HasColumnType("datetime");

                entity.Property(e => e.DataPrevistaFineSla).HasColumnType("datetime");

                entity.Property(e => e.DataSorter).HasColumnType("datetime");

                entity.Property(e => e.Note)
                    .HasMaxLength(300)
                    .IsUnicode(false);

                entity.Property(e => e.OperatoreAccettazione)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.OperatoreInvioAltroCentro)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.HasOne(d => d.IdCentroArrivoNavigation)
                    .WithMany(p => p.Bancalis)
                    .HasForeignKey(d => d.IdCentroArrivo)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Bancali_CentriLav");

                entity.HasOne(d => d.IdCommessaNavigation)
                    .WithMany(p => p.Bancalis)
                    .HasForeignKey(d => d.IdCommessa)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Bancali_Commesse");

                entity.HasOne(d => d.IdOperatoreAccettazioneNavigation)
                    .WithMany(p => p.Bancalis)
                    .HasForeignKey(d => d.IdOperatoreAccettazione)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Bancali_Operatori");
            });

            modelBuilder.Entity<BancaliDispacci>(entity =>
            {
                entity.HasKey(e => e.IdAssociazione)
                    .HasName("PK_Table_1_1");

                entity.ToTable("BancaliDispacci");

                entity.Property(e => e.Bancale)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Centro)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.DataAssociazione).HasColumnType("datetime");

                entity.Property(e => e.Dispaccio)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Operatore)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.IdBancaleNavigation)
                    .WithMany(p => p.BancaliDispaccis)
                    .HasForeignKey(d => d.IdBancale)
                    .HasConstraintName("FK_BancaliDispacci_Bancali");
            });

            modelBuilder.Entity<CentriLav>(entity =>
            {
                entity.HasKey(e => e.IdCentroLavorazione);

                entity.ToTable("CentriLav");

                entity.Property(e => e.IdCentroLavorazione).ValueGeneratedNever();

                entity.Property(e => e.CentroLavDesc)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<CommessaTipologiaContenitore>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("CommessaTipologiaContenitore");

                entity.Property(e => e.DescCommessa)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.DescContenitore)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.DescTipologia)
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Commesse>(entity =>
            {
                entity.HasKey(e => e.IdCommessa)
                    .HasName("PK_Commesse2");

                entity.ToTable("Commesse");

                entity.HasIndex(e => e.Commessa, "IX_Commesse")
                    .IsUnique();

                entity.Property(e => e.Commessa)
                    .IsRequired()
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.DataCreazione).HasColumnType("datetime");

                entity.Property(e => e.Note)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.HasOne(d => d.IdOperatoreNavigation)
                    .WithMany(p => p.Commesses)
                    .HasForeignKey(d => d.IdOperatore)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Commesse_Operatori1");
            });

            modelBuilder.Entity<Contenitori>(entity =>
            {
                entity.HasKey(e => e.IdContenitore);

                entity.ToTable("Contenitori");

                entity.Property(e => e.IdContenitore).ValueGeneratedNever();

                entity.Property(e => e.Contenitore)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.DataCreazione).HasColumnType("datetime");

                entity.HasOne(d => d.IdOperatoreCreazioneNavigation)
                    .WithMany(p => p.Contenitoris)
                    .HasForeignKey(d => d.IdOperatoreCreazione)
                    .HasConstraintName("FK_Contenitori_Operatori");
            });

            modelBuilder.Entity<Operatori>(entity =>
            {
                entity.HasKey(e => e.IdOperatore);

                entity.ToTable("Operatori");

                entity.Property(e => e.IdOperatore).ValueGeneratedNever();

                entity.Property(e => e.Azienda)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Operatore)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Password).IsUnicode(false);

                entity.Property(e => e.Ruolo)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.IdCentroLavNavigation)
                    .WithMany(p => p.Operatoris)
                    .HasForeignKey(d => d.IdCentroLav)
                    .HasConstraintName("FK_Operatori_CentriLav");
            });

            modelBuilder.Entity<Scatole>(entity =>
            {
                entity.HasKey(e => e.IdScatola);

                entity.ToTable("Scatole");

                entity.Property(e => e.DataNormalizzazione).HasColumnType("datetime");

                entity.Property(e => e.DataSorter).HasColumnType("datetime");

                entity.Property(e => e.Note).IsUnicode(false);

                entity.Property(e => e.OperatoreNormalizzazione)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.OperatoreSorter)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Scatola)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.HasOne(d => d.IdCentroNormalizzazioneNavigation)
                    .WithMany(p => p.ScatoleIdCentroNormalizzazioneNavigations)
                    .HasForeignKey(d => d.IdCentroNormalizzazione)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Scatole_CentriLav");

                entity.HasOne(d => d.IdCentroSorterizzazioneNavigation)
                    .WithMany(p => p.ScatoleIdCentroSorterizzazioneNavigations)
                    .HasForeignKey(d => d.IdCentroSorterizzazione)
                    .HasConstraintName("FK_Scatole_CentriLav1");

                entity.HasOne(d => d.IdCommessaNavigation)
                    .WithMany(p => p.Scatoles)
                    .HasForeignKey(d => d.IdCommessa)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Scatole_Commesse");

                entity.HasOne(d => d.IdContenitoreNavigation)
                    .WithMany(p => p.Scatoles)
                    .HasForeignKey(d => d.IdContenitore)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Scatole_Contenitori");

                entity.HasOne(d => d.IdStatoNavigation)
                    .WithMany(p => p.Scatoles)
                    .HasForeignKey(d => d.IdStato)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Scatole_Stati");

                entity.HasOne(d => d.IdTipoNormalizzazioneNavigation)
                    .WithMany(p => p.Scatoles)
                    .HasForeignKey(d => d.IdTipoNormalizzazione)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Scatole_TipiNormalizzazione");

                entity.HasOne(d => d.IdTipologiaNavigation)
                    .WithMany(p => p.Scatoles)
                    .HasForeignKey(d => d.IdTipologia)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Scatole_Tipologie");
            });

            modelBuilder.Entity<SettingsCommesse>(entity =>
            {
                entity.HasKey(e => e.IdSettingsCommesse);

                entity.ToTable("SettingsCommesse");

                entity.Property(e => e.Centro)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasOne(d => d.IdCentroNavigation)
                    .WithMany(p => p.SettingsCommesses)
                    .HasForeignKey(d => d.IdCentro)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SettingsCommesse_CentriLav");

                entity.HasOne(d => d.IdCommessaNavigation)
                    .WithMany(p => p.SettingsCommesses)
                    .HasForeignKey(d => d.IdCommessa)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SettingsCommesse_Commesse");
            });

            modelBuilder.Entity<Stati>(entity =>
            {
                entity.HasKey(e => e.IdStato);

                entity.ToTable("Stati");

                entity.Property(e => e.IdStato).ValueGeneratedNever();

                entity.Property(e => e.Stato)
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<TipiNormalizzazione>(entity =>
            {
                entity.HasKey(e => e.IdTipoNormalizzazione);

                entity.ToTable("TipiNormalizzazione");

                entity.Property(e => e.IdTipoNormalizzazione).ValueGeneratedNever();

                entity.Property(e => e.TipoNormalizzazione)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Tipologie>(entity =>
            {
                entity.HasKey(e => e.IdTipologia);

                entity.ToTable("Tipologie");

                entity.Property(e => e.IdTipologia).ValueGeneratedNever();

                entity.Property(e => e.DataCreazione).HasColumnType("datetime");

                entity.Property(e => e.Note).IsUnicode(false);

                entity.Property(e => e.Tipologia)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.HasOne(d => d.IdOperatoreCreazioneNavigation)
                    .WithMany(p => p.Tipologies)
                    .HasForeignKey(d => d.IdOperatoreCreazione)
                    .HasConstraintName("FK_Tipologie_Operatori");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
