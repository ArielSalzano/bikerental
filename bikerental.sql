USE [master]
GO
/****** Object:  Database [bikerental]    Script Date: 3/26/2019 5:59:07 PM ******/
CREATE DATABASE [bikerental] ON  PRIMARY 
( NAME = N'bikerental', FILENAME = N'c:\Program Files\Microsoft SQL Server\MSSQL10_50.SQLEXPRESS2008R2\MSSQL\DATA\bikerental.mdf' , SIZE = 2048KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'bikerental_log', FILENAME = N'c:\Program Files\Microsoft SQL Server\MSSQL10_50.SQLEXPRESS2008R2\MSSQL\DATA\bikerental_log.ldf' , SIZE = 1024KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO
ALTER DATABASE [bikerental] SET COMPATIBILITY_LEVEL = 100
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [bikerental].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [bikerental] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [bikerental] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [bikerental] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [bikerental] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [bikerental] SET ARITHABORT OFF 
GO
ALTER DATABASE [bikerental] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [bikerental] SET AUTO_CREATE_STATISTICS ON 
GO
ALTER DATABASE [bikerental] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [bikerental] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [bikerental] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [bikerental] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [bikerental] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [bikerental] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [bikerental] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [bikerental] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [bikerental] SET  DISABLE_BROKER 
GO
ALTER DATABASE [bikerental] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [bikerental] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [bikerental] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [bikerental] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [bikerental] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [bikerental] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [bikerental] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [bikerental] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [bikerental] SET  MULTI_USER 
GO
ALTER DATABASE [bikerental] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [bikerental] SET DB_CHAINING OFF 
GO
USE [bikerental]
GO
/****** Object:  StoredProcedure [dbo].[rental_add]    Script Date: 3/26/2019 5:59:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[rental_add]
 (@typeid tinyint = null)
 AS
 /** adds new rental, returns its values **/

 DECLARE @id BIGINT
 SELECT @id =ISNULL(MAX(id),0) +1 FROM dbo.rental

 DECLARE @discount tinyint

 IF @typeid = 4 SELECT @discount = discount FROM dbo.rentaltype WHERE id = @typeid
  
 INSERT INTO dbo.rental ( id, [timestamp], discount )
 VALUES	( @id, GETDATE(), @discount)

 SELECT * FROM rental WHERE id = @id 

GO
/****** Object:  StoredProcedure [dbo].[rentaldetail]    Script Date: 3/26/2019 5:59:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[rentaldetail]
 (@id bigint)
 AS
 
 DECLARE @rentaltotal AS smallmoney

 SELECT @rentaltotal =  dbo.rentaltotal (@id)

 /** returns rental and rental items values **/
 SELECT r.id rentalid, r.[timestamp], r.discount, @rentaltotal * (100-ISNULL(r.discount,0)) /100 totalprice, i.id rentalitem, i.typeid,t.typename, i.quantity, i.price, i.quantity * i.price AS itemtotal, i.pickup, 
        CASE i.typeid 
		  WHEN 1 THEN dateadd(HOUR,i.quantity, i.pickup)
		  WHEN 2 THEN dateadd(DAY,i.quantity, i.pickup)
		  WHEN 3 THEN dateadd(WEEK,i.quantity, i.pickup)
		END	dropoff
 
 FROM rental r, rentalitem i, rentaltype t
 where r.id = @id 
 AND   r.id = i.rentalid
 AND   i.typeid = t.id


GO
/****** Object:  StoredProcedure [dbo].[rentalitem_add]    Script Date: 3/26/2019 5:59:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[rentalitem_add]
 (@rentalid bigint ,
  @typeid SMALLINT,
  @quantity tinyint,
  @pickup smalldatetime
 )
 AS
 /** adds new rentalitem, returns rental and rental items values **/
  
 DECLARE @id tinyint
 SELECT @id = ISNULL(MAX(id),0) +1 FROM dbo.rentalitem WHERE rentalid = @rentalid
 
 INSERT INTO dbo.rentalitem ( rentalid, id, typeid, quantity, pickup, price )
 SELECT @rentalid, @id, @typeid, @quantity, @pickup, price
 FROM   dbo.rentaltype 
 WHERE id = @typeid
 
 EXEC rentaldetail @rentalid
GO
/****** Object:  StoredProcedure [dbo].[rentaltype_save]    Script Date: 3/26/2019 5:59:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[rentaltype_save]
(@id smallint=NULL, 
 @typename varchar(20)= NULL, 
 @price smallmoney= NULL,
 @discount tinyint= NULL
 )
 
AS
/** adds new rental type or modifies existing, returns its values**/

IF @id IS NULL
BEGIN 
	SELECT @id = ISNULL(MAX(id),0) +1 FROM dbo.rentaltype
	INSERT INTO dbo.rentaltype
			( id, typename, price, discount )
	VALUES	( @id,@typename,@price, @discount)
END
ELSE
BEGIN 
	UPDATE dbo.rentaltype
	SET    price = @price,
	       discount =  @discount
	WHERE  id  = @id
END

SELECT * FROM dbo.rentaltype WHERE id = @id

GO
/****** Object:  UserDefinedFunction [dbo].[rentaltotal]    Script Date: 3/26/2019 5:59:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE FUNCTION [dbo].[rentaltotal](@id bigint)  
RETURNS smallmoney
AS  
/** Returns rental total price as sum of items totals**/  
BEGIN  
   declare @total SMALLMONEY 
   
   SELECT @total = SUM(i.quantity * i.price)
   FROM  dbo.rentalitem i
   WHERE i.rentalid  = @id
   GROUP BY rentalid
     
   RETURN(@total)   
 END   
  
GO
/****** Object:  Table [dbo].[rental]    Script Date: 3/26/2019 5:59:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[rental](
	[id] [bigint] NOT NULL,
	[timestamp] [smalldatetime] NOT NULL,
	[discount] [tinyint] NULL,
 CONSTRAINT [PK_rentalt] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[rentalitem]    Script Date: 3/26/2019 5:59:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[rentalitem](
	[rentalid] [bigint] NOT NULL,
	[id] [tinyint] NOT NULL,
	[typeid] [smallint] NULL,
	[quantity] [tinyint] NULL,
	[pickup] [smalldatetime] NULL,
	[price] [smallmoney] NULL,
 CONSTRAINT [PK_rentalitem] PRIMARY KEY CLUSTERED 
(
	[rentalid] ASC,
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[rentaltype]    Script Date: 3/26/2019 5:59:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[rentaltype](
	[id] [smallint] NOT NULL,
	[typename] [varchar](20) NOT NULL,
	[price] [smallmoney] NULL,
	[discount] [tinyint] NULL,
 CONSTRAINT [PK_rentaltype] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
ALTER TABLE [dbo].[rentalitem]  WITH CHECK ADD  CONSTRAINT [FK_rental] FOREIGN KEY([rentalid])
REFERENCES [dbo].[rental] ([id])
GO
ALTER TABLE [dbo].[rentalitem] CHECK CONSTRAINT [FK_rental]
GO
ALTER TABLE [dbo].[rentalitem]  WITH CHECK ADD  CONSTRAINT [FK_type] FOREIGN KEY([typeid])
REFERENCES [dbo].[rentaltype] ([id])
GO
ALTER TABLE [dbo].[rentalitem] CHECK CONSTRAINT [FK_type]
GO
USE [master]
GO
ALTER DATABASE [bikerental] SET  READ_WRITE 
GO
