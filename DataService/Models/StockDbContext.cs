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
        public virtual DbSet<BestStockType> BestStockType { get; set; }
        public virtual DbSet<BestStocks> BestStocks { get; set; }
        public virtual DbSet<Broker> Broker { get; set; }
        public virtual DbSet<BrokerTransaction> BrokerTransaction { get; set; }
        public virtual DbSet<BrokerTransactionDetails> BrokerTransactionDetails { get; set; }
        public virtual DbSet<Chip> Chip { get; set; }
        public virtual DbSet<HistoryPrice> HistoryPrice { get; set; }
        public virtual DbSet<MinuteKLine> MinuteKLine { get; set; }
        public virtual DbSet<MonthData> MonthData { get; set; }
        public virtual DbSet<Prices> Prices { get; set; }
        public virtual DbSet<RealtimeBestStocks> RealtimeBestStocks { get; set; }
        public virtual DbSet<SeasonData> SeasonData { get; set; }
        public virtual DbSet<StockBrokers> StockBrokers { get; set; }
        public virtual DbSet<StockHistory> StockHistory { get; set; }
        public virtual DbSet<Stocks> Stocks { get; set; }
        public virtual DbSet<Thousand> Thousand { get; set; }
        public virtual DbSet<Token> Token { get; set; }
        public virtual DbSet<TwStock> TwStock { get; set; }
        public virtual DbSet<YearData> YearData { get; set; }
        public virtual DbSet<_MinuteKLine> _MinuteKLine { get; set; }
        public virtual DbSet<_MonthData> _MonthData { get; set; }
        public virtual DbSet<_Prices> _Prices { get; set; }
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

            modelBuilder.Entity<BestStockType>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.Value)
                    .IsRequired()
                    .HasMaxLength(50);
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

            modelBuilder.Entity<Broker>(entity =>
            {
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
                entity.HasIndex(e => e.StockId)
                    .HasName("IX_BrokerTransaction");

                entity.HasIndex(e => new { e.BrokerName, e.StockId, e.Datetime })
                    .HasName("UX_BrokerTransactionDetails")
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

            modelBuilder.Entity<BrokerTransactionDetails>(entity =>
            {
                entity.HasKey(e => new { e.BrokerId, e.StockId, e.Datetime })
                    .HasName("PK_BrokerTransactionDetails_1")
                    .IsClustered(false);

                entity.HasIndex(e => e.StockId)
                    .HasName("Index_BrokerTransactionDetails_StockId")
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

            modelBuilder.Entity<MinuteKLine>(entity =>
            {
                entity.HasKey(e => new { e.StockId, e.Datetime })
                    .HasName("PK_MinuteKine");

                entity.HasIndex(e => new { e.Name, e.Open, e.High, e.Low, e.Close, e.Volume, e.Datetime })
                    .HasName("Index_MinuteKLine_Datetime");

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

            modelBuilder.Entity<MonthData>(entity =>
            {
                entity.HasKey(e => new { e.StockId, e.Datetime })
                    .HasName("PK_MonthData_1");

                entity.HasIndex(e => new { e.StockId, e.Name, e.單月年增率, e.Datetime })
                    .HasName("MonthData_StockId_Name");

                entity.HasIndex(e => new { e.Name, e.單月營收, e.去年同月營收, e.單月月增率, e.單月年增率, e.累計營收, e.去年累計營收, e.累積年增率, e.CreatedOn, e.董監持股增減, e.Close, e.Percent, e.董監持股比例, e.Datetime })
                    .HasName("Index_MonthData_Datetime");

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

            modelBuilder.Entity<Prices>(entity =>
            {
                entity.HasKey(e => new { e.StockId, e.Datetime });

                entity.HasIndex(e => e.StockId)
                    .HasName("IX_Prices");

                entity.HasIndex(e => new { e.Datetime, e.StockId })
                    .HasName("Index_Datetime");

                entity.HasIndex(e => new { e.Close, e.Datetime, e.StockId })
                    .HasName("Index_For_Update_Close");

                entity.HasIndex(e => new { e.StockId, e.Datetime, e.外資買賣超, e.投信買賣超 })
                    .HasName("Prices_ITrust_Index");

                entity.HasIndex(e => new { e.StockId, e.Close, e.VMA10, e.十日主力買超張數, e.十日主力賣超張數, e.Datetime })
                    .HasName("Prices_10days_Index");

                entity.HasIndex(e => new { e.StockId, e.Close, e.VMA20, e.二十日主力買超張數, e.二十日主力賣超張數, e.Datetime })
                    .HasName("Prices_20days_Index");

                entity.HasIndex(e => new { e.StockId, e.Close, e.VMA5, e.五日主力買超張數, e.五日主力賣超張數, e.Datetime })
                    .HasName("Prices_5days_Index");

                entity.HasIndex(e => new { e.StockId, e.Close, e.VMA60, e.六十日主力買超張數, e.六十日主力賣超張數, e.Datetime })
                    .HasName("Prices_60days_Index");

                entity.HasIndex(e => new { e.StockId, e.主力買超張數, e.主力賣超張數, e.Datetime, e.成交量, e.投信買賣超 })
                    .HasName("Index_[投信買賣超");

                entity.HasIndex(e => new { e.StockId, e.投信買賣超, e.主力買超張數, e.主力賣超張數, e.Datetime, e.外資買賣超 })
                    .HasName("Prices_MainForce_Index");

                entity.HasIndex(e => new { e.Open, e.Close, e.MA20, e.MA60, e.主力買超張數, e.主力賣超張數, e.Datetime, e.漲跌百分比, e.成交量 })
                    .HasName("Index_Prices_Datetime_成交量");

                entity.HasIndex(e => new { e.StockId, e.Name, e.High, e.Low, e.Open, e.DIF1, e.MACD1, e.OSC1, e.RSV1, e.K1, e.D1, e.DIF, e.MACD, e.OSC, e.RSV, e.K, e.D, e.六十日主力賣超張數, e.投信持股, e.投信持股比例, e.自營商持股, e.董監持股, e.董監持股比例, e.十日主力賣超張數, e.二十日主力買超張數, e.二十日主力賣超張數, e.四十日主力買超張數, e.四十日主力賣超張數, e.六十日主力買超張數, e.VMA240, e.主力買超張數, e.主力賣超張數, e.五日主力買超張數, e.五日主力賣超張數, e.十日主力買超張數, e.VMA3, e.VMA5, e.VMA10, e.VMA20, e.VMA60, e.VMA120, e.MA5, e.MA10, e.MA20, e.MA60, e.MA120, e.MA240, e.當沖均損益, e.MA3, e.MA5_, e.MA10_, e.MA20_, e.MA60_, e.外資買賣超, e.投信買賣超, e.自營商買賣超, e.當沖張數, e.當沖比例, e.當沖總損益, e.融券買進, e.融券賣出, e.融券餘額, e.資券相抵, e.外資持股, e.外資持股比例, e.CreatedOn, e.融資買進, e.融資賣出, e.融資現償, e.融資餘額, e.融資使用率, e.Close, e.漲跌, e.漲跌百分比, e.成交量, e.成交金額, e.本益比, e.Datetime })
                    .HasName("Index_Prices_Datetime");

                entity.HasIndex(e => new { e.Name, e.High, e.Low, e.Open, e.Close, e.漲跌, e.漲跌百分比, e.成交量, e.成交金額, e.本益比, e.CreatedOn, e.融資買進, e.融資賣出, e.融資現償, e.融資餘額, e.融資使用率, e.融券買進, e.融券賣出, e.融券餘額, e.資券相抵, e.外資持股, e.外資持股比例, e.外資買賣超, e.投信買賣超, e.自營商買賣超, e.當沖張數, e.當沖比例, e.當沖總損益, e.當沖均損益, e.MA3, e.MA5_, e.MA10_, e.MA20_, e.MA60_, e.MA5, e.MA10, e.MA20, e.MA60, e.MA120, e.MA240, e.VMA3, e.VMA5, e.VMA10, e.VMA20, e.VMA60, e.VMA120, e.VMA240, e.主力買超張數, e.主力賣超張數, e.五日主力買超張數, e.五日主力賣超張數, e.十日主力買超張數, e.十日主力賣超張數, e.二十日主力買超張數, e.二十日主力賣超張數, e.四十日主力買超張數, e.四十日主力賣超張數, e.六十日主力買超張數, e.六十日主力賣超張數, e.投信持股, e.投信持股比例, e.自營商持股, e.董監持股, e.董監持股比例, e.DIF, e.MACD, e.OSC, e.RSV, e.K, e.D, e.DIF1, e.MACD1, e.OSC1, e.RSV1, e.K1, e.D1, e.董監持股買賣, e.AvgUpDays, e.Datetime })
                    .HasName("Index_Prices_Datetime2");

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

            modelBuilder.Entity<RealtimeBestStocks>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Datetime).HasColumnType("datetime");

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

            modelBuilder.Entity<SeasonData>(entity =>
            {
                entity.HasKey(e => new { e.StockId, e.Datetime })
                    .HasName("PK_SeasonData_1");

                entity.Property(e => e.StockId)
                    .HasMaxLength(10)
                    .IsFixedLength();

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

            modelBuilder.Entity<StockBrokers>(entity =>
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
                entity.HasKey(e => e.StockId)
                    .HasName("PK_Stock");

                entity.HasIndex(e => new { e.StockId, e.Name, e.MarketCategory, e.Industry, e.ListingOn, e.CreatedOn, e.UpdatedOn, e.Address, e.Website, e.營收比重, e.股本, e.股價, e.每股淨值, e.每股盈餘, e.Description, e.ROE, e.ROA, e.股票期貨, e.Status })
                    .HasName("IX_Stocks_Status");

                entity.Property(e => e.StockId)
                    .HasMaxLength(8)
                    .IsUnicode(false);

                entity.Property(e => e.Address).HasMaxLength(256);

                entity.Property(e => e.Description).HasColumnType("nvarchar(max)");

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

            modelBuilder.Entity<Thousand>(entity =>
            {
                entity.HasKey(e => new { e.StockId, e.Datetime })
                    .HasName("PK_Thousand_1");

                entity.HasIndex(e => new { e.Name, e.PUnder100, e.P200, e.P400, e.S600, e.S800, e.S1000, e.SOver1000, e.SUnder100, e.P600, e.P800, e.P1000, e.POver1000, e.S200, e.S400, e.StockId, e.Datetime })
                    .HasName("Thousand_Index");

                entity.HasIndex(e => new { e.StockId, e.Name, e.CreatedOn, e.PPUnder100, e.PUnder100, e.P1, e.P5, e.P10, e.P15, e.P20, e.P30, e.P40, e.P50, e.P100, e.P200, e.P400, e.P600, e.P800, e.P1000, e.PPOver1000, e.POver1000, e.C1, e.C5, e.C10, e.C15, e.C20, e.C30, e.C40, e.C50, e.C100, e.C200, e.C400, e.C600, e.C800, e.C1000, e.COver1000, e.S1, e.S5, e.S10, e.S15, e.S20, e.S30, e.S40, e.S50, e.S100, e.S200, e.S400, e.S600, e.S800, e.S1000, e.SOver1000, e.TotalCount, e.TotalStock, e.CUnder100, e.SUnder100, e.Datetime })
                    .HasName("Index_Thousand_Datetime");

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

                entity.Property(e => e.LineToken).HasMaxLength(64);
            });

            modelBuilder.Entity<TwStock>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Datetime).HasColumnType("date");

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

            modelBuilder.Entity<_MinuteKLine>(entity =>
            {
                entity.HasKey(e => e.StockId);

                entity.Property(e => e.StockId)
                    .HasMaxLength(8)
                    .IsUnicode(false);

                entity.Property(e => e.Datetime).HasColumnType("datetime");

                entity.Property(e => e.Name).HasMaxLength(50);
            });

            modelBuilder.Entity<_MonthData>(entity =>
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

            modelBuilder.Entity<_Prices>(entity =>
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
