using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CoreRestPermutations.DAL
{
    public class OriginalValues
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Value { get; set; }
        public double ElapsedSeconds { get; set; }

        public virtual ICollection<Combinations> Combinations { get; set; }
    }

    public class Combinations
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Value { get; set; }

        [ForeignKey("OriginalValues")]
        public int OriginalValueId { get; set; }
        public virtual OriginalValues OriginalValues { get; set; }
    }
    public class PermutationsContext : DbContext
    {
        public DbSet<OriginalValues> OriginalValues { get; set; }
        public DbSet<Combinations> Combinations { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseNpgsql("Host=postgres64.1gb.ru;Port=5432;Database=xgb_temporary;Username=xgb_temporary;Password=5d3b92afwr;EntityAdminDatabase=xgb_temporary");
        }
    }
}
