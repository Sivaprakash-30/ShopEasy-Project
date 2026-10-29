using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ShopEasy.Models;

public partial class ShopEasyContext : DbContext
{
    public ShopEasyContext()
    {
    }

    public ShopEasyContext(DbContextOptions<ShopEasyContext> options)
        : base(options)
    {
    }

    public virtual DbSet<CustomerReg> CustomerRegs { get; set; }

    public virtual DbSet<LoginTb> LoginTbs { get; set; }

    public virtual DbSet<SellerReg> SellerRegs { get; set; }

    public virtual DbSet<ProductCategory> ProductCategories { get; set; }
     
    public virtual DbSet<ProductType> ProductTypes { get; set; }
    public virtual DbSet<Stock> Stocks { get; set; }
    public virtual DbSet<Cart> Carts { get; set; }
    public virtual DbSet<wishlist> wishlists { get; set; }
    public virtual DbSet<Addresses> Addresses { get; set; }
    public virtual DbSet<Order> Orders { get; set; }
    public virtual DbSet<Feedback> Feedbacks { get; set; } 







    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {

    }
       

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CustomerReg>(entity =>
        {
            entity.HasKey(e => e.RegId);

            entity.ToTable("Customer_Reg");

            entity.Property(e => e.RegId).HasColumnName("RegID");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.UserName)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<LoginTb>(entity =>
        {
            entity.HasKey(e => e.LoginId);

            entity.ToTable("Login_TB");

            entity.Property(e => e.LoginId).HasColumnName("LoginID");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Password)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<SellerReg>(entity =>
        {
            entity.HasKey(e => e.SId);

            entity.ToTable("Seller_Reg");

            entity.Property(e => e.SId).HasColumnName("sID");
            entity.Property(e => e.CompanyName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.SellerName)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
