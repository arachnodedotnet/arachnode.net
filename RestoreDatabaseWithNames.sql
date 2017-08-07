--RESTORE FILELISTONLY FROM DISK = 'C:\Databases\arachnode.net.bak'
--RESTORE DATABASE [arachnode.net] FROM  DISK = N'S:\Applications\arachnode.net\arachnode.net.bak' WITH  FILE = 2,  NOUNLOAD,  STATS = 10

RESTORE DATABASE [arachnode.net] FROM  DISK = N'S:\Databases\arachnode.net.bak' WITH FILE = 1,
MOVE N'arachnode.net_dat' TO N'S:\Databases\arachnode.net.mdf',
MOVE N'arachnode.net_BusinessInformation' TO N'S:\Databases\arachnode.net_BusinessInformation.ndf',
MOVE N'arachnode.net_Configuration' TO N'S:\Databases\arachnode.net_Configuration.ndf',
MOVE N'arachnode.net_CrawlRequests' TO N'S:\Databases\arachnode.net_CrawlRequests.ndf',
MOVE N'arachnode.net_DisallowedAbsoluteUris' TO N'S:\Databases\arachnode.net_DisallowedAbsoluteUris.ndf',
MOVE N'arachnode.net_DisallowedAbsoluteUris_Discoveries' TO N'S:\Databases\arachnode.net_DisallowedAbsoluteUris_Discoveries.ndf',
MOVE N'arachnode.net_Discoveries' TO N'S:\Databases\arachnode.net_Discoveries.ndf',
MOVE N'arachnode.net_Documents' TO N'S:\Databases\arachnode.net_Documents.ndf',
MOVE N'arachnode.net_EmailAddresses' TO N'S:\Databases\arachnode.net_EmailAddresses.ndf',
MOVE N'arachnode.net_EmailAddresses_Discoveries' TO N'S:\Databases\arachnode.net_EmailAddresses_Discoveries.ndf',
MOVE N'arachnode.net_Exceptions' TO N'S:\Databases\arachnode.net_Exceptions.ndf',
MOVE N'arachnode.net_Files' TO N'S:\Databases\arachnode.net_Files.ndf',
MOVE N'arachnode.net_Files_Discoveries' TO N'S:\Databases\arachnode.net_Files_Discoveries.ndf',
MOVE N'arachnode.net_Files_MetaData' TO N'S:\Databases\arachnode.net_Files_MetaData.ndf',
MOVE N'arachnode.net_HyperLinks' TO N'S:\Databases\arachnode.net_HyperLinks.ndf',
MOVE N'arachnode.net_HyperLinks_Discoveries' TO N'S:\Databases\arachnode.net_HyperLinks_Discoveries.ndf',
MOVE N'arachnode.net_Reporting' TO N'S:\Databases\arachnode.net_Reporting.ndf',
MOVE N'arachnode.net_Images' TO N'S:\Databases\arachnode.net_Images.ndf',
MOVE N'arachnode.net_Images_Discoveries' TO N'S:\Databases\arachnode.net_Images_Discoveries.ndf',
MOVE N'arachnode.net_Images_MetaData' TO N'S:\Databases\arachnode.net_Images_MetaData.ndf',
MOVE N'arachnode.net_WebPages' TO N'S:\Databases\arachnode.net_WebPages.ndf',
MOVE N'arachnode.net_WebPages_MetaData' TO N'S:\Databases\arachnode.net_WebPages_MetaData.ndf',
MOVE N'ftrow_arachnode.net_ftc_Files' TO N'S:\Databases\arachnode.net_ftc_Files.ndf',
MOVE N'ftrow_arachnode.net_ftc_WebPages' TO N'S:\Databases\arachnode.net_ftc_WebPages.ndf',
MOVE N'ftrow_arachnode.net_ftc_WebPages_MetaData' TO N'S:\Databases\arachnode.net_ftc_WebPages_MetaData.ndf',
MOVE N'ftrow_arachnode.net_ftc_Files_MetaData' TO N'S:\Databases\arachnode.net_ftc_Files_MetaData.ndf',
MOVE N'ftrow_arachnode.net_ftc_Images' TO N'S:\Databases\arachnode.net_ftc_Images.ndf',
MOVE N'ftrow_arachnode.net_ftc_Images_MetaData' TO N'S:\Databases\arachnode.net_ftc_Images_MetaData.ndf',
MOVE N'arachnode.net_log' TO N'S:\Databases\arachnode.net.ldf', NOUNLOAD, REPLACE, STATS = 10
GO

USE [arachnode.net]

EXEC [dbo].[arachnode_usp_arachnode.net_RESET_DATABASE]
GO

--Change Autogrowth Settings... (the AN DB expands at 1024MB per AutoGrowth event)
--ALTER DATABASE [arachnode.net] MODIFY FILE (NAME='arachnode.net_dat', FILEGROWTH=100MB);
--ALTER DATABASE [arachnode.net] MODIFY FILE (NAME='arachnode.net_CrawlRequests', FILEGROWTH=100MB);
--ALTER DATABASE [arachnode.net] MODIFY FILE (NAME='arachnode.net_EmailAddresses', FILEGROWTH=100MB);
--ALTER DATABASE [arachnode.net] MODIFY FILE (NAME='arachnode.net_Exceptions', FILEGROWTH=100MB);
--ALTER DATABASE [arachnode.net] MODIFY FILE (NAME='arachnode.net_Files', FILEGROWTH=100MB);
--ALTER DATABASE [arachnode.net] MODIFY FILE (NAME='arachnode.net_HyperLinks', FILEGROWTH=100MB);
--ALTER DATABASE [arachnode.net] MODIFY FILE (NAME='arachnode.net_WebPages', FILEGROWTH=100MB);
--ALTER DATABASE [arachnode.net] MODIFY FILE (NAME='arachnode.net_Files_MetaData', FILEGROWTH=100MB);
--ALTER DATABASE [arachnode.net] MODIFY FILE (NAME='arachnode.net_WebPages_MetaData', FILEGROWTH=100MB);
--ALTER DATABASE [arachnode.net] MODIFY FILE (NAME='arachnode.net_Reporting', FILEGROWTH=100MB);
--ALTER DATABASE [arachnode.net] MODIFY FILE (NAME='arachnode.net_Cache', FILEGROWTH=100MB);
--ALTER DATABASE [arachnode.net] MODIFY FILE (NAME='arachnode.net_DisallowedAbsoluteUris', FILEGROWTH=100MB);
--ALTER DATABASE [arachnode.net] MODIFY FILE (NAME='arachnode.net_Images', FILEGROWTH=100MB);
--ALTER DATABASE [arachnode.net] MODIFY FILE (NAME='arachnode.net_Images_MetaData', FILEGROWTH=100MB);
--ALTER DATABASE [arachnode.net] MODIFY FILE (NAME='ftrow_arachnode.net_ftc_Files', FILEGROWTH=100MB);
--ALTER DATABASE [arachnode.net] MODIFY FILE (NAME='ftrow_arachnode.net_ftc_WebPages', FILEGROWTH=100MB);
--ALTER DATABASE [arachnode.net] MODIFY FILE (NAME='ftrow_arachnode.net_ftc_WebPages_MetaData', FILEGROWTH=100MB);
--ALTER DATABASE [arachnode.net] MODIFY FILE (NAME='ftrow_arachnode.net_ftc_Files_MetaData', FILEGROWTH=100MB);
--ALTER DATABASE [arachnode.net] MODIFY FILE (NAME='ftrow_arachnode.net_ftc_Images', FILEGROWTH=100MB);
--ALTER DATABASE [arachnode.net] MODIFY FILE (NAME='ftrow_arachnode.net_ftc_Images_MetaData', FILEGROWTH=100MB);
--ALTER DATABASE [arachnode.net] MODIFY FILE (NAME='arachnode.net_Documents', FILEGROWTH=100MB);
--ALTER DATABASE [arachnode.net] MODIFY FILE (NAME='arachnode.net_Configuration', FILEGROWTH=100MB);
--ALTER DATABASE [arachnode.net] MODIFY FILE (NAME='arachnode.net_log', FILEGROWTH=100MB);

--If you ever need to move a file/filegroup...
--ALTER DATABASE [arachnode.net] SET OFFLINE;

--ALTER DATABASE [arachnode.net] MODIFY FILE ( NAME = [arachnode.net_dat], FILENAME = 'C:\Databases\arachnode.net.mdf'
--ALTER DATABASE [arachnode.net] MODIFY FILE ( NAME = [arachnode.net_log], FILENAME = 'C:\Databases\arachnode.net.ldf'
--ALTER DATABASE [arachnode.net] MODIFY FILE ( NAME = [arachnode.net_CrawlRequests], FILENAME = 'C:\Databases\arachnode.net_CrawlRequests.ndf'
--ALTER DATABASE [arachnode.net] MODIFY FILE ( NAME = [arachnode.net_EmailAddresses], FILENAME = 'C:\Databases\arachnode.net_EmailAddresses.ndf'
--ALTER DATABASE [arachnode.net] MODIFY FILE ( NAME = [arachnode.net_Exceptions], FILENAME = 'C:\Databases\arachnode.net_Exceptions.ndf'
--ALTER DATABASE [arachnode.net] MODIFY FILE ( NAME = [arachnode.net_Files], FILENAME = 'C:\Databases\arachnode.net_Files.ndf'
--ALTER DATABASE [arachnode.net] MODIFY FILE ( NAME = [arachnode.net_HyperLinks], FILENAME = 'C:\Databases\arachnode.net_HyperLinks.ndf'
--ALTER DATABASE [arachnode.net] MODIFY FILE ( NAME = [arachnode.net_WebPages], FILENAME = 'C:\Databases\arachnode.net_WebPages.ndf'
--ALTER DATABASE [arachnode.net] MODIFY FILE ( NAME = [arachnode.net_Files_MetaData], FILENAME = 'C:\Databases\arachnode.net_Files_MetaData.ndf'
--ALTER DATABASE [arachnode.net] MODIFY FILE ( NAME = [arachnode.net_WebPages_MetaData], FILENAME = 'C:\Databases\arachnode.net_WebPages_MetaData.ndf'
--ALTER DATABASE [arachnode.net] MODIFY FILE ( NAME = [arachnode.net_Cache], FILENAME = 'C:\Databases\arachnode.net_Cache.ndf'
--ALTER DATABASE [arachnode.net] MODIFY FILE ( NAME = [arachnode.net_Reporting], FILENAME = 'C:\Databases\arachnode.net_Reporting.ndf'
--ALTER DATABASE [arachnode.net] MODIFY FILE ( NAME = [arachnode.net_DisallowedAbsoluteUris], FILENAME = 'C:\Databases\arachnode.net_DisallowedAbsoluteUris.ndf'
--ALTER DATABASE [arachnode.net] MODIFY FILE ( NAME = [arachnode.net_Images], FILENAME = 'C:\Databases\arachnode.net_Images.ndf'
--ALTER DATABASE [arachnode.net] MODIFY FILE ( NAME = [arachnode.net_Images_MetaData], FILENAME = 'C:\Databases\arachnode.net_Images_MetaData.ndf'
--ALTER DATABASE [arachnode.net] MODIFY FILE ( NAME = [ftrow_arachnode.net_ftc_Files], FILENAME = 'C:\Databases\arachnode.net_ftc_Files.ndf'
--ALTER DATABASE [arachnode.net] MODIFY FILE ( NAME = [ftrow_arachnode.net_ftc_WebPages], FILENAME = 'C:\Databases\arachnode.net_ftc_WebPages.ndf'
--ALTER DATABASE [arachnode.net] MODIFY FILE ( NAME = [ftrow_arachnode.net_ftc_WebPages_MetaData], FILENAME = 'C:\Databases\arachnode.net_ftc_WebPages_MetaData.ndf'
--ALTER DATABASE [arachnode.net] MODIFY FILE ( NAME = [ftrow_arachnode.net_ftc_Files_MetaData], FILENAME = 'C:\Databases\arachnode.net_ftc_Files_MetaData.ndf'
--ALTER DATABASE [arachnode.net] MODIFY FILE ( NAME = [ftrow_arachnode.net_ftc_Images], FILENAME = 'C:\Databases\arachnode.net_ftc_Images.ndf'
--ALTER DATABASE [arachnode.net] MODIFY FILE ( NAME = [ftrow_arachnode.net_ftc_Images_MetaData], FILENAME = 'C:\Databases\arachnode.net_ftc_Images_MetaData.ndf'
--ALTER DATABASE [arachnode.net] MODIFY FILE ( NAME = [arachnode.net_Documents], FILENAME = 'C:\Databases\arachnode.net_Documents.ndf'
--ALTER DATABASE [arachnode.net] MODIFY FILE ( NAME = [arachnode.net_Configuration], FILENAME = 'C:\Databases\arachnode.net_Configuration.ndf'

--ALTER DATABASE [arachnode.net] SET ONLINE;