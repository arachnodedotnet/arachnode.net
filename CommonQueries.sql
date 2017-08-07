--Common Tasks...
--[dbo].[arachnode_usp_arachnode.net_RESET_DATABASE]

Select * From Files
Select * From Images
Select * From WebPages

Select * From HyperLinks

--Troubleshooting...
Select Top 1000 * From Exceptions Order by Created DESC
Select * From DisallowedAbsoluteUris

--Crawl state (where the crawl is currently at, or after pressing CTRL-C)...
Select * From CrawlRequests
Select * From Discoveries

--Reset the Crawler state...
--Delete From Discoveries
--Delete From CrawlRequests

--Check Plugin Configs...
Select * From cfg.CrawlActions
Select * From cfg.CrawlRules

--Check tables corresponding to 'ClassifyAbsoluteUris'
Select * From [Domains]
Select * From Hosts

--Table Row Counts
--all tables rowcount
    SELECT DISTINCT
            ( t.object_id ),
            t.name,
            i.rows
    from    sys.tables t (NOLOCK)
            join sys.sysindexes i (NOLOCK) on t.object_id = i.id
    where   t.type_desc = 'USER_TABLE'
            and i.root is not null
            and t.name <> 'sysdiagrams'
    order by i.rows DESC, t.name ASC
    
--osql -E -q "ALTER DATABASE [arachnode.net] SET TRUSTWORTHY ON"
--osql -E -q "ALTER AUTHORIZATION ON DATABASE::[arachnode.net] TO [sa]"
--osql -E -q "EXEC sp_configure 'clr enabled', 1"
--osql -E -q "RECONFIGURE"