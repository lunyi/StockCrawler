Scaffold-DbContext "Server=220.133.185.1;Database=StockDb;User ID=stock;Password=stock;" Microsoft.EntityFrameworkCore.SqlServer -OutputDir Models -Force -UseDatabaseNames


Scaffold-DbContext "Server=localhost;Port=5433;Database=stock;User ID=postgres;Password=1q2w3e4r;" Npgsql.EntityFrameworkCore.PostgreSQL -OutputDir Models -Force -UseDatabaseNames
