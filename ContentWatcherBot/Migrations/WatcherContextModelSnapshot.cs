﻿// <auto-generated />
using System;
using ContentWatcherBot.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace ContentWatcherBot.Migrations
{
    [DbContext(typeof(WatcherContext))]
    partial class WatcherContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.0-preview1.19506.2");

            modelBuilder.Entity("ContentWatcherBot.Database.Guild", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<ulong>("GuildId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("Guilds");
                });

            modelBuilder.Entity("ContentWatcherBot.Database.GuildWatcher", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<ulong>("ChannelId")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("GuildId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("ServerId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("WatcherId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("GuildId");

                    b.HasIndex("WatcherId");

                    b.ToTable("GuildWatchers");
                });

            modelBuilder.Entity("ContentWatcherBot.Database.Watcher", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Description")
                        .HasColumnType("TEXT");

                    b.Property<string>("Param")
                        .HasColumnType("TEXT");

                    b.Property<string>("PreviousContentIds")
                        .HasColumnType("TEXT");

                    b.Property<string>("Title")
                        .HasColumnType("TEXT");

                    b.Property<int>("Type")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Url")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Watchers");
                });

            modelBuilder.Entity("ContentWatcherBot.Database.GuildWatcher", b =>
                {
                    b.HasOne("ContentWatcherBot.Database.Guild", "Guild")
                        .WithMany("GuildWatchers")
                        .HasForeignKey("GuildId");

                    b.HasOne("ContentWatcherBot.Database.Watcher", "Watcher")
                        .WithMany("GuildWatchers")
                        .HasForeignKey("WatcherId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
