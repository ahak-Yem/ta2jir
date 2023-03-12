using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ta2jir.Models;

public partial class Ta2jirContext : DbContext
{
    public Ta2jirContext()
    {
    }

    public Ta2jirContext(DbContextOptions<Ta2jirContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Admin> Admins { get; set; }

    public virtual DbSet<Case> Cases { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Deal> Deals { get; set; }

    public virtual DbSet<Negotitation> Negotitations { get; set; }

    public virtual DbSet<Offer> Offers { get; set; }

    public virtual DbSet<Picture> Pictures { get; set; }

    public virtual DbSet<Request> Requests { get; set; }

    public virtual DbSet<User> Users { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb3_general_ci")
            .HasCharSet("utf8mb3");

        modelBuilder.Entity<Admin>(entity =>
        {
            entity.HasKey(e => e.AdminId).HasName("PRIMARY");

            entity.ToTable("admins");

            entity.HasIndex(e => e.AdminId, "AdminID_UNIQUE").IsUnique();
            entity.HasIndex(e => e.Email, "Email_UNIQUE").IsUnique();
            entity.Property(e => e.AdminId).HasColumnName("AdminID");
            entity.Property(e => e.ClassificationLevel)
                .HasDefaultValueSql("'1'")
                .HasColumnName("classificationLevel");
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.Rating)
                .HasColumnType("enum('1','2','3','4','5')")
                .HasColumnName("rating");
            entity.Property(e => e.Email).HasMaxLength(320);
        });

        modelBuilder.Entity<Case>(entity =>
        {
            entity.HasKey(e => e.CaseId).HasName("PRIMARY");

            entity.ToTable("cases");

            entity.HasIndex(e => e.AdminId, "fk_Cases_Admins1_idx");

            entity.HasIndex(e => e.DealsId, "fk_Cases_Deals1_idx");

            entity.Property(e => e.CaseId).HasColumnName("CaseID");
            entity.Property(e => e.AdminId).HasColumnName("AdminID");
            entity.Property(e => e.DealsId).HasColumnName("DealsID");
            entity.Property(e => e.Decision).HasColumnType("mediumtext");
            entity.Property(e => e.IsSolved)
                .HasDefaultValueSql("'no'")
                .HasColumnType("enum('yes','no')")
                .HasColumnName("isSolved");
            entity.Property(e => e.State)
                .HasColumnType("enum('reported','processing','decided','closed')")
                .HasColumnName("state");
            entity.Property(e => e.Subject).HasMaxLength(45);

            entity.HasOne(d => d.Admin).WithMany(p => p.Cases)
                .HasForeignKey(d => d.AdminId)
                .HasConstraintName("fk_Cases_Admins1");

            entity.HasOne(d => d.Deals).WithMany(p => p.Cases)
                .HasForeignKey(d => d.DealsId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_Cases_Deals1");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategorieId).HasName("PRIMARY");

            entity.ToTable("categories");

            entity.HasIndex(e => e.Name, "Name_UNIQUE").IsUnique();

            entity.Property(e => e.CategorieId).HasColumnName("CategorieID");
            entity.Property(e => e.Name).HasMaxLength(45);
        });

        modelBuilder.Entity<Deal>(entity =>
        {
            entity.HasKey(e => e.DealsId).HasName("PRIMARY");

            entity.ToTable("deals");

            entity.HasIndex(e => e.RequestId, "fk_Deals_Requests1_idx");

            entity.Property(e => e.DealsId)
                .ValueGeneratedNever()
                .HasColumnName("DealsID");
            entity.Property(e => e.OwnerRating).HasColumnType("enum('1','2','3','4','5')");
            entity.Property(e => e.RenterRating).HasColumnType("enum('1','2','3','4','5')");
            entity.Property(e => e.RequestId).HasColumnName("RequestID");
            entity.Property(e => e.State)
                .HasColumnType("enum('agreed','rented','late','defect','returned','closed')")
                .HasColumnName("state");

            entity.HasOne(d => d.Request).WithMany(p => p.Deals)
                .HasForeignKey(d => d.RequestId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_Deals_Requests1");
        });

        modelBuilder.Entity<Negotitation>(entity =>
        {
            entity.HasKey(e => e.NegotitationId).HasName("PRIMARY");

            entity.ToTable("negotitations");

            entity.HasIndex(e => e.NegotitationId, "NegotitationID_UNIQUE").IsUnique();

            entity.Property(e => e.NegotitationId).HasColumnName("NegotitationID");
            entity.Property(e => e.LastPrice).HasComment("price agreed on");
            entity.Property(e => e.OwnerPrice).HasComment("last price offered from owner");
            entity.Property(e => e.RenterPrice).HasComment("last price offered from renter");
            entity.Property(e => e.State)
                .HasDefaultValueSql("'happening'")
                .HasColumnType("enum('happening','agreed','failed','canceled')")
                .HasColumnName("state");
        });

        modelBuilder.Entity<Offer>(entity =>
        {
            entity.HasKey(e => e.OfferId).HasName("PRIMARY");

            entity.ToTable("offers");

            entity.HasIndex(e => e.OfferId, "OfferID_UNIQUE").IsUnique();

            entity.HasIndex(e => e.CategorieId, "fk_Offers_Categories1_idx");

            entity.HasIndex(e => e.OwnerId, "fk_Offers_Users1_idx");

            entity.Property(e => e.OfferId).HasColumnName("OfferID");
            entity.Property(e => e.CategorieId).HasColumnName("CategorieID");
            entity.Property(e => e.Defects)
                .HasColumnType("mediumtext")
                .HasColumnName("defects");
            entity.Property(e => e.Deposit).HasColumnType("double unsigned");
            entity.Property(e => e.ObjName).HasMaxLength(80);
            entity.Property(e => e.OtherDetails)
                .HasColumnType("mediumtext")
                .HasColumnName("otherDetails");
            entity.Property(e => e.OwnerId).HasColumnName("OwnerID");
            entity.Property(e => e.Price).HasColumnType("double unsigned");
            entity.Property(e => e.State)
                .HasDefaultValueSql("'available'")
                .HasColumnType("enum('available','booked','defect','deleted','blocked','underinvestigation')")
                .HasColumnName("state");

            entity.HasOne(d => d.Categorie).WithMany(p => p.Offers)
                .HasForeignKey(d => d.CategorieId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_Offers_Categories1");

            entity.HasOne(d => d.Owner).WithMany(p => p.Offers)
                .HasForeignKey(d => d.OwnerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_Offers_Users1");
        });

        modelBuilder.Entity<Picture>(entity =>
        {
            entity.HasKey(e => new { e.PictureId, e.OfferId })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

            entity.ToTable("pictures");

            entity.HasIndex(e => e.OfferId, "fk_Pictures_Offers1_idx");

            entity.Property(e => e.PictureId)
                .ValueGeneratedOnAdd()
                .HasColumnName("PictureID");
            entity.Property(e => e.OfferId).HasColumnName("OfferID");
            entity.Property(e => e.PictureBytes).HasColumnType("blob");
            entity.Property(e => e.PictureName).HasMaxLength(45);

            entity.HasOne(d => d.Offer).WithMany(p => p.Pictures)
                .HasForeignKey(d => d.OfferId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_Pictures_Offers1");
        });

        modelBuilder.Entity<Request>(entity =>
        {
            entity.HasKey(e => e.RequestId).HasName("PRIMARY");

            entity.ToTable("requests");

            entity.HasIndex(e => e.RequestId, "RequestID_UNIQUE").IsUnique();

            entity.HasIndex(e => e.NegotitationId, "fk_Requests_Negotitations1_idx");

            entity.HasIndex(e => e.OfferId, "fk_Requests_Offers1_idx");

            entity.HasIndex(e => e.RenterId, "fk_Requests_Users1_idx");

            entity.Property(e => e.RequestId).HasColumnName("RequestID");
            entity.Property(e => e.NegotitationId).HasColumnName("NegotitationID");
            entity.Property(e => e.OfferId).HasColumnName("OfferID");
            entity.Property(e => e.RenterId).HasColumnName("RenterID");
            entity.Property(e => e.State)
                .HasDefaultValueSql("'waiting'")
                .HasColumnType("enum('waiting','approved','rejected','canceled','expired')")
                .HasColumnName("state");

            entity.HasOne(d => d.Negotitation).WithMany(p => p.Requests)
                .HasForeignKey(d => d.NegotitationId)
                .HasConstraintName("fk_Requests_Negotitations1");

            entity.HasOne(d => d.Offer).WithMany(p => p.Requests)
                .HasForeignKey(d => d.OfferId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_Requests_Offers1");

            entity.HasOne(d => d.Renter).WithMany(p => p.Requests)
                .HasForeignKey(d => d.RenterId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_Requests_Users1");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PRIMARY");

            entity.ToTable("users");

            entity.HasIndex(e => e.Email, "Email_UNIQUE").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.Email).HasMaxLength(320);
            entity.Property(e => e.IsBlocked)
                .HasDefaultValueSql("'no'")
                .HasColumnType("enum('yes','no')")
                .HasColumnName("isBlocked");
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.ProfilePic).HasMaxLength(320);
            entity.Property(e => e.UserRating).HasColumnType("double unsigned");
            entity.Property(e => e.Password).HasMaxLength(100);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
