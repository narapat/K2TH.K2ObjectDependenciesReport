USE [K2]
GO

/****** Object:  Table [dbo].[FormProcessDependency]    Script Date: 9/9/2014 10:51:13 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[FormProcessDependency](
       [FormProcessDependency] [int] IDENTITY(1,1) NOT NULL,
       [FormGUID] [uniqueidentifier] NULL,
       [ProcessName] [nvarchar](500) NULL,
CONSTRAINT [PK_FormProcessDependency] PRIMARY KEY CLUSTERED 
(
       [FormProcessDependency] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


USE [K2]
GO

/****** Object:  Table [dbo].[FormViewDependency]    Script Date: 9/9/2014 10:51:25 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[FormViewDependency](
       [FormViewDependencyID] [int] IDENTITY(1,1) NOT NULL,
       [FormGUID] [uniqueidentifier] NULL,
       [ViewGUID] [uniqueidentifier] NULL,
CONSTRAINT [PK_FormViewDependency] PRIMARY KEY CLUSTERED 
(
       [FormViewDependencyID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

USE [K2]
GO

/****** Object:  Table [dbo].[ViewSMODependency]    Script Date: 9/9/2014 10:51:38 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ViewSMODependency](
       [ViewSMODependencyID] [int] IDENTITY(1,1) NOT NULL,
       [ViewGUID] [uniqueidentifier] NULL,
       [SmartObjectGUID] [uniqueidentifier] NULL,
CONSTRAINT [PK_ViewSMODependency] PRIMARY KEY CLUSTERED 
(
       [ViewSMODependencyID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


USE [K2]
GO

/****** Object:  View [dbo].[vwObjectDependency]    Script Date: 9/9/2014 10:52:37 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[vwObjectDependency]
AS
SELECT DISTINCT f.DisplayName AS Form, v.DisplayName AS [View], so.SystemName AS SmartObject, fpd.ProcessName AS Process
FROM            K2HostServer.Form.Form AS f INNER JOIN
                         dbo.FormViewDependency AS fvd ON fvd.FormGUID = f.ID INNER JOIN
                         K2HostServer.Form.[View] AS v ON fvd.ViewGUID = v.ID INNER JOIN
                         dbo.ViewSMODependency AS vsd ON vsd.ViewGUID = v.ID INNER JOIN
                         dbo.SmartObject AS so ON vsd.SmartObjectGUID = so.SmartObjectGUID INNER JOIN
                         dbo.FormProcessDependency AS fpd ON fpd.FormGUID = f.ID

GO
