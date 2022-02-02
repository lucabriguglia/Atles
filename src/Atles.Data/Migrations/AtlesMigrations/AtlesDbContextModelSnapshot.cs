﻿// <auto-generated />
using System;
using Atles.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Atles.Data.Migrations.AtlesMigrations
{
    [DbContext(typeof(AtlesDbContext))]
    partial class AtlesDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("Atles.Domain.Models.Category", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("PermissionSetId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("RepliesCount")
                        .HasColumnType("int");

                    b.Property<Guid>("SiteId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("SortOrder")
                        .HasColumnType("int");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<int>("TopicsCount")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("PermissionSetId");

                    b.HasIndex("SiteId");

                    b.ToTable("Category", (string)null);
                });

            modelBuilder.Entity("Atles.Domain.Models.Event", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Data")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("SiteId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("TargetId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("TargetType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("TimeStamp")
                        .HasColumnType("datetime2");

                    b.Property<string>("Type")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Event", (string)null);
                });

            modelBuilder.Entity("Atles.Domain.Models.Forum", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("CategoryId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("LastPostId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("PermissionSetId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("RepliesCount")
                        .HasColumnType("int");

                    b.Property<string>("Slug")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("SortOrder")
                        .HasColumnType("int");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<int>("TopicsCount")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.HasIndex("LastPostId");

                    b.HasIndex("PermissionSetId");

                    b.ToTable("Forum", (string)null);
                });

            modelBuilder.Entity("Atles.Domain.Models.PermissionSet", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("SiteId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("PermissionSet", (string)null);
                });

            modelBuilder.Entity("Atles.Domain.Models.Post", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Content")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("CreatedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("ForumId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("HasAnswer")
                        .HasColumnType("bit");

                    b.Property<bool>("IsAnswer")
                        .HasColumnType("bit");

                    b.Property<Guid?>("LastReplyId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("Locked")
                        .HasColumnType("bit");

                    b.Property<Guid?>("ModifiedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("ModifiedOn")
                        .HasColumnType("datetime2");

                    b.Property<bool>("Pinned")
                        .HasColumnType("bit");

                    b.Property<int>("RepliesCount")
                        .HasColumnType("int");

                    b.Property<string>("Slug")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<string>("Title")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("TopicId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("CreatedBy");

                    b.HasIndex("ForumId");

                    b.HasIndex("LastReplyId");

                    b.HasIndex("ModifiedBy");

                    b.HasIndex("TopicId");

                    b.ToTable("Post", (string)null);
                });

            modelBuilder.Entity("Atles.Domain.Models.PostReaction", b =>
                {
                    b.Property<Guid>("PostId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("TimeStamp")
                        .HasColumnType("datetime2");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.HasKey("PostId", "UserId");

                    b.HasIndex("UserId");

                    b.ToTable("PostReaction", (string)null);
                });

            modelBuilder.Entity("Atles.Domain.Models.PostReactionSummary", b =>
                {
                    b.Property<Guid>("PostId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.Property<int>("Count")
                        .HasColumnType("int");

                    b.HasKey("PostId", "Type");

                    b.ToTable("PostReactionSummary", (string)null);
                });

            modelBuilder.Entity("Atles.Domain.Models.Site", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("AdminCss")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("AdminTheme")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("HeadScript")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Language")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Privacy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PublicCss")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PublicTheme")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Terms")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Title")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Site", (string)null);
                });

            modelBuilder.Entity("Atles.Domain.Models.Subscription", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("ItemId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.HasKey("UserId", "ItemId");

                    b.ToTable("Subscription", (string)null);
                });

            modelBuilder.Entity("Atles.Domain.Models.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("DisplayName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("IdentityUserId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("RepliesCount")
                        .HasColumnType("int");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<DateTime>("TimeStamp")
                        .HasColumnType("datetime2");

                    b.Property<int>("TopicsCount")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("User", (string)null);
                });

            modelBuilder.Entity("Atles.Domain.Models.Category", b =>
                {
                    b.HasOne("Atles.Domain.Models.PermissionSet", "PermissionSet")
                        .WithMany("Categories")
                        .HasForeignKey("PermissionSetId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("Atles.Domain.Models.Site", "Site")
                        .WithMany("Categories")
                        .HasForeignKey("SiteId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("PermissionSet");

                    b.Navigation("Site");
                });

            modelBuilder.Entity("Atles.Domain.Models.Event", b =>
                {
                    b.HasOne("Atles.Domain.Models.User", "User")
                        .WithMany("Events")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.NoAction);

                    b.Navigation("User");
                });

            modelBuilder.Entity("Atles.Domain.Models.Forum", b =>
                {
                    b.HasOne("Atles.Domain.Models.Category", "Category")
                        .WithMany("Forums")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("Atles.Domain.Models.Post", "LastPost")
                        .WithMany()
                        .HasForeignKey("LastPostId");

                    b.HasOne("Atles.Domain.Models.PermissionSet", "PermissionSet")
                        .WithMany("Forums")
                        .HasForeignKey("PermissionSetId")
                        .OnDelete(DeleteBehavior.NoAction);

                    b.Navigation("Category");

                    b.Navigation("LastPost");

                    b.Navigation("PermissionSet");
                });

            modelBuilder.Entity("Atles.Domain.Models.PermissionSet", b =>
                {
                    b.OwnsMany("Atles.Domain.Models.Permission", "Permissions", b1 =>
                        {
                            b1.Property<Guid>("PermissionSetId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<string>("RoleId")
                                .HasColumnType("nvarchar(450)");

                            b1.Property<int>("Type")
                                .HasColumnType("int");

                            b1.HasKey("PermissionSetId", "RoleId", "Type");

                            b1.ToTable("Permission", (string)null);

                            b1.WithOwner()
                                .HasForeignKey("PermissionSetId");
                        });

                    b.Navigation("Permissions");
                });

            modelBuilder.Entity("Atles.Domain.Models.Post", b =>
                {
                    b.HasOne("Atles.Domain.Models.User", "CreatedByUser")
                        .WithMany("Posts")
                        .HasForeignKey("CreatedBy")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("Atles.Domain.Models.Forum", "Forum")
                        .WithMany("Posts")
                        .HasForeignKey("ForumId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("Atles.Domain.Models.Post", "LastReply")
                        .WithMany()
                        .HasForeignKey("LastReplyId");

                    b.HasOne("Atles.Domain.Models.User", "ModifiedByUser")
                        .WithMany()
                        .HasForeignKey("ModifiedBy");

                    b.HasOne("Atles.Domain.Models.Post", "Topic")
                        .WithMany()
                        .HasForeignKey("TopicId");

                    b.Navigation("CreatedByUser");

                    b.Navigation("Forum");

                    b.Navigation("LastReply");

                    b.Navigation("ModifiedByUser");

                    b.Navigation("Topic");
                });

            modelBuilder.Entity("Atles.Domain.Models.PostReaction", b =>
                {
                    b.HasOne("Atles.Domain.Models.Post", "Post")
                        .WithMany("PostReactions")
                        .HasForeignKey("PostId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("Atles.Domain.Models.User", "User")
                        .WithMany("PostReactions")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Post");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Atles.Domain.Models.PostReactionSummary", b =>
                {
                    b.HasOne("Atles.Domain.Models.Post", "Post")
                        .WithMany("PostReactionSummaries")
                        .HasForeignKey("PostId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Post");
                });

            modelBuilder.Entity("Atles.Domain.Models.Subscription", b =>
                {
                    b.HasOne("Atles.Domain.Models.User", "User")
                        .WithMany("Subscriptions")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("Atles.Domain.Models.Category", b =>
                {
                    b.Navigation("Forums");
                });

            modelBuilder.Entity("Atles.Domain.Models.Forum", b =>
                {
                    b.Navigation("Posts");
                });

            modelBuilder.Entity("Atles.Domain.Models.PermissionSet", b =>
                {
                    b.Navigation("Categories");

                    b.Navigation("Forums");
                });

            modelBuilder.Entity("Atles.Domain.Models.Post", b =>
                {
                    b.Navigation("PostReactionSummaries");

                    b.Navigation("PostReactions");
                });

            modelBuilder.Entity("Atles.Domain.Models.Site", b =>
                {
                    b.Navigation("Categories");
                });

            modelBuilder.Entity("Atles.Domain.Models.User", b =>
                {
                    b.Navigation("Events");

                    b.Navigation("PostReactions");

                    b.Navigation("Posts");

                    b.Navigation("Subscriptions");
                });
#pragma warning restore 612, 618
        }
    }
}
