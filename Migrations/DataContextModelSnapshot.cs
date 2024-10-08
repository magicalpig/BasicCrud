﻿// <auto-generated />
using System;
using BasicCrud.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace BasicCrud.Migrations
{
    [DbContext(typeof(DataContext))]
    partial class DataContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.6");

            modelBuilder.Entity("BasicCrud.Models.Composer", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Composers", (string)null);
                });

            modelBuilder.Entity("BasicCrud.Models.Composition", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<Guid>("ComposerId")
                        .HasColumnType("TEXT");

                    b.Property<int>("Format")
                        .HasColumnType("INTEGER");

                    b.Property<int>("KeySignature")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("NumberOfMovements")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("ComposerId");

                    b.ToTable("Compositions", (string)null);
                });

            modelBuilder.Entity("BasicCrud.Models.Composition", b =>
                {
                    b.HasOne("BasicCrud.Models.Composer", "Composer")
                        .WithMany("Compositions")
                        .HasForeignKey("ComposerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Composer");
                });

            modelBuilder.Entity("BasicCrud.Models.Composer", b =>
                {
                    b.Navigation("Compositions");
                });
#pragma warning restore 612, 618
        }
    }
}
