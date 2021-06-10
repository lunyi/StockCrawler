using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace PostgresData.Models
{
    public partial class stockContext : DbContext
    {
        public stockContext()
        {
        }

        public stockContext(DbContextOptions<stockContext> options)
            : base(options)
        {
        }

        public virtual DbSet<AnaCMoney> AnaCMoneys { get; set; }
        public virtual DbSet<AnaFutureEngine> AnaFutureEngines { get; set; }
        public virtual DbSet<AnaStatementDog> AnaStatementDogs { get; set; }
        public virtual DbSet<BestStock> BestStocks { get; set; }
        public virtual DbSet<BestStockType> BestStockTypes { get; set; }
        public virtual DbSet<Broker> Brokers { get; set; }
        public virtual DbSet<BrokerTransaction> BrokerTransactions { get; set; }
        public virtual DbSet<BrokerTransactionDetail> BrokerTransactionDetails { get; set; }
        public virtual DbSet<Chip> Chips { get; set; }
        public virtual DbSet<HistoryPrice> HistoryPrices { get; set; }
        public virtual DbSet<MinuteKLine> MinuteKLines { get; set; }
        public virtual DbSet<MonthDatum> MonthData { get; set; }
        public virtual DbSet<Price> Prices { get; set; }
        public virtual DbSet<PricesTemp2d5c4a95> PricesTemp2d5c4a95s { get; set; }
        public virtual DbSet<PricesTemp6919f38f> PricesTemp6919f38fs { get; set; }
        public virtual DbSet<RealtimeBestStock> RealtimeBestStocks { get; set; }
        public virtual DbSet<SeasonDatum> SeasonData { get; set; }
        public virtual DbSet<Stock> Stocks { get; set; }
        public virtual DbSet<StockBroker> StockBrokers { get; set; }
        public virtual DbSet<StockHistory> StockHistories { get; set; }
        public virtual DbSet<Thousand> Thousands { get; set; }
        public virtual DbSet<Token> Tokens { get; set; }
        public virtual DbSet<TwStock> TwStocks { get; set; }
        public virtual DbSet<YearDatum> YearData { get; set; }
        public virtual DbSet<_Industry> _Industries { get; set; }
        public virtual DbSet<_MinuteKLine> _MinuteKLines { get; set; }
        public virtual DbSet<_MonthDatum> _MonthData { get; set; }
        public virtual DbSet<_Price> _Prices { get; set; }
        public virtual DbSet<_WeekyChip> _WeekyChips { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseNpgsql("Server=localhost;Port=5433;Database=stock;User ID=postgres;Password=1q2w3e4r;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "Chinese (Traditional)_Taiwan.950");

            modelBuilder.Entity<AnaCMoney>(entity =>
            {
                entity.ToTable("AnaCMoney");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Remark)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.StockId)
                    .IsRequired()
                    .HasMaxLength(8);
            });

            modelBuilder.Entity<AnaFutureEngine>(entity =>
            {
                entity.ToTable("AnaFutureEngine");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(512);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.StockId)
                    .IsRequired()
                    .HasMaxLength(8);

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasMaxLength(512);
            });

            modelBuilder.Entity<AnaStatementDog>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(512);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.StockId)
                    .IsRequired()
                    .HasMaxLength(8);

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasMaxLength(512);
            });

            modelBuilder.Entity<BestStock>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Description).HasMaxLength(50);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.StockId)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsFixedLength(true);

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<BestStockType>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("BestStockType");

                entity.Property(e => e.Value)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<Broker>(entity =>
            {
                entity.ToTable("Broker");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Address).HasMaxLength(1024);

                entity.Property(e => e.BHID).HasMaxLength(32);

                entity.Property(e => e.BrokerName)
                    .IsRequired()
                    .HasMaxLength(64);

                entity.Property(e => e.MainName).HasMaxLength(64);

                entity.Property(e => e.Tel).HasMaxLength(64);

                entity.Property(e => e.b).HasMaxLength(32);
            });

            modelBuilder.Entity<BrokerTransaction>(entity =>
            {
                entity.ToTable("BrokerTransaction");

                entity.HasIndex(e => new { e.BrokerName, e.StockId, e.Datetime }, "BrokerTransaction_BrokerName_StockId_Datetime_key")
                    .IsUnique();

                entity.HasIndex(e => e.StockId, "BrokerTransaction_IX_BrokerTransaction");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.BrokerName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(12);

                entity.Property(e => e.StockId)
                    .IsRequired()
                    .HasMaxLength(10);
            });

            modelBuilder.Entity<BrokerTransactionDetail>(entity =>
            {
                entity.HasKey(e => new { e.BrokerId, e.StockId, e.Datetime })
                    .HasName("BrokerTransactionDetails_pkey");

                entity.HasIndex(e => e.StockId, "Index_BrokerTransactionDetails_StockId");

                entity.Property(e => e.BrokerId).HasMaxLength(16);

                entity.Property(e => e.StockId).HasMaxLength(10);

                entity.Property(e => e.BrokerName)
                    .IsRequired()
                    .HasMaxLength(64);

                entity.Property(e => e.StockName)
                    .IsRequired()
                    .HasMaxLength(12);
            });

            modelBuilder.Entity<Chip>(entity =>
            {
                entity.ToTable("Chip");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Name).HasMaxLength(50);

                entity.Property(e => e.StockId).HasMaxLength(8);

                entity.Property(e => e.主力買進).HasPrecision(18, 3);

                entity.Property(e => e.主力賣出).HasPrecision(18, 3);
            });

            modelBuilder.Entity<HistoryPrice>(entity =>
            {
                entity.ToTable("HistoryPrice");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Close).HasPrecision(18, 2);

                entity.Property(e => e.High).HasPrecision(18, 2);

                entity.Property(e => e.Low).HasPrecision(18, 2);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Open).HasPrecision(18, 2);

                entity.Property(e => e.StockId)
                    .IsRequired()
                    .HasMaxLength(8);

                entity.Property(e => e.三大法人持股比重).HasPrecision(18, 3);

                entity.Property(e => e.主力買超張數).HasPrecision(18, 3);

                entity.Property(e => e.主力賣超張數).HasPrecision(18, 3);

                entity.Property(e => e.二十日主力買超張數).HasPrecision(18, 3);

                entity.Property(e => e.二十日主力賣超張數).HasPrecision(18, 3);

                entity.Property(e => e.五日主力買超張數).HasPrecision(18, 3);

                entity.Property(e => e.五日主力賣超張數).HasPrecision(18, 3);

                entity.Property(e => e.六十日主力買超張數).HasPrecision(18, 3);

                entity.Property(e => e.六十日主力賣超張數).HasPrecision(18, 3);

                entity.Property(e => e.券資比).HasPrecision(18, 2);

                entity.Property(e => e.十日主力買超張數).HasPrecision(18, 3);

                entity.Property(e => e.十日主力賣超張數).HasPrecision(18, 3);

                entity.Property(e => e.四十日主力買超張數).HasPrecision(18, 3);

                entity.Property(e => e.四十日主力賣超張數).HasPrecision(18, 3);

                entity.Property(e => e.外資持股比重).HasPrecision(18, 3);

                entity.Property(e => e.本益比).HasPrecision(18, 2);

                entity.Property(e => e.漲跌).HasPrecision(18, 2);

                entity.Property(e => e.漲跌百分比).HasPrecision(18, 2);

                entity.Property(e => e.融資使用率).HasPrecision(18, 2);
            });

            modelBuilder.Entity<MinuteKLine>(entity =>
            {
                entity.HasKey(e => new { e.StockId, e.Datetime })
                    .HasName("MinuteKLine_pkey");

                entity.ToTable("MinuteKLine");

                entity.HasIndex(e => e.Datetime, "Index_MinuteKLine_Datetime");

                entity.Property(e => e.StockId).HasMaxLength(8);

                entity.Property(e => e.Close).HasPrecision(18, 2);

                entity.Property(e => e.High).HasPrecision(18, 2);

                entity.Property(e => e.Low).HasPrecision(18, 2);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Open).HasPrecision(18, 2);
            });

            modelBuilder.Entity<MonthDatum>(entity =>
            {
                entity.HasKey(e => new { e.StockId, e.Datetime })
                    .HasName("MonthData_pkey");

                entity.HasIndex(e => e.Datetime, "Index_MonthData_Datetime");

                entity.HasIndex(e => e.Datetime, "MonthData_StockId_Name");

                entity.Property(e => e.StockId).HasMaxLength(8);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<Price>(entity =>
            {
                entity.HasKey(e => new { e.StockId, e.Datetime })
                    .HasName("Prices_pkey");

                entity.HasIndex(e => e.Datetime, "Index_Prices_Datetime");

                entity.HasIndex(e => e.Datetime, "Index_Prices_Datetime2");

                entity.HasIndex(e => new { e.Datetime, e.漲跌百分比, e.成交量 }, "Index_Prices_Datetime_成交量");

                entity.HasIndex(e => e.Datetime, "Prices_10days_Index");

                entity.HasIndex(e => e.Datetime, "Prices_20days_Index");

                entity.HasIndex(e => e.Datetime, "Prices_5days_Index");

                entity.HasIndex(e => e.Datetime, "Prices_60days_Index");

                entity.HasIndex(e => new { e.Datetime, e.外資買賣超, e.投信買賣超 }, "Prices_ITrust_Index");

                entity.HasIndex(e => e.StockId, "Prices_IX_Prices");

                entity.HasIndex(e => e.StockId, "Prices_Index_Datetime");

                entity.HasIndex(e => new { e.Datetime, e.StockId }, "Prices_Index_For_Update_Close");

                entity.HasIndex(e => new { e.Datetime, e.成交量, e.投信買賣超 }, "Prices_Index_[投信買賣超");

                entity.HasIndex(e => new { e.Datetime, e.外資買賣超 }, "Prices_MainForce_Index");

                entity.Property(e => e.StockId).HasMaxLength(8);

                entity.Property(e => e.Close).HasPrecision(18, 2);

                entity.Property(e => e.D)
                    .HasPrecision(18, 2)
                    .HasDefaultValueSql("'0'::numeric");

                entity.Property(e => e.D1)
                    .HasMaxLength(8)
                    .HasDefaultValueSql("'0'::character varying");

                entity.Property(e => e.DIF)
                    .HasPrecision(18, 2)
                    .HasDefaultValueSql("'0'::numeric");

                entity.Property(e => e.DIF1)
                    .HasMaxLength(8)
                    .HasDefaultValueSql("'0'::character varying");

                entity.Property(e => e.High).HasPrecision(18, 2);

                entity.Property(e => e.K)
                    .HasPrecision(18, 2)
                    .HasDefaultValueSql("'0'::numeric");

                entity.Property(e => e.K1)
                    .HasMaxLength(8)
                    .HasDefaultValueSql("'0'::character varying");

                entity.Property(e => e.Low).HasPrecision(18, 2);

                entity.Property(e => e.MA10).HasPrecision(18, 2);

                entity.Property(e => e.MA10_).HasMaxLength(8);

                entity.Property(e => e.MA120).HasPrecision(18, 2);

                entity.Property(e => e.MA20).HasPrecision(18, 2);

                entity.Property(e => e.MA20_).HasMaxLength(8);

                entity.Property(e => e.MA240).HasPrecision(18, 2);

                entity.Property(e => e.MA3).HasPrecision(18, 2);

                entity.Property(e => e.MA5).HasPrecision(18, 2);

                entity.Property(e => e.MA5_).HasMaxLength(8);

                entity.Property(e => e.MA60).HasPrecision(18, 2);

                entity.Property(e => e.MA60_).HasMaxLength(8);

                entity.Property(e => e.MACD)
                    .HasPrecision(18, 2)
                    .HasDefaultValueSql("'0'::numeric");

                entity.Property(e => e.MACD1)
                    .HasMaxLength(8)
                    .HasDefaultValueSql("'0'::character varying");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.OSC)
                    .HasPrecision(18, 2)
                    .HasDefaultValueSql("'0'::numeric");

                entity.Property(e => e.OSC1)
                    .HasMaxLength(8)
                    .HasDefaultValueSql("'0'::character varying");

                entity.Property(e => e.Open).HasPrecision(18, 2);

                entity.Property(e => e.RSV)
                    .HasPrecision(18, 2)
                    .HasDefaultValueSql("'0'::numeric");

                entity.Property(e => e.RSV1)
                    .HasMaxLength(8)
                    .HasDefaultValueSql("'0'::character varying");

                entity.Property(e => e.Signal).HasMaxLength(512);

                entity.Property(e => e.VMA10).HasPrecision(18, 2);

                entity.Property(e => e.VMA120).HasPrecision(18, 2);

                entity.Property(e => e.VMA20).HasPrecision(18, 2);

                entity.Property(e => e.VMA240).HasPrecision(18, 2);

                entity.Property(e => e.VMA3).HasPrecision(18, 2);

                entity.Property(e => e.VMA5).HasPrecision(18, 2);

                entity.Property(e => e.VMA60).HasPrecision(18, 2);

                entity.Property(e => e.主力買超張數).HasPrecision(18, 3);

                entity.Property(e => e.主力賣超張數).HasPrecision(18, 3);

                entity.Property(e => e.二十日主力買超張數).HasPrecision(18, 3);

                entity.Property(e => e.二十日主力賣超張數).HasPrecision(18, 3);

                entity.Property(e => e.五日主力買超張數).HasPrecision(18, 3);

                entity.Property(e => e.五日主力賣超張數).HasPrecision(18, 3);

                entity.Property(e => e.六十日主力買超張數).HasPrecision(18, 3);

                entity.Property(e => e.六十日主力賣超張數).HasPrecision(18, 3);

                entity.Property(e => e.分價量表).HasMaxLength(4096);

                entity.Property(e => e.十日主力買超張數).HasPrecision(18, 3);

                entity.Property(e => e.十日主力賣超張數).HasPrecision(18, 3);

                entity.Property(e => e.四十日主力買超張數).HasPrecision(18, 3);

                entity.Property(e => e.四十日主力賣超張數).HasPrecision(18, 3);

                entity.Property(e => e.外資持股比例).HasPrecision(18, 2);

                entity.Property(e => e.投信持股比例).HasPrecision(18, 2);

                entity.Property(e => e.本益比).HasPrecision(18, 2);

                entity.Property(e => e.漲跌).HasPrecision(18, 2);

                entity.Property(e => e.漲跌百分比).HasPrecision(18, 2);

                entity.Property(e => e.董監持股比例).HasPrecision(18, 2);

                entity.Property(e => e.融資使用率).HasPrecision(18, 2);
            });

            modelBuilder.Entity<PricesTemp2d5c4a95>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("PricesTemp2d5c4a95");

                entity.Property(e => e.Close).HasPrecision(18, 2);

                entity.Property(e => e.D).HasPrecision(18, 2);

                entity.Property(e => e.D1).HasMaxLength(8);

                entity.Property(e => e.DIF).HasPrecision(18, 2);

                entity.Property(e => e.DIF1).HasMaxLength(8);

                entity.Property(e => e.High).HasPrecision(18, 2);

                entity.Property(e => e.K).HasPrecision(18, 2);

                entity.Property(e => e.K1).HasMaxLength(8);

                entity.Property(e => e.Low).HasPrecision(18, 2);

                entity.Property(e => e.MA10).HasPrecision(18, 2);

                entity.Property(e => e.MA10_).HasMaxLength(8);

                entity.Property(e => e.MA120).HasPrecision(18, 2);

                entity.Property(e => e.MA20).HasPrecision(18, 2);

                entity.Property(e => e.MA20_).HasMaxLength(8);

                entity.Property(e => e.MA240).HasPrecision(18, 2);

                entity.Property(e => e.MA3).HasPrecision(18, 2);

                entity.Property(e => e.MA5).HasPrecision(18, 2);

                entity.Property(e => e.MA5_).HasMaxLength(8);

                entity.Property(e => e.MA60).HasPrecision(18, 2);

                entity.Property(e => e.MA60_).HasMaxLength(8);

                entity.Property(e => e.MACD).HasPrecision(18, 2);

                entity.Property(e => e.MACD1).HasMaxLength(8);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.OSC).HasPrecision(18, 2);

                entity.Property(e => e.OSC1).HasMaxLength(8);

                entity.Property(e => e.Open).HasPrecision(18, 2);

                entity.Property(e => e.RSV).HasPrecision(18, 2);

                entity.Property(e => e.RSV1).HasMaxLength(8);

                entity.Property(e => e.Signal).HasMaxLength(512);

                entity.Property(e => e.StockId)
                    .IsRequired()
                    .HasMaxLength(8);

                entity.Property(e => e.VMA10).HasPrecision(18, 2);

                entity.Property(e => e.VMA120).HasPrecision(18, 2);

                entity.Property(e => e.VMA20).HasPrecision(18, 2);

                entity.Property(e => e.VMA240).HasPrecision(18, 2);

                entity.Property(e => e.VMA3).HasPrecision(18, 2);

                entity.Property(e => e.VMA5).HasPrecision(18, 2);

                entity.Property(e => e.VMA60).HasPrecision(18, 2);

                entity.Property(e => e.主力買超張數).HasPrecision(18, 3);

                entity.Property(e => e.主力賣超張數).HasPrecision(18, 3);

                entity.Property(e => e.二十日主力買超張數).HasPrecision(18, 3);

                entity.Property(e => e.二十日主力賣超張數).HasPrecision(18, 3);

                entity.Property(e => e.五日主力買超張數).HasPrecision(18, 3);

                entity.Property(e => e.五日主力賣超張數).HasPrecision(18, 3);

                entity.Property(e => e.六十日主力買超張數).HasPrecision(18, 3);

                entity.Property(e => e.六十日主力賣超張數).HasPrecision(18, 3);

                entity.Property(e => e.分價量表).HasMaxLength(4096);

                entity.Property(e => e.十日主力買超張數).HasPrecision(18, 3);

                entity.Property(e => e.十日主力賣超張數).HasPrecision(18, 3);

                entity.Property(e => e.四十日主力買超張數).HasPrecision(18, 3);

                entity.Property(e => e.四十日主力賣超張數).HasPrecision(18, 3);

                entity.Property(e => e.外資持股比例).HasPrecision(18, 2);

                entity.Property(e => e.投信持股比例).HasPrecision(18, 2);

                entity.Property(e => e.本益比).HasPrecision(18, 2);

                entity.Property(e => e.漲跌).HasPrecision(18, 2);

                entity.Property(e => e.漲跌百分比).HasPrecision(18, 2);

                entity.Property(e => e.董監持股比例).HasPrecision(18, 2);

                entity.Property(e => e.融資使用率).HasPrecision(18, 2);
            });

            modelBuilder.Entity<PricesTemp6919f38f>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("PricesTemp6919f38f");

                entity.Property(e => e.Close).HasPrecision(18, 2);

                entity.Property(e => e.D).HasPrecision(18, 2);

                entity.Property(e => e.D1).HasMaxLength(8);

                entity.Property(e => e.DIF).HasPrecision(18, 2);

                entity.Property(e => e.DIF1).HasMaxLength(8);

                entity.Property(e => e.High).HasPrecision(18, 2);

                entity.Property(e => e.K).HasPrecision(18, 2);

                entity.Property(e => e.K1).HasMaxLength(8);

                entity.Property(e => e.Low).HasPrecision(18, 2);

                entity.Property(e => e.MA10).HasPrecision(18, 2);

                entity.Property(e => e.MA10_).HasMaxLength(8);

                entity.Property(e => e.MA120).HasPrecision(18, 2);

                entity.Property(e => e.MA20).HasPrecision(18, 2);

                entity.Property(e => e.MA20_).HasMaxLength(8);

                entity.Property(e => e.MA240).HasPrecision(18, 2);

                entity.Property(e => e.MA3).HasPrecision(18, 2);

                entity.Property(e => e.MA5).HasPrecision(18, 2);

                entity.Property(e => e.MA5_).HasMaxLength(8);

                entity.Property(e => e.MA60).HasPrecision(18, 2);

                entity.Property(e => e.MA60_).HasMaxLength(8);

                entity.Property(e => e.MACD).HasPrecision(18, 2);

                entity.Property(e => e.MACD1).HasMaxLength(8);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.OSC).HasPrecision(18, 2);

                entity.Property(e => e.OSC1).HasMaxLength(8);

                entity.Property(e => e.Open).HasPrecision(18, 2);

                entity.Property(e => e.RSV).HasPrecision(18, 2);

                entity.Property(e => e.RSV1).HasMaxLength(8);

                entity.Property(e => e.Signal).HasMaxLength(512);

                entity.Property(e => e.StockId)
                    .IsRequired()
                    .HasMaxLength(8);

                entity.Property(e => e.VMA10).HasPrecision(18, 2);

                entity.Property(e => e.VMA120).HasPrecision(18, 2);

                entity.Property(e => e.VMA20).HasPrecision(18, 2);

                entity.Property(e => e.VMA240).HasPrecision(18, 2);

                entity.Property(e => e.VMA3).HasPrecision(18, 2);

                entity.Property(e => e.VMA5).HasPrecision(18, 2);

                entity.Property(e => e.VMA60).HasPrecision(18, 2);

                entity.Property(e => e.主力買超張數).HasPrecision(18, 3);

                entity.Property(e => e.主力賣超張數).HasPrecision(18, 3);

                entity.Property(e => e.二十日主力買超張數).HasPrecision(18, 3);

                entity.Property(e => e.二十日主力賣超張數).HasPrecision(18, 3);

                entity.Property(e => e.五日主力買超張數).HasPrecision(18, 3);

                entity.Property(e => e.五日主力賣超張數).HasPrecision(18, 3);

                entity.Property(e => e.六十日主力買超張數).HasPrecision(18, 3);

                entity.Property(e => e.六十日主力賣超張數).HasPrecision(18, 3);

                entity.Property(e => e.分價量表).HasMaxLength(4096);

                entity.Property(e => e.十日主力買超張數).HasPrecision(18, 3);

                entity.Property(e => e.十日主力賣超張數).HasPrecision(18, 3);

                entity.Property(e => e.四十日主力買超張數).HasPrecision(18, 3);

                entity.Property(e => e.四十日主力賣超張數).HasPrecision(18, 3);

                entity.Property(e => e.外資持股比例).HasPrecision(18, 2);

                entity.Property(e => e.投信持股比例).HasPrecision(18, 2);

                entity.Property(e => e.本益比).HasPrecision(18, 2);

                entity.Property(e => e.漲跌).HasPrecision(18, 2);

                entity.Property(e => e.漲跌百分比).HasPrecision(18, 2);

                entity.Property(e => e.董監持股比例).HasPrecision(18, 2);

                entity.Property(e => e.融資使用率).HasPrecision(18, 2);
            });

            modelBuilder.Entity<RealtimeBestStock>(entity =>
            {
                entity.HasIndex(e => new { e.Type, e.Datetime }, "Index_RealtimeBestStocks_Type_Datetime");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Description).HasMaxLength(50);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.StockId)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsFixedLength(true);

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<SeasonDatum>(entity =>
            {
                entity.HasKey(e => new { e.StockId, e.Datetime })
                    .HasName("SeasonData_pkey");

                entity.Property(e => e.StockId)
                    .HasMaxLength(10)
                    .IsFixedLength(true);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<Stock>(entity =>
            {
                entity.HasIndex(e => e.Status, "IX_Stocks_Status");

                entity.Property(e => e.StockId).HasMaxLength(8);

                entity.Property(e => e.Address).HasMaxLength(256);

                entity.Property(e => e.CreatedOn).HasColumnType("timestamp with time zone");

                entity.Property(e => e.Description).HasColumnType("character varying");

                entity.Property(e => e.Industry)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.ListingOn).HasColumnType("timestamp with time zone");

                entity.Property(e => e.MarketCategory)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.UpdatedOn).HasColumnType("timestamp with time zone");

                entity.Property(e => e.Website).HasMaxLength(256);

                entity.Property(e => e.營收比重).HasMaxLength(1024);
            });

            modelBuilder.Entity<StockBroker>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.BrokerId)
                    .IsRequired()
                    .HasMaxLength(32);

                entity.Property(e => e.BrokerName)
                    .IsRequired()
                    .HasMaxLength(64);

                entity.Property(e => e.EndDate).HasColumnType("date");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.StartDate).HasColumnType("date");

                entity.Property(e => e.StockId)
                    .IsRequired()
                    .HasMaxLength(8);
            });

            modelBuilder.Entity<StockHistory>(entity =>
            {
                entity.ToTable("StockHistory");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Address).HasMaxLength(256);

                entity.Property(e => e.CreatedOn).HasColumnType("timestamp with time zone");

                entity.Property(e => e.Description).HasMaxLength(256);

                entity.Property(e => e.Industry)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.ListingOn).HasColumnType("timestamp with time zone");

                entity.Property(e => e.MarketCategory)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.StockId)
                    .IsRequired()
                    .HasMaxLength(8);

                entity.Property(e => e.UpdatedOn).HasColumnType("timestamp with time zone");

                entity.Property(e => e.Website).HasMaxLength(256);

                entity.Property(e => e.營收比重).HasMaxLength(1024);
            });

            modelBuilder.Entity<Thousand>(entity =>
            {
                entity.HasKey(e => new { e.StockId, e.Datetime })
                    .HasName("Thousand_pkey");

                entity.ToTable("Thousand");

                entity.HasIndex(e => e.Datetime, "Index_Thousand_Datetime");

                entity.HasIndex(e => new { e.StockId, e.Datetime }, "Thousand_Index");

                entity.Property(e => e.StockId).HasMaxLength(8);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<Token>(entity =>
            {
                entity.HasKey(e => e.LineToken)
                    .HasName("Token_pkey");

                entity.ToTable("Token");

                entity.Property(e => e.LineToken).HasMaxLength(64);
            });

            modelBuilder.Entity<TwStock>(entity =>
            {
                entity.HasKey(e => e.Datetime)
                    .HasName("TwStock_pkey");

                entity.ToTable("TwStock");
            });

            modelBuilder.Entity<YearDatum>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.StockId)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsFixedLength(true);
            });

            modelBuilder.Entity<_Industry>(entity =>
            {
                entity.HasKey(e => e.Industry)
                    .HasName("_Industry_pkey");

                entity.ToTable("_Industry");

                entity.Property(e => e.Industry).HasMaxLength(50);
            });

            modelBuilder.Entity<_MinuteKLine>(entity =>
            {
                entity.HasKey(e => e.StockId)
                    .HasName("_MinuteKLine_pkey");

                entity.ToTable("_MinuteKLine");

                entity.Property(e => e.StockId).HasMaxLength(8);

                entity.Property(e => e.Name).HasMaxLength(50);
            });

            modelBuilder.Entity<_MonthDatum>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.Datetime)
                    .IsRequired()
                    .HasMaxLength(10);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.StockId)
                    .IsRequired()
                    .HasMaxLength(8);
            });

            modelBuilder.Entity<_Price>(entity =>
            {
                entity.HasKey(e => new { e.StockId, e.Datetime })
                    .HasName("_Prices_pkey");

                entity.Property(e => e.StockId).HasMaxLength(8);

                entity.Property(e => e.Datetime).HasMaxLength(10);

                entity.Property(e => e.Close).HasPrecision(18, 2);

                entity.Property(e => e.D9)
                    .HasMaxLength(8)
                    .HasDefaultValueSql("'0'::character varying");

                entity.Property(e => e.DIF)
                    .HasMaxLength(8)
                    .HasDefaultValueSql("'0'::character varying");

                entity.Property(e => e.High).HasPrecision(18, 2);

                entity.Property(e => e.K9)
                    .HasMaxLength(8)
                    .HasDefaultValueSql("'0'::character varying");

                entity.Property(e => e.Low).HasPrecision(18, 2);

                entity.Property(e => e.MA10).HasMaxLength(8);

                entity.Property(e => e.MA20).HasMaxLength(8);

                entity.Property(e => e.MA5).HasMaxLength(8);

                entity.Property(e => e.MA60).HasMaxLength(8);

                entity.Property(e => e.MACD)
                    .HasMaxLength(8)
                    .HasDefaultValueSql("'0'::character varying");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.OSC)
                    .HasMaxLength(8)
                    .HasDefaultValueSql("'0'::character varying");

                entity.Property(e => e.Open).HasPrecision(18, 2);

                entity.Property(e => e.RSV)
                    .HasMaxLength(8)
                    .HasDefaultValueSql("'0'::character varying");

                entity.Property(e => e.Signal)
                    .IsRequired()
                    .HasMaxLength(512);

                entity.Property(e => e.主力買賣比例).HasPrecision(18, 2);

                entity.Property(e => e.主力買賣超).HasPrecision(18, 3);

                entity.Property(e => e.二十日籌碼集中度).HasPrecision(18, 2);

                entity.Property(e => e.五日籌碼集中度).HasPrecision(18, 2);

                entity.Property(e => e.十日主力買賣比例).HasPrecision(18, 2);

                entity.Property(e => e.十日籌碼集中度).HasPrecision(18, 2);

                entity.Property(e => e.外資持股比例).HasPrecision(18, 2);

                entity.Property(e => e.投信持股比例).HasPrecision(18, 2);

                entity.Property(e => e.本益比).HasPrecision(18, 2);

                entity.Property(e => e.漲跌).HasPrecision(18, 2);

                entity.Property(e => e.漲跌百分比).HasPrecision(18, 2);

                entity.Property(e => e.籌碼集中度).HasPrecision(18, 2);

                entity.Property(e => e.董監持股比例).HasPrecision(18, 2);

                entity.Property(e => e.融資使用率).HasPrecision(18, 2);
            });

            modelBuilder.Entity<_WeekyChip>(entity =>
            {
                entity.ToTable("_WeekyChip");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Datetime)
                    .IsRequired()
                    .HasMaxLength(10);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.StockId)
                    .IsRequired()
                    .HasMaxLength(8);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
