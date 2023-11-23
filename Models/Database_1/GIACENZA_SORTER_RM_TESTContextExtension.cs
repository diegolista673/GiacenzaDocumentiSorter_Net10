using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace GiacenzaSorterRm.Models.Database
{
    public partial class GIACENZA_SORTER_RM_TESTContext
    {


        public virtual DbSet<BancaleFuoriSlaView> BancaleFuoriSlaView { get; set; }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BancaleFuoriSlaView>(entity =>
            {
                entity.HasNoKey();
            });

        }


    }
}
