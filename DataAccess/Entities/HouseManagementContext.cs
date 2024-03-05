using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Entities;

public partial class HouseManagementContext : DbContext
{
    public HouseManagementContext()
    {
    }

    public HouseManagementContext(DbContextOptions<HouseManagementContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Bill> Bills { get; set; }

    public virtual DbSet<Contract> Contracts { get; set; }

    public virtual DbSet<House> Houses { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Otpverify> Otpverifies { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Room> Rooms { get; set; }

    public virtual DbSet<User> Users { get; set; }

  /*  protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySQL("Data Source=MSI;Initial Catalog=HouseManagement;Integrated Security=True;Encrypt=True;Trust Server Certificate=True");
*/
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Bill>(entity =>
        {
            entity.ToTable("Bill");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.ElectricityUnitPrice)
                .HasComment("Đơn giá điện")
                .HasColumnType("money");
            entity.Property(e => e.ElectricityUsed).HasComment("Khối lượng điện đã sử dụng");
            entity.Property(e => e.Month).HasColumnType("datetime");
            entity.Property(e => e.PaymentDate).HasColumnType("datetime");
            entity.Property(e => e.RentAmount).HasColumnType("money");
            entity.Property(e => e.ServicePrice).HasColumnType("money");
            entity.Property(e => e.TotalPice).HasColumnType("money");
            entity.Property(e => e.WaterUnitPrice)
                .HasComment("Đơn giá nước")
                .HasColumnType("money");
            entity.Property(e => e.WaterUsed).HasComment("Số lượng nước đã sử dụng");

            entity.HasOne(d => d.CreateByNavigation).WithMany(p => p.Bills)
                .HasForeignKey(d => d.CreateBy)
                .HasConstraintName("FK_Bill_User");

            entity.HasOne(d => d.Room).WithMany(p => p.Bills)
                .HasForeignKey(d => d.RoomId)
                .HasConstraintName("FK_Bill_Room");
        });

        modelBuilder.Entity<Contract>(entity =>
        {
            entity.ToTable("Contract");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.EndDate).HasColumnType("datetime");
            entity.Property(e => e.FileUrl)
                .HasMaxLength(250)
                .HasColumnName("FileURL");
            entity.Property(e => e.StartDate).HasColumnType("datetime");

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
            entity.ToTable("House");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Address).HasMaxLength(50);
            entity.Property(e => e.HouseAccount).HasMaxLength(50);
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.RoomQuantity).HasComment("Số lượng phòng");
            entity.Property(e => e.Status).HasMaxLength(50);

            entity.HasOne(d => d.Owner).WithMany(p => p.Houses)
                .HasForeignKey(d => d.OwnerId)
                .HasConstraintName("FK_House_User");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.ToTable("Notification");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.House).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.HouseId)
                .HasConstraintName("FK_Notification_House");
        });

        modelBuilder.Entity<Otpverify>(entity =>
        {
            entity.ToTable("OTPVerify");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.ExpiredAt).HasColumnType("datetime");
            entity.Property(e => e.OtpCode).HasMaxLength(50);

            entity.HasOne(d => d.User).WithMany(p => p.Otpverifies)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OTPVerify_User");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.AccountNumber).HasMaxLength(50);
            entity.Property(e => e.AccountType).HasMaxLength(50);
            entity.Property(e => e.QrcodeImage).HasColumnName("QRCodeImage");
            entity.Property(e => e.TransferContent).HasMaxLength(100);

            entity.HasOne(d => d.Owner).WithMany(p => p.Payments)
                .HasForeignKey(d => d.OwnerId)
                .HasConstraintName("FK_Payments_User");
        });

        modelBuilder.Entity<Room>(entity =>
        {
            entity.ToTable("Room");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.Status).HasMaxLength(50);

            entity.HasOne(d => d.House).WithMany(p => p.Rooms)
                .HasForeignKey(d => d.HouseId)
                .HasConstraintName("FK_Room_House");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("User");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Address).HasMaxLength(50);
            entity.Property(e => e.CitizenIdNumber).HasMaxLength(50);
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.Dob)
                .HasColumnType("datetime")
                .HasColumnName("DOB");
            entity.Property(e => e.Email).HasMaxLength(50);
            entity.Property(e => e.FullName).HasMaxLength(50);
            entity.Property(e => e.Gender).HasMaxLength(50);
            entity.Property(e => e.LastLoggedIn).HasColumnType("datetime");
            entity.Property(e => e.LicensePlates)
                .HasMaxLength(50)
                .HasComment("Biển số xe");
            entity.Property(e => e.PhoneNumber).HasMaxLength(50);
            entity.Property(e => e.Role).HasMaxLength(50);
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
                        j.HasKey("UserId", "RoomId");
                        j.ToTable("UserRoom");
                    });
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
