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
        public virtual DbSet<HistoryPrice> HistoryPrice { get; set; }
        public virtual DbSet<Infomations> Infomations { get; set; }
        public virtual DbSet<MonthData> MonthData { get; set; }
        public virtual DbSet<Prices> Prices { get; set; }
        public virtual DbSet<SeasonData> SeasonData { get; set; }
        public virtual DbSet<StockHistory> StockHistory { get; set; }
        public virtual DbSet<Stocks> Stocks { get; set; }
        public virtual DbSet<Thousand> Thousand { get; set; }
        public virtual DbSet<YearData> YearData { get; set; }
        public virtual DbSet<_WeekyChip> _WeekyChip { get; set; }

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

            modelBuilder.Entity<HistoryPrice>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Close).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.Property(e => e.Datetime).HasColumnType("datetime");

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

                entity.Property(e => e.三大法人持股比重).HasColumnType("numeric(18, 3)");

                entity.Property(e => e.主力買超張數).HasColumnType("numeric(18, 3)");

                entity.Property(e => e.主力賣超張數).HasColumnType("numeric(18, 3)");

                entity.Property(e => e.二十日主力買超張數).HasColumnType("numeric(18, 3)");

                entity.Property(e => e.二十日主力賣超張數).HasColumnType("numeric(18, 3)");

                entity.Property(e => e.五日主力買超張數).HasColumnType("numeric(18, 3)");

                entity.Property(e => e.五日主力賣超張數).HasColumnType("numeric(18, 3)");

                entity.Property(e => e.六十日主力買超張數).HasColumnType("numeric(18, 3)");

                entity.Property(e => e.六十日主力賣超張數).HasColumnType("numeric(18, 3)");

                entity.Property(e => e.券資比).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.十日主力買超張數).HasColumnType("numeric(18, 3)");

                entity.Property(e => e.十日主力賣超張數).HasColumnType("numeric(18, 3)");

                entity.Property(e => e.四十日主力買超張數).HasColumnType("numeric(18, 3)");

                entity.Property(e => e.四十日主力賣超張數).HasColumnType("numeric(18, 3)");

                entity.Property(e => e.外資持股比重).HasColumnType("numeric(18, 3)");

                entity.Property(e => e.本益比).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.漲跌).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.漲跌百分比).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.融資使用率).HasColumnType("numeric(18, 2)");
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

            modelBuilder.Entity<MonthData>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.Property(e => e.Datetime).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.StockId)
                    .IsRequired()
                    .HasMaxLength(8)
                    .IsUnicode(false);

                entity.Property(e => e.去年同月營收).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.去年累計營收).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.單月年增率).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.單月月增率).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.單月營收).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.累積年增率).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.累計營收).HasColumnType("decimal(18, 2)");
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

            modelBuilder.Entity<SeasonData>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.Property(e => e.Datetime).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.ROA).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.ROE).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.StockId)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsFixedLength();

                entity.Property(e => e.公告每股淨值).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.每股營業額).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.每股稅後盈餘).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.毛利率).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.營業利益率).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.股本).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.股東權益).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.負債總計).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.資產總計).HasColumnType("decimal(18, 2)");
            });

            modelBuilder.Entity<StockHistory>(entity =>
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

                entity.Property(e => e.每股淨值).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.每股盈餘).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.營收比重).HasMaxLength(1024);

                entity.Property(e => e.股價).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.股本).HasColumnType("decimal(18, 2)");
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

                entity.Property(e => e.每股淨值).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.每股盈餘).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.營收比重).HasMaxLength(1024);

                entity.Property(e => e.股價).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.股本).HasColumnType("decimal(18, 2)");
            });

            modelBuilder.Entity<Thousand>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.Property(e => e.Datetime).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.P1).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.P10).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.P100).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.P1000).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.P15).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.P20).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.P200).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.P30).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.P40).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.P400).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.P5).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.P50).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.P600).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.P800).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.POver1000).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.PUnder100).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.S1).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.S10).HasColumnType("decimal(18, 3)");

                entity.Property(e => e.S100).HasColumnType("decimal(18, 3)");

                entity.Property(e => e.S1000).HasColumnType("decimal(18, 3)");

                entity.Property(e => e.S15).HasColumnType("decimal(18, 3)");

                entity.Property(e => e.S20).HasColumnType("decimal(18, 3)");

                entity.Property(e => e.S200).HasColumnType("decimal(18, 3)");

                entity.Property(e => e.S30).HasColumnType("decimal(18, 3)");

                entity.Property(e => e.S40).HasColumnType("decimal(18, 3)");

                entity.Property(e => e.S400).HasColumnType("decimal(18, 3)");

                entity.Property(e => e.S5).HasColumnType("decimal(18, 3)");

                entity.Property(e => e.S50).HasColumnType("decimal(18, 3)");

                entity.Property(e => e.S600).HasColumnType("decimal(18, 3)");

                entity.Property(e => e.S800).HasColumnType("decimal(18, 3)");

                entity.Property(e => e.SOver1000).HasColumnType("decimal(18, 3)");

                entity.Property(e => e.SUnder100).HasColumnType("decimal(18, 3)");

                entity.Property(e => e.StockId)
                    .IsRequired()
                    .HasMaxLength(8)
                    .IsUnicode(false);

                entity.Property(e => e.TotalStock).HasColumnType("decimal(18, 3)");
            });

            modelBuilder.Entity<YearData>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.StockId)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsFixedLength();

                entity.Property(e => e.公告每股淨值).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.每股稅後盈餘).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.毛利率).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.流動負債).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.流動資產).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.營業毛利).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.稅前純益).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.稅後純益).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.稅後股東權益報酬率).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.稅後資產報酬率).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.股東權益).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.負債總計).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.資產總計).HasColumnType("decimal(18, 2)");
            });

            modelBuilder.Entity<_WeekyChip>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Close).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.Datetime)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.POver1000).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.POver400).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.PUnder100).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.PUnder400).HasColumnType("decimal(18, 2)");

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
