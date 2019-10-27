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

        public virtual DbSet<AnaCMoney> AnaCMoney { get; set; }
        public virtual DbSet<AnaFutureEngine> AnaFutureEngine { get; set; }
        public virtual DbSet<AnaStatementDogs> AnaStatementDogs { get; set; }
        public virtual DbSet<BestStocks> BestStocks { get; set; }
        public virtual DbSet<Chip> Chip { get; set; }
        public virtual DbSet<Infomations> Infomations { get; set; }
        public virtual DbSet<Prices> Prices { get; set; }
        public virtual DbSet<Seasons> Seasons { get; set; }
        public virtual DbSet<Stocks> Stocks { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Server=220.133.185.1;Database=StockDb;User ID=stock;Password=stock;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AnaCMoney>(entity =>
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

            modelBuilder.Entity<AnaFutureEngine>(entity =>
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

            modelBuilder.Entity<AnaStatementDogs>(entity =>
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

            modelBuilder.Entity<BestStocks>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.Property(e => e.Description).HasMaxLength(50);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.StockId)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsFixedLength();

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<Chip>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.Property(e => e.EndDate).HasColumnType("datetime");

                entity.Property(e => e.Name).HasMaxLength(50);

                entity.Property(e => e.StartDate).HasColumnType("datetime");

                entity.Property(e => e.StockId)
                    .HasMaxLength(8)
                    .IsUnicode(false);

                entity.Property(e => e.主力買進).HasColumnType("numeric(18, 3)");

                entity.Property(e => e.主力賣出).HasColumnType("numeric(18, 3)");
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
                entity.HasIndex(e => e.StockId)
                    .HasName("IX_Prices");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Close).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.Property(e => e.Datetime).HasColumnType("datetime");

                entity.Property(e => e.High).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.Low).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.MA10).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.MA120).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.MA20).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.MA240).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.MA3).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.MA5).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.MA60).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Open).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.StockId)
                    .IsRequired()
                    .HasMaxLength(8)
                    .IsUnicode(false);

                entity.Property(e => e.VMA10).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.VMA120).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.VMA20).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.VMA240).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.VMA3).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.VMA5).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.VMA60).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.主力買超張數).HasColumnType("numeric(18, 3)");

                entity.Property(e => e.主力賣超張數).HasColumnType("numeric(18, 3)");

                entity.Property(e => e.二十日主力買超張數).HasColumnType("numeric(18, 3)");

                entity.Property(e => e.二十日主力賣超張數).HasColumnType("numeric(18, 3)");

                entity.Property(e => e.五日主力買超張數).HasColumnType("numeric(18, 3)");

                entity.Property(e => e.五日主力賣超張數).HasColumnType("numeric(18, 3)");

                entity.Property(e => e.六十日主力買超張數).HasColumnType("numeric(18, 3)");

                entity.Property(e => e.六十日主力賣超張數).HasColumnType("numeric(18, 3)");

                entity.Property(e => e.十日主力買超張數).HasColumnType("numeric(18, 3)");

                entity.Property(e => e.十日主力賣超張數).HasColumnType("numeric(18, 3)");

                entity.Property(e => e.四十日主力買超張數).HasColumnType("numeric(18, 3)");

                entity.Property(e => e.四十日主力賣超張數).HasColumnType("numeric(18, 3)");

                entity.Property(e => e.外資持股比例).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.本益比).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.漲跌).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.漲跌百分比).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.融資使用率).HasColumnType("numeric(18, 2)");
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

                entity.Property(e => e.Address).HasMaxLength(256);

                entity.Property(e => e.Description).HasMaxLength(256);

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

                entity.Property(e => e.Website).HasMaxLength(256);

                entity.Property(e => e.營收比重).HasMaxLength(1024);

                entity.Property(e => e.股本).HasColumnType("decimal(18, 2)");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
