/**/

--This should be run when it appears that arachnode.net (using MS-SQL) is slowing down.  MS-SQL indexes fragment naturally as the B-Trees expand.  As the AN databases grow in size the need to optimize the indexes lessens.

PRINT 'Optimizing Arachnode Data ...'
EXEC sp_updatestats
DBCC UPDATEUSAGE ('arachnode.net') WITH COUNT_ROWS

--Examine index fragmentation.
    SELECT  a.index_id,
            [name],
            avg_fragmentation_in_percent
    FROM    sys.dm_db_index_physical_stats(DB_ID(),
                                           OBJECT_ID(N'Production.Product'),
                                           NULL, NULL, NULL) AS a
            INNER JOIN sys.indexes AS b (NOLOCK) ON a.object_id = b.object_id
                                           AND a.index_id = b.index_id
    ORDER BY avg_fragmentation_in_percent DESC

PRINT 'Optimizing Arachnode Data ...'
EXEC sp_updatestats
DBCC UPDATEUSAGE ('arachnode.net') WITH COUNT_ROWS

--REORGANIZE COMMON/CORE...
    
/*DBCC DBREINDEX ('dbo.CrawlRequests')*/  ALTER INDEX ALL ON dbo.CrawlRequests REORGANIZE
/*DBCC DBREINDEX ('dbo.DisallowedAbsoluteUris')*/  ALTER INDEX ALL ON dbo.DisallowedAbsoluteUris REORGANIZE
/*DBCC DBREINDEX ('dbo.DisallowedAbsoluteUris_Discoveries')*/  ALTER INDEX ALL ON dbo.DisallowedAbsoluteUris_Discoveries REORGANIZE
/*DBCC DBREINDEX ('dbo.Discoveries')*/  ALTER INDEX ALL ON dbo.Discoveries REORGANIZE
/*DBCC DBREINDEX ('dbo.EmailAddresses')*/  ALTER INDEX ALL ON dbo.EmailAddresses REORGANIZE
/*DBCC DBREINDEX ('dbo.EmailAddresses_Discoveries')*/  ALTER INDEX ALL ON dbo.EmailAddresses_Discoveries REORGANIZE
/*DBCC DBREINDEX ('dbo.Exceptions')*/  ALTER INDEX ALL ON dbo.Exceptions REORGANIZE
/*DBCC DBREINDEX ('dbo.Files')*/  ALTER INDEX ALL ON dbo.Files REORGANIZE
/*DBCC DBREINDEX ('dbo.Files_Discoveries')*/  ALTER INDEX ALL ON dbo.Files_Discoveries REORGANIZE
/*DBCC DBREINDEX ('dbo.Files_MetaData')*/  ALTER INDEX ALL ON dbo.Files_MetaData REORGANIZE
/*DBCC DBREINDEX ('dbo.HyperLinks')*/  ALTER INDEX ALL ON dbo.HyperLinks REORGANIZE
/*DBCC DBREINDEX ('dbo.HyperLinks_Discoveries')*/  ALTER INDEX ALL ON dbo.HyperLinks_Discoveries REORGANIZE
/*DBCC DBREINDEX ('dbo.Images')*/  ALTER INDEX ALL ON dbo.Images REORGANIZE
/*DBCC DBREINDEX ('dbo.Images_Discoveries')*/  ALTER INDEX ALL ON dbo.Images_Discoveries REORGANIZE
/*DBCC DBREINDEX ('dbo.Image_MetaData')*/  ALTER INDEX ALL ON dbo.Images_MetaData REORGANIZE
/*DBCC DBREINDEX ('dbo.WebPages')*/  ALTER INDEX ALL ON dbo.WebPages REORGANIZE
/*DBCC DBREINDEX ('dbo.WebPages_MetaData')*/  ALTER INDEX ALL ON dbo.WebPages_MetaData REORGANIZE

--REBUILD COMMON/CORE...

/*DBCC DBREINDEX ('dbo.CrawlRequests')*/  /*ALTER INDEX ALL ON dbo.CrawlRequests REORGANIZE*/      ALTER INDEX ALL ON dbo.CrawlRequests REBUILD          WITH (               FILLFACTOR =               10,               PAD_INDEX =               ON,               SORT_IN_TEMPDB =               ON)  /*DBCC SHOWCONTIG ('dbo.CrawlRequests')*/  
/*DBCC DBREINDEX ('dbo.DisallowedAbsoluteUris')*/  /*ALTER INDEX ALL ON dbo.DisallowedAbsoluteUris REORGANIZE*/      ALTER INDEX ALL ON dbo.DisallowedAbsoluteUris REBUILD          WITH (               FILLFACTOR =               10,               PAD_INDEX =               ON,               SORT_IN_TEMPDB =               ON)  /*DBCC SHOWCONTIG ('dbo.DisallowedAbsoluteUris')*/  
/*DBCC DBREINDEX ('dbo.DisallowedAbsoluteUris_Discoveries')*/  /*ALTER INDEX ALL ON dbo.DisallowedAbsoluteUris_Discoveries REORGANIZE*/      ALTER INDEX ALL ON dbo.DisallowedAbsoluteUris_Discoveries REBUILD          WITH (               FILLFACTOR =               10,               PAD_INDEX =               ON,               SORT_IN_TEMPDB =               ON)  /*DBCC SHOWCONTIG ('dbo.DisallowedAbsoluteUris_Discoveries')*/  
/*DBCC DBREINDEX ('dbo.Discoveries')*/  /*ALTER INDEX ALL ON dbo.Discoveries REORGANIZE*/      ALTER INDEX ALL ON dbo.Discoveries REBUILD          WITH (               FILLFACTOR =               10,               PAD_INDEX =               ON,               SORT_IN_TEMPDB =               ON)  /*DBCC SHOWCONTIG ('dbo.Discoveries')*/  
/*DBCC DBREINDEX ('dbo.EmailAddresses')*/  /*ALTER INDEX ALL ON dbo.EmailAddresses REORGANIZE*/      ALTER INDEX ALL ON dbo.EmailAddresses REBUILD          WITH (               FILLFACTOR =               10,               PAD_INDEX =               ON,               SORT_IN_TEMPDB =               ON)  /*DBCC SHOWCONTIG ('dbo.EmailAddresses')*/  
/*DBCC DBREINDEX ('dbo.EmailAddresses_Discoveries')*/  /*ALTER INDEX ALL ON dbo.EmailAddresses_Discoveries REORGANIZE*/      ALTER INDEX ALL ON dbo.EmailAddresses_Discoveries REBUILD          WITH (               FILLFACTOR =               10,               PAD_INDEX =               ON,               SORT_IN_TEMPDB =               ON)  /*DBCC SHOWCONTIG ('dbo.EmailAddresses_Discoveries')*/  
/*DBCC DBREINDEX ('dbo.Exceptions')*/  /*ALTER INDEX ALL ON dbo.Exceptions REORGANIZE*/      ALTER INDEX ALL ON dbo.Exceptions REBUILD          WITH (               FILLFACTOR =               10,               PAD_INDEX =               ON,               SORT_IN_TEMPDB =               ON)  /*DBCC SHOWCONTIG ('dbo.Exceptions')*/  
/*DBCC DBREINDEX ('dbo.Files')*/  /*ALTER INDEX ALL ON dbo.Files REORGANIZE*/      ALTER INDEX ALL ON dbo.Files REBUILD          WITH (               FILLFACTOR =               10,               PAD_INDEX =               ON,               SORT_IN_TEMPDB =               ON)  /*DBCC SHOWCONTIG ('dbo.Files')*/  
/*DBCC DBREINDEX ('dbo.Files_Discoveries')*/  /*ALTER INDEX ALL ON dbo.Files_Discoveries REORGANIZE*/      ALTER INDEX ALL ON dbo.Files_Discoveries REBUILD          WITH (               FILLFACTOR =               10,               PAD_INDEX =               ON,               SORT_IN_TEMPDB =               ON)  /*DBCC SHOWCONTIG ('dbo.Files_Discoveries')*/  
/*DBCC DBREINDEX ('dbo.Files_MetaData')*/  /*ALTER INDEX ALL ON dbo.Files_MetaData REORGANIZE*/      ALTER INDEX ALL ON dbo.Files_MetaData REBUILD          WITH (               FILLFACTOR =               10,               PAD_INDEX =               ON,               SORT_IN_TEMPDB =               ON)  /*DBCC SHOWCONTIG ('dbo.Files_MetaData')*/  
/*DBCC DBREINDEX ('dbo.HyperLinks')*/  /*ALTER INDEX ALL ON dbo.HyperLinks REORGANIZE*/      ALTER INDEX ALL ON dbo.HyperLinks REBUILD          WITH (               FILLFACTOR =               10,               PAD_INDEX =               ON,               SORT_IN_TEMPDB =               ON)  /*DBCC SHOWCONTIG ('dbo.HyperLinks')*/  
/*DBCC DBREINDEX ('dbo.HyperLinks_Discoveries')*/  /*ALTER INDEX ALL ON dbo.HyperLinks_Discoveries REORGANIZE*/      ALTER INDEX ALL ON dbo.HyperLinks_Discoveries REBUILD          WITH (               FILLFACTOR =               10,               PAD_INDEX =               ON,               SORT_IN_TEMPDB =               ON)  /*DBCC SHOWCONTIG ('dbo.HyperLinks_Discoveries')*/  
/*DBCC DBREINDEX ('dbo.Images')*/  /*ALTER INDEX ALL ON dbo.Images REORGANIZE*/      ALTER INDEX ALL ON dbo.Images REBUILD          WITH (               FILLFACTOR =               10,               PAD_INDEX =               ON,               SORT_IN_TEMPDB =               ON)  /*DBCC SHOWCONTIG ('dbo.Images')*/  
/*DBCC DBREINDEX ('dbo.Images_MetaData')*/  /*ALTER INDEX ALL ON dbo.Images_MetaData REORGANIZE*/      ALTER INDEX ALL ON dbo.Images_MetaData REBUILD          WITH (               FILLFACTOR =               10,               PAD_INDEX =               ON,               SORT_IN_TEMPDB =               ON)  /*DBCC SHOWCONTIG ('dbo.Images_MetaData')*/  
/*DBCC DBREINDEX ('dbo.Images_Discoveries')*/  /*ALTER INDEX ALL ON dbo.Images_Discoveries REORGANIZE*/      ALTER INDEX ALL ON dbo.Images_Discoveries REBUILD          WITH (               FILLFACTOR =               10,               PAD_INDEX =               ON,               SORT_IN_TEMPDB =               ON)  /*DBCC SHOWCONTIG ('dbo.Images_Discoveries')*/  
/*DBCC DBREINDEX ('dbo.WebPages')*/  /*ALTER INDEX ALL ON dbo.WebPages REORGANIZE*/      ALTER INDEX ALL ON dbo.WebPages REBUILD          WITH (               FILLFACTOR =               10,               PAD_INDEX =               ON,               SORT_IN_TEMPDB =               ON)  /*DBCC SHOWCONTIG ('dbo.WebPages')*/  
/*DBCC DBREINDEX ('dbo.WebPages_MetaData')*/  /*ALTER INDEX ALL ON dbo.WebPages REORGANIZE*/      ALTER INDEX ALL ON dbo.WebPages REBUILD          WITH (               FILLFACTOR =               10,               PAD_INDEX =               ON,               SORT_IN_TEMPDB =               ON)  /*DBCC SHOWCONTIG ('dbo.WebPages_MetaData')*/  

PRINT 'Optimizing Arachnode Data ...'
EXEC sp_updatestats
DBCC UPDATEUSAGE ('arachnode.net') WITH COUNT_ROWS

--Examine index fragmentation.
    SELECT  a.index_id,
            [name],
            avg_fragmentation_in_percent
    FROM    sys.dm_db_index_physical_stats(DB_ID(),
                                           OBJECT_ID(N'Production.Product'),
                                           NULL, NULL, NULL) AS a
            INNER JOIN sys.indexes AS b (NOLOCK) ON a.object_id = b.object_id
                                           AND a.index_id = b.index_id
    ORDER BY avg_fragmentation_in_percent DESC   

--Examine index fragmentation / generate REORGANIZE/REBUILD queries.
SELECT  a.index_id,
		t.[name],
		i.[name],
		avg_fragmentation_in_percent, 
		'ALTER INDEX [' + i.[name] + '] ON [' + OBJECT_SCHEMA_NAME(a.object_id) + '].[' + t.[name] + '] REORGANIZE',
		'ALTER INDEX ALL ON [' + OBJECT_SCHEMA_NAME(a.object_id) + '].[' + t.[name] + '] REBUILD          WITH (               FILLFACTOR =               10,               PAD_INDEX =               ON,               SORT_IN_TEMPDB =               ON)'
FROM    sys.dm_db_index_physical_stats(DB_ID(),
									   OBJECT_ID(N'Production.Product'),
									   NULL, NULL, NULL) AS a                                           
		INNER JOIN sys.indexes (NOLOCK) AS i ON a.object_id = i.object_id
		INNER JOIN sys.tables (NOLOCK) AS t ON i.object_id = t.object_id
									   AND a.index_id = i.index_id
WHERE avg_fragmentation_in_percent >= 10
AND OBJECT_SCHEMA_NAME(a.object_id) <> 'cfg'
AND i.Name is not NULL
ORDER BY avg_fragmentation_in_percent DESC   
 