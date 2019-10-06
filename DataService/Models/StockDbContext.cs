using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace DataService.Models
{
    public partial class StockDbContext : DbContext
    {
        public StockDbContext()
        {
        }

        public StockDbContext(DbContextOptions<StockDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Checks> Checks { get; set; }
        public virtual DbSet<Infomations> Infomations { get; set; }
        public virtual DbSet<Prices> Prices { get; set; }
        public virtual DbSet<Remarks> Remarks { get; set; }
        public virtual DbSet<Seasons> Seasons { get; set; }
        public virtual DbSet<Stocks> Stocks { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Server=localhost;Database=StockDb;User ID=sa;Password=sa;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Checks>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(512);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.StockId)
                    .IsRequired()
                    .HasMaxLength(8)
                    .IsUnicode(false);

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasMaxLength(512);
            });

            modelBuilder.Entity<Infomations>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.Property(e => e.EarningsPerShare).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.PriceBookRatio).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.PriceEarningRatio).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.StockId)
                    .IsRequired()
                    .HasMaxLength(8)
                    .IsUnicode(false);

                entity.Property(e => e.Time)
                    .HasMaxLength(8)
                    .IsUnicode(false);

                entity.Property(e => e.UpdatedOn).HasColumnType("datetime");
            });

            modelBuilder.Entity<Prices>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.Close).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.Property(e => e.High).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.Low).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Open).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.StockId)
                    .IsRequired()
                    .HasMaxLength(8)
                    .IsUnicode(false);

                entity.Property(e => e.UpdatedOn).HasColumnType("datetime");

                entity.Property(e => e.外資持股比例).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.融資使用率).HasColumnType("numeric(18, 2)");
            });

            modelBuilder.Entity<Remarks>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Remark)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.StockId)
                    .IsRequired()
                    .HasMaxLength(8)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Seasons>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.每股盈餘).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.負債比率)
                    .HasMaxLength(10)
                    .IsFixedLength();
            });

            modelBuilder.Entity<Stocks>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Industry)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.MarketCategory)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.StockId)
                    .IsRequired()
                    .HasMaxLength(8)
                    .IsUnicode(false);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
