using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Entities;

public partial class HouseManagementContext : DbContext
{
    public HouseManagementContext(DbContextOptions<HouseManagementContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Bill> Bills { get; set; }

    public virtual DbSet<BillService> BillServices { get; set; }

    public virtual DbSet<Contract> Contracts { get; set; }

    public virtual DbSet<House> Houses { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Otpverify> Otpverifies { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Room> Rooms { get; set; }

    public virtual DbSet<Service> Services { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Bill>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("Bill");

            entity.HasIndex(e => e.RoomId, "FK_Bill_Room");

            entity.HasIndex(e => e.CreateBy, "FK_Bill_User");

            entity.Property(e => e.Month).HasMaxLength(6);
            entity.Property(e => e.PaymentDate).HasMaxLength(6);
            entity.Property(e => e.TotalPrice).HasPrecision(19, 4);

            entity.HasOne(d => d.CreateByNavigation).WithMany(p => p.Bills)
                .HasForeignKey(d => d.CreateBy)
                .HasConstraintName("FK_Bill_User");

            entity.HasOne(d => d.Room).WithMany(p => p.Bills)
                .HasForeignKey(d => d.RoomId)
                .HasConstraintName("FK_Bill_Room");
        });

        modelBuilder.Entity<BillService>(entity =>
        {
            entity.HasKey(e => new { e.BillId, e.ServiceId }).HasName("PRIMARY");

            entity.ToTable("BillService");

            entity.HasIndex(e => e.ServiceId, "ServiceId");

            entity.Property(e => e.Quantity).HasPrecision(10);

            entity.HasOne(d => d.Bill).WithMany(p => p.BillServices)
                .HasForeignKey(d => d.BillId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("BillService_ibfk_1");

            entity.HasOne(d => d.Service).WithMany(p => p.BillServices)
                .HasForeignKey(d => d.ServiceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("BillService_ibfk_2");
        });

        modelBuilder.Entity<Contract>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("Contract");

            entity.HasIndex(e => e.RoomId, "FK_Contract_Room");

            entity.HasIndex(e => e.OwnerId, "FK_Contract_User");

            entity.HasIndex(e => e.CustomerId, "FK_Contract_User1");

            entity.Property(e => e.EndDate).HasMaxLength(6);
            entity.Property(e => e.FileUrl)
                .HasMaxLength(250)
                .HasColumnName("FileURL");
            entity.Property(e => e.ImagesUrl)
                .HasMaxLength(250)
                .HasColumnName("ImagesURL");
            entity.Property(e => e.StartDate).HasMaxLength(6);
            entity.Property(e => e.Status).HasMaxLength(50);

            entity.HasOne(d => d.Customer).WithMany(p => p.ContractCustomers)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("FK_Contract_User1");

            entity.HasOne(d => d.Owner).WithMany(p => p.ContractOwners)
                .HasForeignKey(d => d.OwnerId)
                .HasConstraintName("FK_Contract_User");

            entity.HasOne(d => d.Room).WithMany(p => p.Contracts)
                .HasForeignKey(d => d.RoomId)
                .HasConstraintName("FK_Contract_Room");
        });

        modelBuilder.Entity<House>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("House");

            entity.HasIndex(e => e.OwnerId, "FK_House_User");

            entity.Property(e => e.Address).HasMaxLength(50);
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.RoomQuantity).HasComment("Số lượng phòng");
            entity.Property(e => e.Status).HasMaxLength(50);

            entity.HasOne(d => d.Owner).WithMany(p => p.Houses)
                .HasForeignKey(d => d.OwnerId)
                .HasConstraintName("FK_House_User");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("Notification");

            entity.HasIndex(e => e.HouseId, "FK_Notification_House");

            entity.Property(e => e.Content).HasMaxLength(0);

            entity.HasOne(d => d.House).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.HouseId)
                .HasConstraintName("FK_Notification_House");
        });

        modelBuilder.Entity<Otpverify>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("OTPVerify");

            entity.HasIndex(e => e.UserId, "FK_OTPVerify_User");

            entity.Property(e => e.CreatedAt).HasMaxLength(6);
            entity.Property(e => e.ExpiredAt).HasMaxLength(6);
            entity.Property(e => e.OtpCode).HasMaxLength(50);

            entity.HasOne(d => d.User).WithMany(p => p.Otpverifies)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OTPVerify_User");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.HasIndex(e => e.OwnerId, "FK_Payments_User");

            entity.Property(e => e.AccountNumber).HasMaxLength(50);
            entity.Property(e => e.AccountType).HasMaxLength(50);
            entity.Property(e => e.QrcodeImage)
                .HasMaxLength(512)
                .HasColumnName("QRCodeImage");
            entity.Property(e => e.TransferContent).HasMaxLength(100);

            entity.HasOne(d => d.Owner).WithMany(p => p.Payments)
                .HasForeignKey(d => d.OwnerId)
                .HasConstraintName("FK_Payments_User");
        });

        modelBuilder.Entity<Room>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("Room");

            entity.HasIndex(e => e.HouseId, "FK_Room_House");

            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.SecondPassword).HasMaxLength(512);
            entity.Property(e => e.Status).HasMaxLength(50);

            entity.HasOne(d => d.House).WithMany(p => p.Rooms)
                .HasForeignKey(d => d.HouseId)
                .HasConstraintName("FK_Room_House");
        });

        modelBuilder.Entity<Service>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("Service");

            entity.HasIndex(e => e.CreatedBy, "FK_Service_User_idx");

            entity.Property(e => e.Name).HasMaxLength(150);
            entity.Property(e => e.Price).HasPrecision(19, 4);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.Services)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK_Service_User");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("User");

            entity.Property(e => e.Address).HasMaxLength(50);
            entity.Property(e => e.Avatar).HasMaxLength(512);
            entity.Property(e => e.CitizenIdNumber).HasMaxLength(50);
            entity.Property(e => e.CreatedAt).HasMaxLength(6);
            entity.Property(e => e.Dob)
                .HasMaxLength(6)
                .HasColumnName("DOB");
            entity.Property(e => e.Email).HasMaxLength(50);
            entity.Property(e => e.FullName).HasMaxLength(50);
            entity.Property(e => e.Gender).HasMaxLength(50);
            entity.Property(e => e.LastLoggedIn).HasMaxLength(6);
            entity.Property(e => e.LicensePlates)
                .HasMaxLength(50)
                .HasComment("Biển số xe");
            entity.Property(e => e.Otp)
                .HasMaxLength(255)
                .HasColumnName("OTP");
            entity.Property(e => e.Otpexpiration)
                .HasColumnType("datetime")
                .HasColumnName("OTPExpiration");
            entity.Property(e => e.Password).HasMaxLength(512);
            entity.Property(e => e.PhoneNumber).HasMaxLength(50);
            entity.Property(e => e.Role).HasMaxLength(50);
            entity.Property(e => e.Salt).HasMaxLength(512);
            entity.Property(e => e.Status).HasMaxLength(50);

            entity.HasMany(d => d.Rooms).WithMany(p => p.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "UserRoom",
                    r => r.HasOne<Room>().WithMany()
                        .HasForeignKey("RoomId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_UserRoom_Room"),
                    l => l.HasOne<User>().WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_UserRoom_User"),
                    j =>
                    {
                        j.HasKey("UserId", "RoomId").HasName("PRIMARY");
                        j.ToTable("UserRoom");
                        j.HasIndex(new[] { "RoomId" }, "FK_UserRoom_Room");
                    });
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
