using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GiacenzaSorterRm.AppCode;
using GiacenzaSorterRm.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace GiacenzaSorterRm.Models.Database
{
    public partial class GiacenzaSorterContext
    {

        public virtual DbSet<BancaleFuoriSlaView> BancaleFuoriSlaView { get; set; }
        public virtual DbSet<GiacenzaView> GiacenzaView { get; set; }
        public virtual DbSet<MaceroView> MaceroView { get; set; }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BancaleFuoriSlaView>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<GiacenzaView>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<MaceroView>(entity =>
            {
                entity.HasNoKey();
            });

        }


    }
}
