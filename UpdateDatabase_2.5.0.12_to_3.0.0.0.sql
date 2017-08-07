Use [arachnode.net]

UPDATE cfg.[Version] Set Value = '3.0.0.0'

UPDATE cfg.CrawlActions Set AssemblyName = REPLACE(AssemblyName, 'Arachnode.SiteCrawler', 'Arachnode.Plugins')
UPDATE cfg.CrawlRules Set AssemblyName = REPLACE(AssemblyName, 'Arachnode.SiteCrawler', 'Arachnode.Plugins')

UPDATE cfg.CrawlActions Set TypeName = REPLACE(TypeName, 'Arachnode.SiteCrawler.Actions', 'Arachnode.Plugins.CrawlActions')
UPDATE cfg.CrawlRules Set TypeName = REPLACE(TypeName, 'Arachnode.SiteCrawler.Rules', 'Arachnode.Plugins.CrawlRules')

Select * From cfg.CrawlActions
Select * From cfg.CrawlRules

EXEC dbo.sp_fulltext_table @tabname=N'[dbo].[WebPages]', @action=N'deactivate'
GO