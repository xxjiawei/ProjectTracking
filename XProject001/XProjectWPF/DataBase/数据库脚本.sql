/*    ==脚本参数==

    源服务器版本 : SQL Server 2016 (13.0.4001)
    源数据库引擎版本 : Microsoft SQL Server Enterprise Edition
    源数据库引擎类型 : 独立的 SQL Server

    目标服务器版本 : SQL Server 2016
    目标数据库引擎版本 : Microsoft SQL Server Enterprise Edition
    目标数据库引擎类型 : 独立的 SQL Server
*/

USE [ProjectTracking]
GO

/****** Object:  Table [dbo].[PT_B_Project]    Script Date: 2018/1/21 23:51:54 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[PT_B_Project](
	[Project_No] [varchar](20) NOT NULL,
	[Quotation_No] [varchar](20) NULL,
	[Follow_Man] [varchar](50) NULL,
	[Project_Type] [varchar](4) NULL,
	[Company_Name] [varchar](150) NULL,
	[Contact_Man] [varchar](50) NULL,
	[Tel] [varchar](50) NULL,
	[Company_Address] [varchar](400) NULL,
	[Fax] [varchar](50) NULL,
	[Email] [varchar](50) NULL,
	[Project_Name] [varchar](500) NULL,
	[Product_Model] [varchar](100) NULL,
	[Cycle_Time] [varchar](50) NULL,
	[Price] [varchar](50) NULL,
	[Is_Tax] [varchar](4) NULL,
	[Quotation_Date] [datetime] NULL,
	[Account_Receivable] [varchar](50) NULL,
	[Payment_Receivable] [varchar](50) NULL,
	[Un_Account_Receivable] [varchar](50) NULL,
	[Agency_Account_Payable] [varchar](50) NULL,
	[Agency_Accounts_Prepaid] [varchar](50) NULL,
	[Un_Agency_Account_Payable] [varchar](50) NULL,
	[Lab_Account_Payable] [varchar](50) NULL,
	[Lab_Accounts_Prepaid] [varchar](50) NULL,
	[Un_Lab_Account_Payable] [varchar](50) NULL,
	[Other_Account] [varchar](50) NULL,
	[Other_Pad_Account] [varchar](50) NULL,
	[Un_Other_Account] [varchar](50) NULL,
	[Profits] [varchar](50) NOT NULL,
	[Now_Profits] [varchar](50) NULL,
	[Pads_Money] [varchar](50) NULL,
	[Is_Pads] [varchar](4) NULL,
	[Is_All_Customer] [varchar](4) NULL,
	[Is_All_Agency] [varchar](4) NULL,
	[Is_All_Lab] [varchar](4) NULL,
	[Is_All_Other] [varchar](4) NULL,
	[Remark] [varchar](400) NULL,
	[Oper_Time] [datetime] NULL,
	[Bill_Status] [varchar](2) NULL,
 CONSTRAINT [PK_PT_B_Project_1] PRIMARY KEY CLUSTERED 
(
	[Project_No] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


/*    ==脚本参数==

    源服务器版本 : SQL Server 2016 (13.0.4001)
    源数据库引擎版本 : Microsoft SQL Server Enterprise Edition
    源数据库引擎类型 : 独立的 SQL Server

    目标服务器版本 : SQL Server 2016
    目标数据库引擎版本 : Microsoft SQL Server Enterprise Edition
    目标数据库引擎类型 : 独立的 SQL Server
*/

USE [ProjectTracking]
GO

/****** Object:  Table [dbo].[PT_B_Project_Agency]    Script Date: 2018/1/21 23:52:28 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[PT_B_Project_Agency](
	[Agency_Pays_Id] [varchar](32) NOT NULL,
	[Project_No] [varchar](20) NULL,
	[Seq_No] [int] NULL,
	[Agency_Money] [varchar](50) NULL,
	[Agency] [varchar](50) NULL,
	[Agency_Remark] [varchar](500) NULL,
	[Agency_Date] [datetime] NULL,
	[Is_Agency_Inv] [varchar](4) NULL,
	[Agency_Inv_Price] [varchar](50) NULL,
	[Agency_Inv_No] [varchar](50) NULL,
	[Agency_Inv_Date] [datetime] NULL,
 CONSTRAINT [PK_PT_B_Project_Agency] PRIMARY KEY CLUSTERED 
(
	[Agency_Pays_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


/*    ==脚本参数==

    源服务器版本 : SQL Server 2016 (13.0.4001)
    源数据库引擎版本 : Microsoft SQL Server Enterprise Edition
    源数据库引擎类型 : 独立的 SQL Server

    目标服务器版本 : SQL Server 2016
    目标数据库引擎版本 : Microsoft SQL Server Enterprise Edition
    目标数据库引擎类型 : 独立的 SQL Server
*/

USE [ProjectTracking]
GO

/****** Object:  Table [dbo].[PT_B_Project_Customer]    Script Date: 2018/1/21 23:52:37 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[PT_B_Project_Customer](
	[Customer_Pays_Id] [varchar](32) NOT NULL,
	[Project_No] [varchar](20) NULL,
	[Seq_No] [int] NULL,
	[Customer_Money] [varchar](50) NULL,
	[Customer] [varchar](50) NULL,
	[Customer_Remark] [varchar](500) NULL,
	[Customer_Date] [datetime] NULL,
	[Is_Customer_Inv] [varchar](4) NULL,
	[Customer_Inv_Price] [varchar](50) NULL,
	[Customer_Inv_No] [varchar](50) NULL,
	[Customer_Inv_Date] [datetime] NULL,
 CONSTRAINT [PK_PT_B_Project_Customer] PRIMARY KEY CLUSTERED 
(
	[Customer_Pays_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


/*    ==脚本参数==

    源服务器版本 : SQL Server 2016 (13.0.4001)
    源数据库引擎版本 : Microsoft SQL Server Enterprise Edition
    源数据库引擎类型 : 独立的 SQL Server

    目标服务器版本 : SQL Server 2016
    目标数据库引擎版本 : Microsoft SQL Server Enterprise Edition
    目标数据库引擎类型 : 独立的 SQL Server
*/

USE [ProjectTracking]
GO

/****** Object:  Table [dbo].[PT_B_Project_Lab]    Script Date: 2018/1/21 23:52:44 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[PT_B_Project_Lab](
	[Lab_Pays_Id] [varchar](32) NOT NULL,
	[Project_No] [varchar](20) NULL,
	[Seq_No] [int] NULL,
	[Lab_Money] [varchar](50) NULL,
	[Lab] [varchar](50) NULL,
	[Lab_Remark] [varchar](500) NULL,
	[Lab_Date] [datetime] NULL,
	[Is_Lab_Inv] [varchar](4) NULL,
	[Lab_Inv_Price] [varchar](50) NULL,
	[Lab_Inv_No] [varchar](50) NULL,
	[Lab_Inv_Date] [datetime] NULL,
 CONSTRAINT [PK_PT_B_Project_Lab] PRIMARY KEY CLUSTERED 
(
	[Lab_Pays_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


/*    ==脚本参数==

    源服务器版本 : SQL Server 2016 (13.0.4001)
    源数据库引擎版本 : Microsoft SQL Server Enterprise Edition
    源数据库引擎类型 : 独立的 SQL Server

    目标服务器版本 : SQL Server 2016
    目标数据库引擎版本 : Microsoft SQL Server Enterprise Edition
    目标数据库引擎类型 : 独立的 SQL Server
*/

USE [ProjectTracking]
GO

/****** Object:  Table [dbo].[PT_B_Project_Other]    Script Date: 2018/1/21 23:52:57 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[PT_B_Project_Other](
	[Other_Pays_Id] [varchar](32) NOT NULL,
	[Project_No] [varchar](20) NULL,
	[Seq_No] [int] NULL,
	[Other_Money] [varchar](50) NULL,
	[Other] [varchar](50) NULL,
	[Other_Remark] [varchar](500) NULL,
	[Other_Date] [datetime] NULL,
	[Is_Other_Inv] [varchar](4) NULL,
	[Other_Inv_Price] [varchar](50) NULL,
	[Other_Inv_No] [varchar](50) NULL,
	[Other_Inv_Date] [datetime] NULL,
 CONSTRAINT [PK_PT_B_Project_Other] PRIMARY KEY CLUSTERED 
(
	[Other_Pays_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


/*    ==脚本参数==

    源服务器版本 : SQL Server 2016 (13.0.4001)
    源数据库引擎版本 : Microsoft SQL Server Enterprise Edition
    源数据库引擎类型 : 独立的 SQL Server

    目标服务器版本 : SQL Server 2016
    目标数据库引擎版本 : Microsoft SQL Server Enterprise Edition
    目标数据库引擎类型 : 独立的 SQL Server
*/

USE [ProjectTracking]
GO

/****** Object:  Table [dbo].[PT_B_Quotation]    Script Date: 2018/1/21 23:53:07 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[PT_B_Quotation](
	[Quotation_No] [varchar](20) NOT NULL,
	[Quotation_Date] [datetime] NULL,
	[Follow_Man] [varchar](50) NULL,
	[Product_Model] [varchar](100) NULL,
	[Project_Name] [varchar](500) NULL,
	[Price] [varchar](50) NULL,
	[Is_Tax] [varchar](4) NULL,
	[Quotation_Type] [varchar](10) NULL,
	[Cycle_Time] [varchar](50) NULL,
	[Company_Name] [varchar](150) NULL,
	[Company_Address] [varchar](400) NULL,
	[Contact_Man] [varchar](50) NULL,
	[Tel] [varchar](50) NULL,
	[Email] [varchar](50) NULL,
	[Fax] [varchar](50) NULL,
	[Remark] [varchar](400) NULL,
	[Bill_Status] [varchar](2) NULL,
	[Oper_Time] [datetime] NULL,
 CONSTRAINT [PK_PT_B_Quotation] PRIMARY KEY CLUSTERED 
(
	[Quotation_No] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


