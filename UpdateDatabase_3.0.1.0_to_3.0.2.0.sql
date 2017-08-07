Use [arachnode.net]

UPDATE cfg.[Version] Set Value = '3.0.2.0'

USE [arachnode.net]
GO

/****** Object:  Table [dbo].[BusinessInformation]    Script Date: 05/14/2013 07:56:25 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[BusinessInformation](
	[WebPageID] [bigint] NOT NULL,
	[Name] [varchar](255) NULL,
	[Address1] [varchar](255) NULL,
	[Address2] [varchar](255) NULL,
	[City] [varchar](255) NULL,
	[State] [varchar](255) NULL,
	[Zip] [varchar](255) NULL,
	[PhoneNumber] [varchar](255) NULL,
	[Category] [varchar](255) NULL,
	[Latitude] [varchar](255) NULL,
	[Longitude] [varchar](255) NULL,
 CONSTRAINT [PK_BusinessInformation] PRIMARY KEY CLUSTERED 
(
	[WebPageID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[BusinessInformation]  WITH CHECK ADD  CONSTRAINT [FK_BusinessInformation_WebPages] FOREIGN KEY([WebPageID])
REFERENCES [dbo].[WebPages] ([ID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[BusinessInformation] CHECK CONSTRAINT [FK_BusinessInformation_WebPages]
GO



GO