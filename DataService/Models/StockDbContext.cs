using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

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
        public virtual DbSet<KeyBroker> KeyBrokers { get; set; }
        public virtual DbSet<MinuteKLine> MinuteKLines { get; set; }
        public virtual DbSet<MonthDatum> MonthData { get; set; }
        public virtual DbSet<Price> Prices { get; set; }
        public virtual DbSet<PricesTemp2d5c4a95> PricesTemp2d5c4a95s { get; set; }
        public virtual DbSet<PricesTemp36bedfd8> PricesTemp36bedfd8s { get; set; }
        public virtual DbSet<PricesTemp6919f38f> PricesTemp6919f38fs { get; set; }
        public virtual DbSet<PricesTempbc77880a> PricesTempbc77880as { get; set; }
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
                optionsBuilder.UseSqlServer("Server=220.133.185.1;Database=StockDb;User ID=stock;Password=stock;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "Chinese_Taiwan_Stroke_CI_AS");

            modelBuilder.Entity<AnaCMoney>(entity =>
            {
                entity.ToTable("AnaCMoney");

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
                entity.ToTable("AnaFutureEngine");

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

            modelBuilder.Entity<AnaStatementDog>(entity =>
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

            modelBuilder.Entity<BestStock>(entity =>
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

                entity.Property(e => e.BusinessDay).HasColumnType("datetime");

                entity.Property(e => e.MainName).HasMaxLength(64);

                entity.Property(e => e.Tel)
                    .HasMaxLength(64)
                    .IsUnicode(false);

                entity.Property(e => e.b).HasMaxLength(32);
            });

            modelBuilder.Entity<BrokerTransaction>(entity =>
            {
                entity.ToTable("BrokerTransaction");

                entity.HasIndex(e => e.StockId, "IX_BrokerTransaction");

                entity.HasIndex(e => new { e.BrokerName, e.StockId, e.Datetime }, "UX_BrokerTransaction")
                    .IsUnique();

                entity.HasIndex(e => new { e.BrokerName, e.StockId, e.Datetime }, "UX_BrokerTransactionDetails")
                    .IsUnique();

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.BrokerName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Buy).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.Close).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.Datetime).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(12);

                entity.Property(e => e.Percent).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.Sell).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.StockId)
                    .IsRequired()
                    .HasMaxLength(10);

                entity.Property(e => e.買賣超).HasColumnType("decimal(18, 2)");
            });

            modelBuilder.Entity<BrokerTransactionDetail>(entity =>
            {
                entity.HasKey(e => new { e.BrokerId, e.StockId, e.Datetime })
                    .HasName("PK_BrokerTransactionDetails_1")
                    .IsClustered(false);

                entity.HasIndex(e => e.StockId, "Index_BrokerTransactionDetails_StockId")
                    .IsClustered();

                entity.Property(e => e.BrokerId)
                    .HasMaxLength(16)
                    .IsUnicode(false);

                entity.Property(e => e.StockId).HasMaxLength(10);

                entity.Property(e => e.Datetime).HasColumnType("datetime");

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
                entity.ToTable("HistoryPrice");

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

            modelBuilder.Entity<KeyBroker>(entity =>
            {
                entity.ToTable("KeyBroker");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.BHID).HasMaxLength(32);

                entity.Property(e => e.BrokerName)
                    .IsRequired()
                    .HasMaxLength(64);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.StockId)
                    .IsRequired()
                    .HasMaxLength(8)
                    .IsUnicode(false);

                entity.Property(e => e.b).HasMaxLength(32);
            });

            modelBuilder.Entity<MinuteKLine>(entity =>
            {
                entity.HasKey(e => new { e.StockId, e.Datetime })
                    .HasName("PK_MinuteKine");

                entity.ToTable("MinuteKLine");

                entity.HasIndex(e => e.Datetime, "Index_MinuteKLine_Datetime");

                entity.Property(e => e.StockId)
                    .HasMaxLength(8)
                    .IsUnicode(false);

                entity.Property(e => e.Datetime).HasColumnType("datetime");

                entity.Property(e => e.Close).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.High).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.Low).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Open).HasColumnType("numeric(18, 2)");
            });

            modelBuilder.Entity<MonthDatum>(entity =>
            {
                entity.HasKey(e => new { e.StockId, e.Datetime })
                    .HasName("PK_MonthData_1");

                entity.HasIndex(e => e.Datetime, "Index_MonthData_Datetime");

                entity.HasIndex(e => e.Datetime, "MonthData_StockId_Name");

                entity.Property(e => e.StockId)
                    .HasMaxLength(8)
                    .IsUnicode(false);

                entity.Property(e => e.Datetime).HasColumnType("datetime");

                entity.Property(e => e.Close).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Percent).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.去年同月營收).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.去年累計營收).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.單月年增率).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.單月月增率).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.單月營收).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.累積年增率).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.累計營收).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.董監持股增減).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.董監持股比例).HasColumnType("decimal(18, 2)");
            });

            modelBuilder.Entity<Price>(entity =>
            {
                entity.HasKey(e => new { e.StockId, e.Datetime });

                entity.HasIndex(e => e.StockId, "IX_Prices");

                entity.HasIndex(e => e.StockId, "Index_Datetime");

                entity.HasIndex(e => new { e.Datetime, e.StockId }, "Index_For_Update_Close");

                entity.HasIndex(e => e.Datetime, "Index_Prices_Datetime");

                entity.HasIndex(e => e.Datetime, "Index_Prices_Datetime2");

                entity.HasIndex(e => new { e.Datetime, e.漲跌百分比, e.成交量 }, "Index_Prices_Datetime_成交量");

                entity.HasIndex(e => new { e.Datetime, e.成交量, e.投信買賣超 }, "Index_[投信買賣超");

                entity.HasIndex(e => e.Datetime, "Prices_10days_Index");

                entity.HasIndex(e => e.Datetime, "Prices_20days_Index");

                entity.HasIndex(e => e.Datetime, "Prices_5days_Index");

                entity.HasIndex(e => e.Datetime, "Prices_60days_Index");

                entity.HasIndex(e => new { e.Datetime, e.外資買賣超, e.投信買賣超 }, "Prices_ITrust_Index");

                entity.HasIndex(e => new { e.Datetime, e.外資買賣超 }, "Prices_MainForce_Index");

                entity.Property(e => e.StockId)
                    .HasMaxLength(8)
                    .IsUnicode(false);

                entity.Property(e => e.Datetime).HasColumnType("datetime");

                entity.Property(e => e.Close).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.Property(e => e.D)
                    .HasColumnType("numeric(18, 2)")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.D1)
                    .HasMaxLength(8)
                    .IsUnicode(false)
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.DIF)
                    .HasColumnType("numeric(18, 2)")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.DIF1)
                    .HasMaxLength(8)
                    .IsUnicode(false)
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.High).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.K)
                    .HasColumnType("numeric(18, 2)")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.K1)
                    .HasMaxLength(8)
                    .IsUnicode(false)
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.Low).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.MA10).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.MA10_)
                    .HasMaxLength(8)
                    .IsUnicode(false);

                entity.Property(e => e.MA120).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.MA20).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.MA20_)
                    .HasMaxLength(8)
                    .IsUnicode(false);

                entity.Property(e => e.MA240).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.MA3).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.MA5).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.MA5_)
                    .HasMaxLength(8)
                    .IsUnicode(false);

                entity.Property(e => e.MA60).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.MA60_)
                    .HasMaxLength(8)
                    .IsUnicode(false);

                entity.Property(e => e.MACD)
                    .HasColumnType("numeric(18, 2)")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.MACD1)
                    .HasMaxLength(8)
                    .IsUnicode(false)
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.OSC)
                    .HasColumnType("numeric(18, 2)")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.OSC1)
                    .HasMaxLength(8)
                    .IsUnicode(false)
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.Open).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.RSV)
                    .HasColumnType("numeric(18, 2)")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.RSV1)
                    .HasMaxLength(8)
                    .IsUnicode(false)
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.Signal).HasMaxLength(512);

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

                entity.Property(e => e.分價量表)
                    .HasMaxLength(4096)
                    .IsUnicode(false);

                entity.Property(e => e.十日主力買超張數).HasColumnType("numeric(18, 3)");

                entity.Property(e => e.十日主力賣超張數).HasColumnType("numeric(18, 3)");

                entity.Property(e => e.四十日主力買超張數).HasColumnType("numeric(18, 3)");

                entity.Property(e => e.四十日主力賣超張數).HasColumnType("numeric(18, 3)");

                entity.Property(e => e.外資持股比例).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.投信持股比例).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.本益比).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.漲跌).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.漲跌百分比).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.當沖均損益).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.當沖張數).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.當沖比例).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.當沖總損益).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.董監持股比例).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.融資使用率).HasColumnType("numeric(18, 2)");
            });

            modelBuilder.Entity<PricesTemp2d5c4a95>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("PricesTemp2d5c4a95");

                entity.Property(e => e.Close).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.Property(e => e.D).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.D1)
                    .HasMaxLength(8)
                    .IsUnicode(false);

                entity.Property(e => e.DIF).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.DIF1)
                    .HasMaxLength(8)
                    .IsUnicode(false);

                entity.Property(e => e.Datetime).HasColumnType("datetime");

                entity.Property(e => e.High).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.K).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.K1)
                    .HasMaxLength(8)
                    .IsUnicode(false);

                entity.Property(e => e.Low).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.MA10).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.MA10_)
                    .HasMaxLength(8)
                    .IsUnicode(false);

                entity.Property(e => e.MA120).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.MA20).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.MA20_)
                    .HasMaxLength(8)
                    .IsUnicode(false);

                entity.Property(e => e.MA240).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.MA3).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.MA5).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.MA5_)
                    .HasMaxLength(8)
                    .IsUnicode(false);

                entity.Property(e => e.MA60).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.MA60_)
                    .HasMaxLength(8)
                    .IsUnicode(false);

                entity.Property(e => e.MACD).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.MACD1)
                    .HasMaxLength(8)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.OSC).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.OSC1)
                    .HasMaxLength(8)
                    .IsUnicode(false);

                entity.Property(e => e.Open).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.RSV).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.RSV1)
                    .HasMaxLength(8)
                    .IsUnicode(false);

                entity.Property(e => e.Signal).HasMaxLength(512);

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

                entity.Property(e => e.分價量表)
                    .HasMaxLength(4096)
                    .IsUnicode(false);

                entity.Property(e => e.十日主力買超張數).HasColumnType("numeric(18, 3)");

                entity.Property(e => e.十日主力賣超張數).HasColumnType("numeric(18, 3)");

                entity.Property(e => e.四十日主力買超張數).HasColumnType("numeric(18, 3)");

                entity.Property(e => e.四十日主力賣超張數).HasColumnType("numeric(18, 3)");

                entity.Property(e => e.外資持股比例).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.投信持股比例).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.本益比).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.漲跌).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.漲跌百分比).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.當沖均損益).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.當沖張數).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.當沖比例).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.當沖總損益).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.董監持股比例).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.融資使用率).HasColumnType("numeric(18, 2)");
            });

            modelBuilder.Entity<PricesTemp36bedfd8>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("PricesTemp36bedfd8");

                entity.Property(e => e.Close).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.Property(e => e.D).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.D1)
                    .HasMaxLength(8)
                    .IsUnicode(false);

                entity.Property(e => e.DIF).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.DIF1)
                    .HasMaxLength(8)
                    .IsUnicode(false);

                entity.Property(e => e.Datetime).HasColumnType("datetime");

                entity.Property(e => e.High).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.K).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.K1)
                    .HasMaxLength(8)
                    .IsUnicode(false);

                entity.Property(e => e.Low).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.MA10).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.MA10_)
                    .HasMaxLength(8)
                    .IsUnicode(false);

                entity.Property(e => e.MA120).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.MA20).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.MA20_)
                    .HasMaxLength(8)
                    .IsUnicode(false);

                entity.Property(e => e.MA240).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.MA3).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.MA5).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.MA5_)
                    .HasMaxLength(8)
                    .IsUnicode(false);

                entity.Property(e => e.MA60).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.MA60_)
                    .HasMaxLength(8)
                    .IsUnicode(false);

                entity.Property(e => e.MACD).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.MACD1)
                    .HasMaxLength(8)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.OSC).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.OSC1)
                    .HasMaxLength(8)
                    .IsUnicode(false);

                entity.Property(e => e.Open).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.RSV).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.RSV1)
                    .HasMaxLength(8)
                    .IsUnicode(false);

                entity.Property(e => e.Signal).HasMaxLength(512);

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

                entity.Property(e => e.分價量表)
                    .HasMaxLength(4096)
                    .IsUnicode(false);

                entity.Property(e => e.十日主力買超張數).HasColumnType("numeric(18, 3)");

                entity.Property(e => e.十日主力賣超張數).HasColumnType("numeric(18, 3)");

                entity.Property(e => e.四十日主力買超張數).HasColumnType("numeric(18, 3)");

                entity.Property(e => e.四十日主力賣超張數).HasColumnType("numeric(18, 3)");

                entity.Property(e => e.外資持股比例).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.投信持股比例).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.本益比).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.漲跌).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.漲跌百分比).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.當沖均損益).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.當沖張數).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.當沖比例).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.當沖總損益).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.董監持股比例).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.融資使用率).HasColumnType("numeric(18, 2)");
            });

            modelBuilder.Entity<PricesTemp6919f38f>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("PricesTemp6919f38f");

                entity.Property(e => e.Close).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.Property(e => e.D).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.D1)
                    .HasMaxLength(8)
                    .IsUnicode(false);

                entity.Property(e => e.DIF).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.DIF1)
                    .HasMaxLength(8)
                    .IsUnicode(false);

                entity.Property(e => e.Datetime).HasColumnType("datetime");

                entity.Property(e => e.High).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.K).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.K1)
                    .HasMaxLength(8)
                    .IsUnicode(false);

                entity.Property(e => e.Low).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.MA10).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.MA10_)
                    .HasMaxLength(8)
                    .IsUnicode(false);

                entity.Property(e => e.MA120).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.MA20).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.MA20_)
                    .HasMaxLength(8)
                    .IsUnicode(false);

                entity.Property(e => e.MA240).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.MA3).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.MA5).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.MA5_)
                    .HasMaxLength(8)
                    .IsUnicode(false);

                entity.Property(e => e.MA60).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.MA60_)
                    .HasMaxLength(8)
                    .IsUnicode(false);

                entity.Property(e => e.MACD).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.MACD1)
                    .HasMaxLength(8)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.OSC).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.OSC1)
                    .HasMaxLength(8)
                    .IsUnicode(false);

                entity.Property(e => e.Open).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.RSV).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.RSV1)
                    .HasMaxLength(8)
                    .IsUnicode(false);

                entity.Property(e => e.Signal).HasMaxLength(512);

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

                entity.Property(e => e.分價量表)
                    .HasMaxLength(4096)
                    .IsUnicode(false);

                entity.Property(e => e.十日主力買超張數).HasColumnType("numeric(18, 3)");

                entity.Property(e => e.十日主力賣超張數).HasColumnType("numeric(18, 3)");

                entity.Property(e => e.四十日主力買超張數).HasColumnType("numeric(18, 3)");

                entity.Property(e => e.四十日主力賣超張數).HasColumnType("numeric(18, 3)");

                entity.Property(e => e.外資持股比例).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.投信持股比例).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.本益比).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.漲跌).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.漲跌百分比).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.當沖均損益).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.當沖張數).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.當沖比例).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.當沖總損益).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.董監持股比例).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.融資使用率).HasColumnType("numeric(18, 2)");
            });

            modelBuilder.Entity<PricesTempbc77880a>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("PricesTempbc77880a");

                entity.Property(e => e.Close).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.Property(e => e.D).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.D1)
                    .HasMaxLength(8)
                    .IsUnicode(false);

                entity.Property(e => e.DIF).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.DIF1)
                    .HasMaxLength(8)
                    .IsUnicode(false);

                entity.Property(e => e.Datetime).HasColumnType("datetime");

                entity.Property(e => e.High).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.K).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.K1)
                    .HasMaxLength(8)
                    .IsUnicode(false);

                entity.Property(e => e.Low).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.MA10).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.MA10_)
                    .HasMaxLength(8)
                    .IsUnicode(false);

                entity.Property(e => e.MA120).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.MA20).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.MA20_)
                    .HasMaxLength(8)
                    .IsUnicode(false);

                entity.Property(e => e.MA240).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.MA3).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.MA5).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.MA5_)
                    .HasMaxLength(8)
                    .IsUnicode(false);

                entity.Property(e => e.MA60).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.MA60_)
                    .HasMaxLength(8)
                    .IsUnicode(false);

                entity.Property(e => e.MACD).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.MACD1)
                    .HasMaxLength(8)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.OSC).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.OSC1)
                    .HasMaxLength(8)
                    .IsUnicode(false);

                entity.Property(e => e.Open).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.RSV).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.RSV1)
                    .HasMaxLength(8)
                    .IsUnicode(false);

                entity.Property(e => e.Signal).HasMaxLength(512);

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

                entity.Property(e => e.分價量表)
                    .HasMaxLength(4096)
                    .IsUnicode(false);

                entity.Property(e => e.十日主力買超張數).HasColumnType("numeric(18, 3)");

                entity.Property(e => e.十日主力賣超張數).HasColumnType("numeric(18, 3)");

                entity.Property(e => e.四十日主力買超張數).HasColumnType("numeric(18, 3)");

                entity.Property(e => e.四十日主力賣超張數).HasColumnType("numeric(18, 3)");

                entity.Property(e => e.外資持股比例).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.投信持股比例).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.本益比).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.漲跌).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.漲跌百分比).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.當沖均損益).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.當沖張數).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.當沖比例).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.當沖總損益).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.董監持股比例).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.融資使用率).HasColumnType("numeric(18, 2)");
            });

            modelBuilder.Entity<RealtimeBestStock>(entity =>
            {
                entity.HasIndex(e => new { e.Type, e.Datetime }, "Index_RealtimeBestStocks_Type_Datetime");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Datetime).HasColumnType("datetime");

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
                    .HasName("PK_SeasonData_1");

                entity.Property(e => e.StockId)
                    .HasMaxLength(10)
                    .IsFixedLength(true);

                entity.Property(e => e.Datetime).HasColumnType("datetime");

                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.ROA).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.ROE).HasColumnType("decimal(18, 2)");

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

            modelBuilder.Entity<Stock>(entity =>
            {
                entity.HasIndex(e => e.Status, "IX_Stocks_Status");

                entity.Property(e => e.StockId)
                    .HasMaxLength(8)
                    .IsUnicode(false);

                entity.Property(e => e.Address).HasMaxLength(256);

                entity.Property(e => e.Industry)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.MarketCategory)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.ROA).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.ROE).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.Website).HasMaxLength(256);

                entity.Property(e => e.每股淨值).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.每股盈餘).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.營收比重).HasMaxLength(1024);

                entity.Property(e => e.股價).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.股本).HasColumnType("decimal(18, 2)");
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
                    .HasMaxLength(8)
                    .IsUnicode(false);

                entity.Property(e => e.Volume).HasColumnType("decimal(18, 0)");
            });

            modelBuilder.Entity<StockHistory>(entity =>
            {
                entity.ToTable("StockHistory");

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
                entity.HasKey(e => new { e.StockId, e.Datetime })
                    .HasName("PK_Thousand_1");

                entity.ToTable("Thousand");

                entity.HasIndex(e => e.Datetime, "Index_Thousand_Datetime");

                entity.HasIndex(e => new { e.StockId, e.Datetime }, "Thousand_Index");

                entity.Property(e => e.StockId)
                    .HasMaxLength(8)
                    .IsUnicode(false);

                entity.Property(e => e.Datetime).HasColumnType("datetime");

                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

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

                entity.Property(e => e.PPOver1000).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.PPUnder100).HasColumnType("decimal(18, 2)");

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

                entity.Property(e => e.TotalStock).HasColumnType("decimal(18, 3)");
            });

            modelBuilder.Entity<Token>(entity =>
            {
                entity.HasKey(e => e.LineToken);

                entity.ToTable("Token");

                entity.Property(e => e.LineToken).HasMaxLength(64);
            });

            modelBuilder.Entity<TwStock>(entity =>
            {
                entity.HasKey(e => e.Datetime);

                entity.ToTable("TwStock");

                entity.Property(e => e.Datetime).HasColumnType("datetime");

                entity.Property(e => e.交易口數PC比).HasColumnType("decimal(17, 2)");

                entity.Property(e => e.前五大).HasColumnType("decimal(17, 2)");

                entity.Property(e => e.前五特).HasColumnType("decimal(17, 2)");

                entity.Property(e => e.前十大).HasColumnType("decimal(17, 2)");

                entity.Property(e => e.前十特).HasColumnType("decimal(17, 2)");

                entity.Property(e => e.外資未平倉).HasColumnType("decimal(17, 2)");

                entity.Property(e => e.外資買賣超).HasColumnType("decimal(17, 2)");

                entity.Property(e => e.成交量).HasColumnType("decimal(17, 2)");

                entity.Property(e => e.投信未平倉).HasColumnType("decimal(17, 2)");

                entity.Property(e => e.投信買賣超).HasColumnType("decimal(17, 2)");

                entity.Property(e => e.收盤價).HasColumnType("decimal(17, 2)");

                entity.Property(e => e.未平倉口數PC比).HasColumnType("decimal(17, 2)");

                entity.Property(e => e.漲跌).HasColumnType("decimal(17, 2)");

                entity.Property(e => e.漲跌百分比).HasColumnType("decimal(17, 2)");

                entity.Property(e => e.總計).HasColumnType("decimal(17, 2)");

                entity.Property(e => e.總計未平倉).HasColumnType("decimal(17, 2)");

                entity.Property(e => e.自營未平倉).HasColumnType("decimal(17, 2)");

                entity.Property(e => e.自營總).HasColumnType("decimal(17, 2)");

                entity.Property(e => e.自營自買).HasColumnType("decimal(17, 2)");

                entity.Property(e => e.自營避險).HasColumnType("decimal(17, 2)");

                entity.Property(e => e.融券增加).HasColumnType("decimal(17, 2)");

                entity.Property(e => e.融券餘額).HasColumnType("decimal(17, 2)");

                entity.Property(e => e.融資增加).HasColumnType("decimal(17, 2)");

                entity.Property(e => e.融資餘額).HasColumnType("decimal(17, 2)");
            });

            modelBuilder.Entity<YearDatum>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.StockId)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsFixedLength(true);

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

            modelBuilder.Entity<_Industry>(entity =>
            {
                entity.HasKey(e => e.Industry);

                entity.ToTable("_Industry");

                entity.Property(e => e.Industry).HasMaxLength(50);

                entity.Property(e => e.percent).HasColumnType("decimal(18, 2)");
            });

            modelBuilder.Entity<_MinuteKLine>(entity =>
            {
                entity.HasKey(e => e.StockId);

                entity.ToTable("_MinuteKLine");

                entity.Property(e => e.StockId)
                    .HasMaxLength(8)
                    .IsUnicode(false);

                entity.Property(e => e.Datetime).HasColumnType("datetime");

                entity.Property(e => e.Name).HasMaxLength(50);
            });

            modelBuilder.Entity<_MonthDatum>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.Close).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.Datetime)
                    .IsRequired()
                    .HasMaxLength(10);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Percent).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.ROA).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.ROE).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.StockId)
                    .IsRequired()
                    .HasMaxLength(8)
                    .IsUnicode(false);

                entity.Property(e => e.公告每股淨值).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.單月年增率).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.單月月增率).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.每股營業額).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.每股稅後盈餘).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.毛利率).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.營業利益率).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.累積年增率).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.股本).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.股東權益).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.董監持股增減).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.董監持股比例).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.負債總計).HasColumnType("decimal(18, 2)");
            });

            modelBuilder.Entity<_Price>(entity =>
            {
                entity.HasKey(e => new { e.StockId, e.Datetime });

                entity.Property(e => e.StockId)
                    .HasMaxLength(8)
                    .IsUnicode(false);

                entity.Property(e => e.Datetime).HasMaxLength(10);

                entity.Property(e => e.Close).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.D9)
                    .HasMaxLength(8)
                    .IsUnicode(false)
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.DIF)
                    .HasMaxLength(8)
                    .IsUnicode(false)
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.High).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.K9)
                    .HasMaxLength(8)
                    .IsUnicode(false)
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.Low).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.MA10)
                    .HasMaxLength(8)
                    .IsUnicode(false);

                entity.Property(e => e.MA20)
                    .HasMaxLength(8)
                    .IsUnicode(false);

                entity.Property(e => e.MA5)
                    .HasMaxLength(8)
                    .IsUnicode(false);

                entity.Property(e => e.MA60)
                    .HasMaxLength(8)
                    .IsUnicode(false);

                entity.Property(e => e.MACD)
                    .HasMaxLength(8)
                    .IsUnicode(false)
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.OSC)
                    .HasMaxLength(8)
                    .IsUnicode(false)
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.Open).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.RSV)
                    .HasMaxLength(8)
                    .IsUnicode(false)
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.Signal)
                    .IsRequired()
                    .HasMaxLength(512);

                entity.Property(e => e.主力買賣比例).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.主力買賣超).HasColumnType("numeric(18, 3)");

                entity.Property(e => e.二十日籌碼集中度).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.五日籌碼集中度).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.十日主力買賣比例).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.十日籌碼集中度).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.外資持股比例).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.投信持股比例).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.本益比).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.漲跌).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.漲跌百分比).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.當沖張數).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.當沖比例).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.籌碼集中度).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.董監持股比例).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.融資使用率).HasColumnType("numeric(18, 2)");
            });

            modelBuilder.Entity<_WeekyChip>(entity =>
            {
                entity.ToTable("_WeekyChip");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Close).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.Datetime)
                    .IsRequired()
                    .HasMaxLength(10);

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

                entity.Property(e => e.主力買賣超).HasColumnType("decimal(18, 3)");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
