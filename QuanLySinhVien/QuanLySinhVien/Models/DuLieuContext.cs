    
#nullable disable
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace QuanLySinhVien.Models;

public partial class DuLieuContext : DbContext
{
    public DuLieuContext(DbContextOptions<DuLieuContext> options)
        : base(options)
    {
    }

    public virtual DbSet<SinhVien> SinhViens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SinhVien>(entity =>
        {
            entity.HasKey(e => e.MaSinhVien).HasName("PK__SinhVien__939AE775427DA4A6");

            entity.ToTable("SinhVien");

            entity.Property(e => e.MaSinhVien).HasMaxLength(10);
            entity.Property(e => e.DiemTrungBinh)
                .IsRequired()
                .HasMaxLength(8);
            entity.Property(e => e.TenSinhVien)
                .IsRequired()
                .HasColumnType("ntext");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}