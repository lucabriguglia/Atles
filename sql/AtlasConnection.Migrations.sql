IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200813085546_InitialCreate')
BEGIN
    CREATE TABLE [Member] (
        [Id] uniqueidentifier NOT NULL,
        [UserId] nvarchar(max) NULL,
        [Email] nvarchar(max) NULL,
        [DisplayName] nvarchar(max) NULL,
        [TopicsCount] int NOT NULL,
        [RepliesCount] int NOT NULL,
        [Status] int NOT NULL,
        CONSTRAINT [PK_Member] PRIMARY KEY ([Id])
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200813085546_InitialCreate')
BEGIN
    CREATE TABLE [PermissionSet] (
        [Id] uniqueidentifier NOT NULL,
        [SiteId] uniqueidentifier NOT NULL,
        [Name] nvarchar(max) NULL,
        [Status] int NOT NULL,
        CONSTRAINT [PK_PermissionSet] PRIMARY KEY ([Id])
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200813085546_InitialCreate')
BEGIN
    CREATE TABLE [Site] (
        [Id] uniqueidentifier NOT NULL,
        [Name] nvarchar(max) NULL,
        [Title] nvarchar(max) NULL,
        [PublicTheme] nvarchar(max) NULL,
        [PublicCss] nvarchar(max) NULL,
        [AdminTheme] nvarchar(max) NULL,
        [AdminCss] nvarchar(max) NULL,
        [Language] nvarchar(max) NULL,
        [Privacy] nvarchar(max) NULL,
        [Terms] nvarchar(max) NULL,
        CONSTRAINT [PK_Site] PRIMARY KEY ([Id])
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200813085546_InitialCreate')
BEGIN
    CREATE TABLE [Event] (
        [Id] uniqueidentifier NOT NULL,
        [SiteId] uniqueidentifier NOT NULL,
        [TimeStamp] datetime2 NOT NULL,
        [TargetId] uniqueidentifier NOT NULL,
        [TargetType] nvarchar(max) NULL,
        [Type] nvarchar(max) NULL,
        [Data] nvarchar(max) NULL,
        [MemberId] uniqueidentifier NULL,
        CONSTRAINT [PK_Event] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Event_Member_MemberId] FOREIGN KEY ([MemberId]) REFERENCES [Member] ([Id])
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200813085546_InitialCreate')
BEGIN
    CREATE TABLE [Permission] (
        [PermissionSetId] uniqueidentifier NOT NULL,
        [Type] int NOT NULL,
        [RoleId] nvarchar(450) NOT NULL,
        CONSTRAINT [PK_Permission] PRIMARY KEY ([PermissionSetId], [RoleId], [Type]),
        CONSTRAINT [FK_Permission_PermissionSet_PermissionSetId] FOREIGN KEY ([PermissionSetId]) REFERENCES [PermissionSet] ([Id])
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200813085546_InitialCreate')
BEGIN
    CREATE TABLE [Category] (
        [Id] uniqueidentifier NOT NULL,
        [SiteId] uniqueidentifier NOT NULL,
        [Name] nvarchar(max) NULL,
        [SortOrder] int NOT NULL,
        [TopicsCount] int NOT NULL,
        [RepliesCount] int NOT NULL,
        [Status] int NOT NULL,
        [PermissionSetId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_Category] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Category_PermissionSet_PermissionSetId] FOREIGN KEY ([PermissionSetId]) REFERENCES [PermissionSet] ([Id]),
        CONSTRAINT [FK_Category_Site_SiteId] FOREIGN KEY ([SiteId]) REFERENCES [Site] ([Id])
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200813085546_InitialCreate')
BEGIN
    CREATE TABLE [Forum] (
        [Id] uniqueidentifier NOT NULL,
        [CategoryId] uniqueidentifier NOT NULL,
        [Name] nvarchar(max) NULL,
        [Description] nvarchar(max) NULL,
        [SortOrder] int NOT NULL,
        [TopicsCount] int NOT NULL,
        [RepliesCount] int NOT NULL,
        [Status] int NOT NULL,
        [PermissionSetId] uniqueidentifier NULL,
        [LastPostId] uniqueidentifier NULL,
        CONSTRAINT [PK_Forum] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Forum_Category_CategoryId] FOREIGN KEY ([CategoryId]) REFERENCES [Category] ([Id]),
        CONSTRAINT [FK_Forum_PermissionSet_PermissionSetId] FOREIGN KEY ([PermissionSetId]) REFERENCES [PermissionSet] ([Id])
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200813085546_InitialCreate')
BEGIN
    CREATE TABLE [Post] (
        [Id] uniqueidentifier NOT NULL,
        [ForumId] uniqueidentifier NOT NULL,
        [Title] nvarchar(max) NULL,
        [Content] nvarchar(max) NULL,
        [RepliesCount] int NOT NULL,
        [Status] int NOT NULL,
        [MemberId] uniqueidentifier NOT NULL,
        [TimeStamp] datetime2 NOT NULL,
        [TopicId] uniqueidentifier NULL,
        [LastReplyId] uniqueidentifier NULL,
        CONSTRAINT [PK_Post] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Post_Forum_ForumId] FOREIGN KEY ([ForumId]) REFERENCES [Forum] ([Id]),
        CONSTRAINT [FK_Post_Post_LastReplyId] FOREIGN KEY ([LastReplyId]) REFERENCES [Post] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Post_Member_MemberId] FOREIGN KEY ([MemberId]) REFERENCES [Member] ([Id]),
        CONSTRAINT [FK_Post_Post_TopicId] FOREIGN KEY ([TopicId]) REFERENCES [Post] ([Id]) ON DELETE NO ACTION
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200813085546_InitialCreate')
BEGIN
    CREATE INDEX [IX_Category_PermissionSetId] ON [Category] ([PermissionSetId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200813085546_InitialCreate')
BEGIN
    CREATE INDEX [IX_Category_SiteId] ON [Category] ([SiteId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200813085546_InitialCreate')
BEGIN
    CREATE INDEX [IX_Event_MemberId] ON [Event] ([MemberId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200813085546_InitialCreate')
BEGIN
    CREATE INDEX [IX_Forum_CategoryId] ON [Forum] ([CategoryId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200813085546_InitialCreate')
BEGIN
    CREATE INDEX [IX_Forum_LastPostId] ON [Forum] ([LastPostId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200813085546_InitialCreate')
BEGIN
    CREATE INDEX [IX_Forum_PermissionSetId] ON [Forum] ([PermissionSetId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200813085546_InitialCreate')
BEGIN
    CREATE INDEX [IX_Post_ForumId] ON [Post] ([ForumId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200813085546_InitialCreate')
BEGIN
    CREATE INDEX [IX_Post_LastReplyId] ON [Post] ([LastReplyId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200813085546_InitialCreate')
BEGIN
    CREATE INDEX [IX_Post_MemberId] ON [Post] ([MemberId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200813085546_InitialCreate')
BEGIN
    CREATE INDEX [IX_Post_TopicId] ON [Post] ([TopicId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200813085546_InitialCreate')
BEGIN
    ALTER TABLE [Forum] ADD CONSTRAINT [FK_Forum_Post_LastPostId] FOREIGN KEY ([LastPostId]) REFERENCES [Post] ([Id]) ON DELETE NO ACTION;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200813085546_InitialCreate')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20200813085546_InitialCreate', N'3.1.6');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200814143301_AddPinnedAndLockedToPost')
BEGIN
    ALTER TABLE [Post] ADD [Locked] bit NOT NULL DEFAULT CAST(0 AS bit);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200814143301_AddPinnedAndLockedToPost')
BEGIN
    ALTER TABLE [Post] ADD [Pinned] bit NOT NULL DEFAULT CAST(0 AS bit);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200814143301_AddPinnedAndLockedToPost')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20200814143301_AddPinnedAndLockedToPost', N'3.1.6');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200815194322_AddSlugToForumAndPost')
BEGIN
    ALTER TABLE [Post] ADD [Slug] nvarchar(max) NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200815194322_AddSlugToForumAndPost')
BEGIN
    ALTER TABLE [Forum] ADD [Slug] nvarchar(max) NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200815194322_AddSlugToForumAndPost')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20200815194322_AddSlugToForumAndPost', N'3.1.6');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200819091023_UpdatePostAndMember')
BEGIN
    ALTER TABLE [Post] DROP CONSTRAINT [FK_Post_Member_MemberId];
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200819091023_UpdatePostAndMember')
BEGIN
    EXEC sp_rename N'[Post].[TimeStamp]', N'CreatedOn', N'COLUMN';
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200819091023_UpdatePostAndMember')
BEGIN
    EXEC sp_rename N'[Post].[MemberId]', N'CreatedBy', N'COLUMN';
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200819091023_UpdatePostAndMember')
BEGIN
    EXEC sp_rename N'[Post].[IX_Post_MemberId]', N'IX_Post_CreatedBy', N'INDEX';
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200819091023_UpdatePostAndMember')
BEGIN
    ALTER TABLE [Post] ADD [ModifiedBy] uniqueidentifier NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200819091023_UpdatePostAndMember')
BEGIN
    ALTER TABLE [Post] ADD [ModifiedOn] datetime2 NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200819091023_UpdatePostAndMember')
BEGIN
    ALTER TABLE [Member] ADD [TimeStamp] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200819091023_UpdatePostAndMember')
BEGIN
    CREATE INDEX [IX_Post_ModifiedBy] ON [Post] ([ModifiedBy]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200819091023_UpdatePostAndMember')
BEGIN
    ALTER TABLE [Post] ADD CONSTRAINT [FK_Post_Member_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [Member] ([Id]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200819091023_UpdatePostAndMember')
BEGIN
    ALTER TABLE [Post] ADD CONSTRAINT [FK_Post_Member_ModifiedBy] FOREIGN KEY ([ModifiedBy]) REFERENCES [Member] ([Id]) ON DELETE NO ACTION;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200819091023_UpdatePostAndMember')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20200819091023_UpdatePostAndMember', N'3.1.6');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200819114322_AddAswerToPost')
BEGIN
    ALTER TABLE [Post] ADD [HasAnswer] bit NOT NULL DEFAULT CAST(0 AS bit);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200819114322_AddAswerToPost')
BEGIN
    ALTER TABLE [Post] ADD [IsAnswer] bit NOT NULL DEFAULT CAST(0 AS bit);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200819114322_AddAswerToPost')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20200819114322_AddAswerToPost', N'3.1.6');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200829182602_RenameMember')
BEGIN
    ALTER TABLE [Event] DROP CONSTRAINT [FK_Event_Member_MemberId];
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200829182602_RenameMember')
BEGIN
    ALTER TABLE [Post] DROP CONSTRAINT [FK_Post_Member_CreatedBy];
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200829182602_RenameMember')
BEGIN
    ALTER TABLE [Post] DROP CONSTRAINT [FK_Post_Member_ModifiedBy];
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200829182602_RenameMember')
BEGIN
    ALTER TABLE [Member] DROP CONSTRAINT [PK_Member];
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200829182602_RenameMember')
BEGIN
    EXEC sp_rename N'[Member]', N'User';
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200829182602_RenameMember')
BEGIN
    EXEC sp_rename N'[Event].[MemberId]', N'UserId', N'COLUMN';
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200829182602_RenameMember')
BEGIN
    EXEC sp_rename N'[Event].[IX_Event_MemberId]', N'IX_Event_UserId', N'INDEX';
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200829182602_RenameMember')
BEGIN
    EXEC sp_rename N'[User].[UserId]', N'IdentityUserId', N'COLUMN';
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200829182602_RenameMember')
BEGIN
    ALTER TABLE [User] ADD CONSTRAINT [PK_User] PRIMARY KEY ([Id]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200829182602_RenameMember')
BEGIN
    ALTER TABLE [Event] ADD CONSTRAINT [FK_Event_User_UserId] FOREIGN KEY ([UserId]) REFERENCES [User] ([Id]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200829182602_RenameMember')
BEGIN
    ALTER TABLE [Post] ADD CONSTRAINT [FK_Post_User_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [User] ([Id]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200829182602_RenameMember')
BEGIN
    ALTER TABLE [Post] ADD CONSTRAINT [FK_Post_User_ModifiedBy] FOREIGN KEY ([ModifiedBy]) REFERENCES [User] ([Id]) ON DELETE NO ACTION;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200829182602_RenameMember')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20200829182602_RenameMember', N'3.1.6');
END;

GO

