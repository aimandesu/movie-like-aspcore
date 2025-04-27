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
CREATE TABLE [Categories] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_Categories] PRIMARY KEY ([Id])
);

CREATE TABLE [Series] (
    [Id] int NOT NULL IDENTITY,
    [Title] nvarchar(max) NOT NULL,
    [Description] nvarchar(max) NOT NULL,
    [Thumbnail] nvarchar(max) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [SeriesFormat] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_Series] PRIMARY KEY ([Id])
);

CREATE TABLE [Tags] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_Tags] PRIMARY KEY ([Id])
);

CREATE TABLE [Users] (
    [Id] int NOT NULL IDENTITY,
    [Username] nvarchar(max) NOT NULL,
    [Email] int NOT NULL,
    [Password] nvarchar(max) NOT NULL,
    [About] nvarchar(max) NOT NULL,
    [Avatar] nvarchar(max) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_Users] PRIMARY KEY ([Id])
);

CREATE TABLE [Episodes] (
    [Id] int NOT NULL IDENTITY,
    [Title] nvarchar(max) NOT NULL,
    [Description] nvarchar(max) NOT NULL,
    [Thumbnail] nvarchar(max) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [Season] int NULL,
    [EpisodeNumber] int NULL,
    [SeriesId] int NOT NULL,
    CONSTRAINT [PK_Episodes] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Episodes_Series_SeriesId] FOREIGN KEY ([SeriesId]) REFERENCES [Series] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [SeriesCategories] (
    [Id] int NOT NULL IDENTITY,
    [CategoryId] int NOT NULL,
    [SeriesId] int NOT NULL,
    CONSTRAINT [PK_SeriesCategories] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_SeriesCategories_Categories_CategoryId] FOREIGN KEY ([CategoryId]) REFERENCES [Categories] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_SeriesCategories_Series_SeriesId] FOREIGN KEY ([SeriesId]) REFERENCES [Series] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [TagCategories] (
    [Id] int NOT NULL IDENTITY,
    [TagId] int NOT NULL,
    [SeriesId] int NOT NULL,
    CONSTRAINT [PK_TagCategories] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_TagCategories_Series_SeriesId] FOREIGN KEY ([SeriesId]) REFERENCES [Series] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_TagCategories_Tags_TagId] FOREIGN KEY ([TagId]) REFERENCES [Tags] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [Comments] (
    [Id] int NOT NULL IDENTITY,
    [Discussion] nvarchar(max) NOT NULL,
    [UserId] int NOT NULL,
    [SeriesId] int NOT NULL,
    CONSTRAINT [PK_Comments] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Comments_Series_SeriesId] FOREIGN KEY ([SeriesId]) REFERENCES [Series] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Comments_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [Histories] (
    [Id] int NOT NULL IDENTITY,
    [UserId] int NOT NULL,
    [VideoId] int NOT NULL,
    CONSTRAINT [PK_Histories] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Histories_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [Impressions] (
    [Id] int NOT NULL IDENTITY,
    [IsRecommended] bit NOT NULL,
    [Rating] int NOT NULL,
    [SeriesId] int NOT NULL,
    [UserId] int NOT NULL,
    CONSTRAINT [PK_Impressions] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Impressions_Series_SeriesId] FOREIGN KEY ([SeriesId]) REFERENCES [Series] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Impressions_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [Wishlists] (
    [Id] int NOT NULL IDENTITY,
    [SeriesId] int NOT NULL,
    [UserId] int NOT NULL,
    CONSTRAINT [PK_Wishlists] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Wishlists_Series_SeriesId] FOREIGN KEY ([SeriesId]) REFERENCES [Series] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Wishlists_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [Videos] (
    [Id] int NOT NULL IDENTITY,
    [VideoUrl] nvarchar(max) NOT NULL,
    [Duration] int NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    [ViewCount] int NOT NULL,
    [EpisodeId] int NOT NULL,
    [HistoryId] int NULL,
    CONSTRAINT [PK_Videos] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Videos_Episodes_EpisodeId] FOREIGN KEY ([EpisodeId]) REFERENCES [Episodes] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Videos_Histories_HistoryId] FOREIGN KEY ([HistoryId]) REFERENCES [Histories] ([Id])
);

CREATE INDEX [IX_Comments_SeriesId] ON [Comments] ([SeriesId]);

CREATE INDEX [IX_Comments_UserId] ON [Comments] ([UserId]);

CREATE INDEX [IX_Episodes_SeriesId] ON [Episodes] ([SeriesId]);

CREATE INDEX [IX_Histories_UserId] ON [Histories] ([UserId]);

CREATE INDEX [IX_Impressions_SeriesId] ON [Impressions] ([SeriesId]);

CREATE INDEX [IX_Impressions_UserId] ON [Impressions] ([UserId]);

CREATE INDEX [IX_SeriesCategories_CategoryId] ON [SeriesCategories] ([CategoryId]);

CREATE INDEX [IX_SeriesCategories_SeriesId] ON [SeriesCategories] ([SeriesId]);

CREATE INDEX [IX_TagCategories_SeriesId] ON [TagCategories] ([SeriesId]);

CREATE INDEX [IX_TagCategories_TagId] ON [TagCategories] ([TagId]);

CREATE INDEX [IX_Videos_EpisodeId] ON [Videos] ([EpisodeId]);

CREATE INDEX [IX_Videos_HistoryId] ON [Videos] ([HistoryId]);

CREATE INDEX [IX_Wishlists_SeriesId] ON [Wishlists] ([SeriesId]);

CREATE INDEX [IX_Wishlists_UserId] ON [Wishlists] ([UserId]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250425072134_initial', N'9.0.0');

DROP INDEX [IX_Videos_EpisodeId] ON [Videos];

CREATE UNIQUE INDEX [IX_Videos_EpisodeId] ON [Videos] ([EpisodeId]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250425082619_updateschema', N'9.0.0');

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250426041606_initialSetup', N'9.0.0');

COMMIT;
GO

