/****** SSMS 中 SelectTopNRows 命令的指令碼  ******/


select * from Stocks where StockId in (

select a.[StockId] from ( SELECT 
     [StockId]
       ,[Name]
  FROM [StockDb].[dbo].[Remarks]
  where Remark = '很好'  or Remark = '不錯') a

join (
	select  
	a.StockId,
	a.Name 
	from (
		SELECT 
			  c1.[StockId]
			  ,c1.[Name]
			  ,c1.[Type],
			  COUNT(c1.Pass) as TotalCount,
			  COUNT(c2.Pass) as Pass
		  FROM [dbo].[Checks] c1
		  left join (select * from [dbo].[Checks] where Pass = 1) c2 
		  on c1.Id = c2.Id
		  --where 
		  --c2.[Pass] = 1
		  group by  
			  c1.[StockId]
			  ,c1.[Name],
			  c1.[Type] 
	) a where a.Pass >= 3

	group by 
	a.StockId,
	a.Name
	having count(Pass) = 4
) b
on a.StockId = b.StockId 
)

order by Industry



select  
a.StockId,
a.Name ,
count(Pass)
from (
	SELECT 
		  c1.[StockId]
		  ,c1.[Name]
		  ,c1.[Type],
		  COUNT(c1.Pass) as TotalCount,
		  COUNT(c2.Pass) as Pass
	  FROM [dbo].[Checks] c1
	  left join (select * from [dbo].[Checks] where Pass = 1) c2 
	  on c1.Id = c2.Id
	  --where 
	  --c2.[Pass] = 1
	  group by  
		  c1.[StockId]
		  ,c1.[Name],
		  c1.[Type] 
) a where a.Pass >= 3

group by 
a.StockId,
a.Name
having count(Pass) = 4
order by count(Pass) desc, a.StockId



 insert [dbo].[BestStocks] 
	  ([Id]
      ,[StockId]
      ,[Name]
      ,[Type]
      ,[Description]
      ,[CreatedOn])
   select 
		newid(),
		StockId,
		[Name],
		'型態選股',
		'型態選股',
		GETDATE()
   from [Stocks] 
   where StockId in (
   '2477',
   '6531',
   '3530',
   '6153',
   '6155',
   '5469',
   '3305',
   '3031',
   '2505',
   '8478'
   )