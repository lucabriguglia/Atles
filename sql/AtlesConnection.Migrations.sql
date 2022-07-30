IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220215094044_InitialCreate')
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

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220215094044_InitialCreate')
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
        [HeadScript] nvarchar(max) NULL,
        CONSTRAINT [PK_Site] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220215094044_InitialCreate')
BEGIN
    CREATE TABLE [User] (
        [Id] uniqueidentifier NOT NULL,
        [IdentityUserId] nvarchar(max) NULL,
        [Email] nvarchar(max) NULL,
        [DisplayName] nvarchar(max) NULL,
        [TopicsCount] int NOT NULL,
        [RepliesCount] int NOT NULL,
        [AnswersCount] int NOT NULL,
        [Status] int NOT NULL,
        [TimeStamp] datetime2 NOT NULL,
        CONSTRAINT [PK_User] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220215094044_InitialCreate')
BEGIN
    CREATE TABLE [UserRank] (
        [Id] uniqueidentifier NOT NULL,
        [Name] nvarchar(max) NULL,
        [Description] nvarchar(max) NULL,
        [SortOrder] int NOT NULL,
        [Badge] nvarchar(max) NULL,
        [Role] nvarchar(max) NULL,
        CONSTRAINT [PK_UserRank] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220215094044_InitialCreate')
BEGIN
    CREATE TABLE [Permission] (
        [Type] int NOT NULL,
        [RoleId] nvarchar(450) NOT NULL,
        [PermissionSetId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_Permission] PRIMARY KEY ([PermissionSetId], [RoleId], [Type]),
        CONSTRAINT [FK_Permission_PermissionSet_PermissionSetId] FOREIGN KEY ([PermissionSetId]) REFERENCES [PermissionSet] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220215094044_InitialCreate')
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

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220215094044_InitialCreate')
BEGIN
    CREATE TABLE [Event] (
        [Id] uniqueidentifier NOT NULL,
        [TimeStamp] datetime2 NOT NULL,
        [Type] nvarchar(max) NULL,
        [Data] nvarchar(max) NULL,
        [TargetId] uniqueidentifier NOT NULL,
        [TargetType] nvarchar(max) NULL,
        [SiteId] uniqueidentifier NOT NULL,
        [UserId] uniqueidentifier NULL,
        CONSTRAINT [PK_Event] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Event_User_UserId] FOREIGN KEY ([UserId]) REFERENCES [User] ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220215094044_InitialCreate')
BEGIN
    CREATE TABLE [Subscription] (
        [UserId] uniqueidentifier NOT NULL,
        [ItemId] uniqueidentifier NOT NULL,
        [Type] int NOT NULL,
        CONSTRAINT [PK_Subscription] PRIMARY KEY ([UserId], [ItemId]),
        CONSTRAINT [FK_Subscription_User_UserId] FOREIGN KEY ([UserId]) REFERENCES [User] ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220215094044_InitialCreate')
BEGIN
    CREATE TABLE [UserRankRule] (
        [UserRankId] uniqueidentifier NOT NULL,
        [Type] int NOT NULL,
        [Name] nvarchar(max) NULL,
        [Description] nvarchar(max) NULL,
        [Count] int NOT NULL,
        [Badge] nvarchar(max) NULL,
        CONSTRAINT [PK_UserRankRule] PRIMARY KEY ([UserRankId], [Type]),
        CONSTRAINT [FK_UserRankRule_UserRank_UserRankId] FOREIGN KEY ([UserRankId]) REFERENCES [UserRank] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220215094044_InitialCreate')
BEGIN
    CREATE TABLE [Forum] (
        [Id] uniqueidentifier NOT NULL,
        [CategoryId] uniqueidentifier NOT NULL,
        [Name] nvarchar(max) NULL,
        [Slug] nvarchar(max) NULL,
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

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220215094044_InitialCreate')
BEGIN
    CREATE TABLE [Post] (
        [Id] uniqueidentifier NOT NULL,
        [ForumId] uniqueidentifier NOT NULL,
        [Title] nvarchar(max) NULL,
        [Slug] nvarchar(max) NULL,
        [Content] nvarchar(max) NULL,
        [RepliesCount] int NOT NULL,
        [Status] int NOT NULL,
        [CreatedBy] uniqueidentifier NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedBy] uniqueidentifier NULL,
        [ModifiedOn] datetime2 NULL,
        [Pinned] bit NOT NULL,
        [Locked] bit NOT NULL,
        [IsAnswer] bit NOT NULL,
        [HasAnswer] bit NOT NULL,
        [TopicId] uniqueidentifier NULL,
        [LastReplyId] uniqueidentifier NULL,
        CONSTRAINT [PK_Post] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Post_Forum_ForumId] FOREIGN KEY ([ForumId]) REFERENCES [Forum] ([Id]),
        CONSTRAINT [FK_Post_Post_LastReplyId] FOREIGN KEY ([LastReplyId]) REFERENCES [Post] ([Id]),
        CONSTRAINT [FK_Post_Post_TopicId] FOREIGN KEY ([TopicId]) REFERENCES [Post] ([Id]),
        CONSTRAINT [FK_Post_User_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [User] ([Id]),
        CONSTRAINT [FK_Post_User_ModifiedBy] FOREIGN KEY ([ModifiedBy]) REFERENCES [User] ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220215094044_InitialCreate')
BEGIN
    CREATE TABLE [PostReaction] (
        [PostId] uniqueidentifier NOT NULL,
        [UserId] uniqueidentifier NOT NULL,
        [Type] int NOT NULL,
        [TimeStamp] datetime2 NOT NULL,
        CONSTRAINT [PK_PostReaction] PRIMARY KEY ([PostId], [UserId]),
        CONSTRAINT [FK_PostReaction_Post_PostId] FOREIGN KEY ([PostId]) REFERENCES [Post] ([Id]),
        CONSTRAINT [FK_PostReaction_User_UserId] FOREIGN KEY ([UserId]) REFERENCES [User] ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220215094044_InitialCreate')
BEGIN
    CREATE TABLE [PostReactionSummary] (
        [PostId] uniqueidentifier NOT NULL,
        [Type] int NOT NULL,
        [Count] int NOT NULL,
        CONSTRAINT [PK_PostReactionSummary] PRIMARY KEY ([PostId], [Type]),
        CONSTRAINT [FK_PostReactionSummary_Post_PostId] FOREIGN KEY ([PostId]) REFERENCES [Post] ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220215094044_InitialCreate')
BEGIN
    CREATE INDEX [IX_Category_PermissionSetId] ON [Category] ([PermissionSetId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220215094044_InitialCreate')
BEGIN
    CREATE INDEX [IX_Category_SiteId] ON [Category] ([SiteId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220215094044_InitialCreate')
BEGIN
    CREATE INDEX [IX_Event_UserId] ON [Event] ([UserId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220215094044_InitialCreate')
BEGIN
    CREATE INDEX [IX_Forum_CategoryId] ON [Forum] ([CategoryId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220215094044_InitialCreate')
BEGIN
    CREATE INDEX [IX_Forum_LastPostId] ON [Forum] ([LastPostId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220215094044_InitialCreate')
BEGIN
    CREATE INDEX [IX_Forum_PermissionSetId] ON [Forum] ([PermissionSetId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220215094044_InitialCreate')
BEGIN
    CREATE INDEX [IX_Post_CreatedBy] ON [Post] ([CreatedBy]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220215094044_InitialCreate')
BEGIN
    CREATE INDEX [IX_Post_ForumId] ON [Post] ([ForumId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220215094044_InitialCreate')
BEGIN
    CREATE INDEX [IX_Post_LastReplyId] ON [Post] ([LastReplyId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220215094044_InitialCreate')
BEGIN
    CREATE INDEX [IX_Post_ModifiedBy] ON [Post] ([ModifiedBy]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220215094044_InitialCreate')
BEGIN
    CREATE INDEX [IX_Post_TopicId] ON [Post] ([TopicId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220215094044_InitialCreate')
BEGIN
    CREATE INDEX [IX_PostReaction_UserId] ON [PostReaction] ([UserId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220215094044_InitialCreate')
BEGIN
    ALTER TABLE [Forum] ADD CONSTRAINT [FK_Forum_Post_LastPostId] FOREIGN KEY ([LastPostId]) REFERENCES [Post] ([Id]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220215094044_InitialCreate')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20220215094044_InitialCreate', N'6.0.1');
END;
GO

COMMIT;
GO

