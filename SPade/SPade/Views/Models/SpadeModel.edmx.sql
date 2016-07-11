
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 07/03/2016 22:00:13
-- Generated from EDMX file: E:\School\Y3\SDP\SPade-MVC\SPade\SPade\Models\SpadeModel.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [SPade];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_adminlogin_fk]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Login] DROP CONSTRAINT [FK_adminlogin_fk];
GO
IF OBJECT_ID(N'[dbo].[FK_assgn_fk]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Class_Assgn] DROP CONSTRAINT [FK_assgn_fk];
GO
IF OBJECT_ID(N'[dbo].[FK_class_fk]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Lec_Class] DROP CONSTRAINT [FK_class_fk];
GO
IF OBJECT_ID(N'[dbo].[FK_class_fk2]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Class_Assgn] DROP CONSTRAINT [FK_class_fk2];
GO
IF OBJECT_ID(N'[dbo].[FK_classcrse_fk]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Class] DROP CONSTRAINT [FK_classcrse_fk];
GO
IF OBJECT_ID(N'[dbo].[FK_Language_fk_constrain]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Module] DROP CONSTRAINT [FK_Language_fk_constrain];
GO
IF OBJECT_ID(N'[dbo].[FK_loginrole_fk]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Login] DROP CONSTRAINT [FK_loginrole_fk];
GO
IF OBJECT_ID(N'[dbo].[FK_Module_fk_constrain]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Assignment] DROP CONSTRAINT [FK_Module_fk_constrain];
GO
IF OBJECT_ID(N'[dbo].[FK_staff_fk]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Lec_Class] DROP CONSTRAINT [FK_staff_fk];
GO
IF OBJECT_ID(N'[dbo].[FK_staff_fkk]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Lec_Class] DROP CONSTRAINT [FK_staff_fkk];
GO
IF OBJECT_ID(N'[dbo].[FK_studlogin_fk]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Login] DROP CONSTRAINT [FK_studlogin_fk];
GO
IF OBJECT_ID(N'[SPadeModelStoreContainer].[FK_subassgn_fk]', 'F') IS NOT NULL
    ALTER TABLE [SPadeModelStoreContainer].[Submission] DROP CONSTRAINT [FK_subassgn_fk];
GO
IF OBJECT_ID(N'[SPadeModelStoreContainer].[FK_substud_fk]', 'F') IS NOT NULL
    ALTER TABLE [SPadeModelStoreContainer].[Submission] DROP CONSTRAINT [FK_substud_fk];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[Admin]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Admin];
GO
IF OBJECT_ID(N'[dbo].[Assignment]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Assignment];
GO
IF OBJECT_ID(N'[dbo].[Class]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Class];
GO
IF OBJECT_ID(N'[dbo].[Class_Assgn]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Class_Assgn];
GO
IF OBJECT_ID(N'[dbo].[Course]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Course];
GO
IF OBJECT_ID(N'[dbo].[Lec_Class]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Lec_Class];
GO
IF OBJECT_ID(N'[dbo].[Lecturer]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Lecturer];
GO
IF OBJECT_ID(N'[dbo].[Login]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Login];
GO
IF OBJECT_ID(N'[dbo].[Module]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Module];
GO
IF OBJECT_ID(N'[dbo].[ProgLanguage]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ProgLanguage];
GO
IF OBJECT_ID(N'[dbo].[Role]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Role];
GO
IF OBJECT_ID(N'[dbo].[Student]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Student];
GO
IF OBJECT_ID(N'[SPadeModelStoreContainer].[Submission]', 'U') IS NOT NULL
    DROP TABLE [SPadeModelStoreContainer].[Submission];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'Admins'
CREATE TABLE [dbo].[Admins] (
    [AdminID] varchar(20)  NOT NULL,
    [ContactNo] int  NOT NULL,
    [Email] varchar(50)  NOT NULL,
    [FullName] varchar(60)  NOT NULL
);
GO

-- Creating table 'Assignments'
CREATE TABLE [dbo].[Assignments] (
    [AssgnID] int  NOT NULL,
    [Describe] varchar(5000)  NOT NULL,
    [MaxAttempt] int  NOT NULL,
    [DueDate] datetime  NOT NULL,
    [Solution] varchar(200)  NOT NULL,
    [ModuleCode] varchar(20)  NOT NULL,
    [CreateAt] datetime  NOT NULL,
    [CreateBy] varchar(20)  NOT NULL,
    [UpdatedAt] datetime  NOT NULL,
    [UpdatedBy] varchar(20)  NOT NULL,
    [DeletedAt] datetime  NULL,
    [DeletedBy] varchar(20)  NULL
);
GO

-- Creating table 'Classes'
CREATE TABLE [dbo].[Classes] (
    [ClassID] int  NOT NULL,
    [CourseID] int  NOT NULL,
    [ClassName] varchar(10)  NOT NULL,
    [CreatedAt] datetime  NOT NULL,
    [CreatedBy] varchar(20)  NOT NULL,
    [UpdatedAt] datetime  NOT NULL,
    [UpdatedBy] varchar(20)  NOT NULL,
    [DeletedAt] datetime  NULL,
    [DeletedBy] varchar(20)  NULL
);
GO

-- Creating table 'Class_Assgn'
CREATE TABLE [dbo].[Class_Assgn] (
    [ClassID] int  NOT NULL,
    [AssgnID] int  NOT NULL,
    [C_id] int IDENTITY(1,1) NOT NULL
);
GO

-- Creating table 'Courses'
CREATE TABLE [dbo].[Courses] (
    [CourseID] int  NOT NULL,
    [CourseName] varchar(200)  NOT NULL,
    [CourseAbbr] varchar(10)  NOT NULL,
    [CreatedAt] datetime  NOT NULL,
    [CreatedBy] varchar(20)  NOT NULL,
    [UpdatedAt] datetime  NOT NULL,
    [UpdatedBy] varchar(20)  NOT NULL,
    [DeletedAt] datetime  NULL,
    [DeletedBy] varchar(20)  NULL
);
GO

-- Creating table 'Lec_Class'
CREATE TABLE [dbo].[Lec_Class] (
    [StaffID] varchar(20)  NOT NULL,
    [ClassID] int  NOT NULL,
    [C_id] int IDENTITY(1,1) NOT NULL
);
GO

-- Creating table 'Lecturers'
CREATE TABLE [dbo].[Lecturers] (
    [StaffID] varchar(20)  NOT NULL,
    [Name] varchar(50)  NOT NULL,
    [Email] varchar(50)  NOT NULL,
    [ContactNo] int  NOT NULL,
    [CreatedAt] datetime  NOT NULL,
    [CreatedBy] varchar(20)  NOT NULL,
    [UpdatedAt] datetime  NOT NULL,
    [UpdatedBy] varchar(20)  NOT NULL,
    [DeletedAt] datetime  NULL,
    [DeletedBy] varchar(20)  NULL
);
GO

-- Creating table 'Logins'
CREATE TABLE [dbo].[Logins] (
    [LoginID] varchar(20)  NOT NULL,
    [Salt] varchar(200)  NOT NULL,
    [RoleID] int  NOT NULL,
    [HashedPassword] binary(32)  NOT NULL
);
GO

-- Creating table 'Modules'
CREATE TABLE [dbo].[Modules] (
    [ModuleCode] varchar(20)  NOT NULL,
    [ModuleName] varchar(100)  NOT NULL,
    [LanguageId] int  NOT NULL,
    [CreatedAt] datetime  NOT NULL,
    [CreatedBy] varchar(20)  NOT NULL,
    [UpdatedAt] datetime  NOT NULL,
    [UpdatedBy] varchar(20)  NOT NULL,
    [DeletedAt] datetime  NULL,
    [DeletedBy] varchar(20)  NULL
);
GO

-- Creating table 'ProgLanguages'
CREATE TABLE [dbo].[ProgLanguages] (
    [LanguageId] int IDENTITY(1,1) NOT NULL,
    [LangageType] varchar(50)  NOT NULL,
    [CreatedAt] datetime  NOT NULL,
    [CreatedBy] varchar(20)  NOT NULL,
    [UpdatedAt] datetime  NOT NULL,
    [UpdatedBy] varchar(20)  NOT NULL,
    [DeletedAt] datetime  NULL,
    [DeletedBy] varchar(20)  NULL
);
GO

-- Creating table 'Roles'
CREATE TABLE [dbo].[Roles] (
    [RoleID] int  NOT NULL,
    [RoleName] varchar(20)  NOT NULL
);
GO

-- Creating table 'Students'
CREATE TABLE [dbo].[Students] (
    [AdminNo] varchar(20)  NOT NULL,
    [Name] varchar(50)  NOT NULL,
    [Email] varchar(50)  NOT NULL,
    [ContactNo] int  NOT NULL,
    [ClassID] int  NOT NULL,
    [CreatedAt] datetime  NOT NULL,
    [CreatedBy] varchar(20)  NOT NULL,
    [UpdatedAt] datetime  NOT NULL,
    [UpdatedBy] varchar(20)  NOT NULL,
    [DeletedAt] datetime  NULL,
    [DeletedBy] varchar(20)  NULL
);
GO

-- Creating table 'Submissions'
CREATE TABLE [dbo].[Submissions] (
    [SubmissionID] int  NOT NULL,
    [AdminNo] varchar(20)  NOT NULL,
    [AssgnID] int  NOT NULL,
    [Grade] decimal(3,2)  NOT NULL,
    [FilePath] varchar(200)  NOT NULL,
    [Timestamp] datetime  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [AdminID] in table 'Admins'
ALTER TABLE [dbo].[Admins]
ADD CONSTRAINT [PK_Admins]
    PRIMARY KEY CLUSTERED ([AdminID] ASC);
GO

-- Creating primary key on [AssgnID] in table 'Assignments'
ALTER TABLE [dbo].[Assignments]
ADD CONSTRAINT [PK_Assignments]
    PRIMARY KEY CLUSTERED ([AssgnID] ASC);
GO

-- Creating primary key on [ClassID] in table 'Classes'
ALTER TABLE [dbo].[Classes]
ADD CONSTRAINT [PK_Classes]
    PRIMARY KEY CLUSTERED ([ClassID] ASC);
GO

-- Creating primary key on [C_id] in table 'Class_Assgn'
ALTER TABLE [dbo].[Class_Assgn]
ADD CONSTRAINT [PK_Class_Assgn]
    PRIMARY KEY CLUSTERED ([C_id] ASC);
GO

-- Creating primary key on [CourseID] in table 'Courses'
ALTER TABLE [dbo].[Courses]
ADD CONSTRAINT [PK_Courses]
    PRIMARY KEY CLUSTERED ([CourseID] ASC);
GO

-- Creating primary key on [C_id] in table 'Lec_Class'
ALTER TABLE [dbo].[Lec_Class]
ADD CONSTRAINT [PK_Lec_Class]
    PRIMARY KEY CLUSTERED ([C_id] ASC);
GO

-- Creating primary key on [StaffID] in table 'Lecturers'
ALTER TABLE [dbo].[Lecturers]
ADD CONSTRAINT [PK_Lecturers]
    PRIMARY KEY CLUSTERED ([StaffID] ASC);
GO

-- Creating primary key on [LoginID] in table 'Logins'
ALTER TABLE [dbo].[Logins]
ADD CONSTRAINT [PK_Logins]
    PRIMARY KEY CLUSTERED ([LoginID] ASC);
GO

-- Creating primary key on [ModuleCode] in table 'Modules'
ALTER TABLE [dbo].[Modules]
ADD CONSTRAINT [PK_Modules]
    PRIMARY KEY CLUSTERED ([ModuleCode] ASC);
GO

-- Creating primary key on [LanguageId] in table 'ProgLanguages'
ALTER TABLE [dbo].[ProgLanguages]
ADD CONSTRAINT [PK_ProgLanguages]
    PRIMARY KEY CLUSTERED ([LanguageId] ASC);
GO

-- Creating primary key on [RoleID] in table 'Roles'
ALTER TABLE [dbo].[Roles]
ADD CONSTRAINT [PK_Roles]
    PRIMARY KEY CLUSTERED ([RoleID] ASC);
GO

-- Creating primary key on [AdminNo] in table 'Students'
ALTER TABLE [dbo].[Students]
ADD CONSTRAINT [PK_Students]
    PRIMARY KEY CLUSTERED ([AdminNo] ASC);
GO

-- Creating primary key on [SubmissionID], [AdminNo], [AssgnID], [Grade], [FilePath], [Timestamp] in table 'Submissions'
ALTER TABLE [dbo].[Submissions]
ADD CONSTRAINT [PK_Submissions]
    PRIMARY KEY CLUSTERED ([SubmissionID], [AdminNo], [AssgnID], [Grade], [FilePath], [Timestamp] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [LoginID] in table 'Logins'
ALTER TABLE [dbo].[Logins]
ADD CONSTRAINT [FK_adminlogin_fk]
    FOREIGN KEY ([LoginID])
    REFERENCES [dbo].[Admins]
        ([AdminID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [AssgnID] in table 'Class_Assgn'
ALTER TABLE [dbo].[Class_Assgn]
ADD CONSTRAINT [FK_assgn_fk]
    FOREIGN KEY ([AssgnID])
    REFERENCES [dbo].[Assignments]
        ([AssgnID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_assgn_fk'
CREATE INDEX [IX_FK_assgn_fk]
ON [dbo].[Class_Assgn]
    ([AssgnID]);
GO

-- Creating foreign key on [ModuleCode] in table 'Assignments'
ALTER TABLE [dbo].[Assignments]
ADD CONSTRAINT [FK_Module_fk_constrain]
    FOREIGN KEY ([ModuleCode])
    REFERENCES [dbo].[Modules]
        ([ModuleCode])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Module_fk_constrain'
CREATE INDEX [IX_FK_Module_fk_constrain]
ON [dbo].[Assignments]
    ([ModuleCode]);
GO

-- Creating foreign key on [AssgnID] in table 'Submissions'
ALTER TABLE [dbo].[Submissions]
ADD CONSTRAINT [FK_subassgn_fk]
    FOREIGN KEY ([AssgnID])
    REFERENCES [dbo].[Assignments]
        ([AssgnID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_subassgn_fk'
CREATE INDEX [IX_FK_subassgn_fk]
ON [dbo].[Submissions]
    ([AssgnID]);
GO

-- Creating foreign key on [ClassID] in table 'Lec_Class'
ALTER TABLE [dbo].[Lec_Class]
ADD CONSTRAINT [FK_class_fk]
    FOREIGN KEY ([ClassID])
    REFERENCES [dbo].[Classes]
        ([ClassID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_class_fk'
CREATE INDEX [IX_FK_class_fk]
ON [dbo].[Lec_Class]
    ([ClassID]);
GO

-- Creating foreign key on [ClassID] in table 'Class_Assgn'
ALTER TABLE [dbo].[Class_Assgn]
ADD CONSTRAINT [FK_class_fk2]
    FOREIGN KEY ([ClassID])
    REFERENCES [dbo].[Classes]
        ([ClassID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_class_fk2'
CREATE INDEX [IX_FK_class_fk2]
ON [dbo].[Class_Assgn]
    ([ClassID]);
GO

-- Creating foreign key on [CourseID] in table 'Classes'
ALTER TABLE [dbo].[Classes]
ADD CONSTRAINT [FK_classcrse_fk]
    FOREIGN KEY ([CourseID])
    REFERENCES [dbo].[Courses]
        ([CourseID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_classcrse_fk'
CREATE INDEX [IX_FK_classcrse_fk]
ON [dbo].[Classes]
    ([CourseID]);
GO

-- Creating foreign key on [StaffID] in table 'Lec_Class'
ALTER TABLE [dbo].[Lec_Class]
ADD CONSTRAINT [FK_staff_fk]
    FOREIGN KEY ([StaffID])
    REFERENCES [dbo].[Lecturers]
        ([StaffID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_staff_fk'
CREATE INDEX [IX_FK_staff_fk]
ON [dbo].[Lec_Class]
    ([StaffID]);
GO

-- Creating foreign key on [StaffID] in table 'Lec_Class'
ALTER TABLE [dbo].[Lec_Class]
ADD CONSTRAINT [FK_staff_fkk]
    FOREIGN KEY ([StaffID])
    REFERENCES [dbo].[Lecturers]
        ([StaffID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_staff_fkk'
CREATE INDEX [IX_FK_staff_fkk]
ON [dbo].[Lec_Class]
    ([StaffID]);
GO

-- Creating foreign key on [RoleID] in table 'Logins'
ALTER TABLE [dbo].[Logins]
ADD CONSTRAINT [FK_loginrole_fk]
    FOREIGN KEY ([RoleID])
    REFERENCES [dbo].[Roles]
        ([RoleID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_loginrole_fk'
CREATE INDEX [IX_FK_loginrole_fk]
ON [dbo].[Logins]
    ([RoleID]);
GO

-- Creating foreign key on [LoginID] in table 'Logins'
ALTER TABLE [dbo].[Logins]
ADD CONSTRAINT [FK_studlogin_fk]
    FOREIGN KEY ([LoginID])
    REFERENCES [dbo].[Students]
        ([AdminNo])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [LanguageId] in table 'Modules'
ALTER TABLE [dbo].[Modules]
ADD CONSTRAINT [FK_Language_fk_constrain]
    FOREIGN KEY ([LanguageId])
    REFERENCES [dbo].[ProgLanguages]
        ([LanguageId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Language_fk_constrain'
CREATE INDEX [IX_FK_Language_fk_constrain]
ON [dbo].[Modules]
    ([LanguageId]);
GO

-- Creating foreign key on [AdminNo] in table 'Submissions'
ALTER TABLE [dbo].[Submissions]
ADD CONSTRAINT [FK_substud_fk]
    FOREIGN KEY ([AdminNo])
    REFERENCES [dbo].[Students]
        ([AdminNo])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_substud_fk'
CREATE INDEX [IX_FK_substud_fk]
ON [dbo].[Submissions]
    ([AdminNo]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------