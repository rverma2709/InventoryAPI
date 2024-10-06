using Microsoft.EntityFrameworkCore;
using Root.Models.LogTables;
using Root.Models.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Root.Services.DBContext
{
    public class DBEntities : DbContext
    {
        public DBEntities()
        {

        }

        public DBEntities(DbContextOptions<DBEntities> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    
        public DbSet<InventoryUser> InventoryUsers { get; set; }
        public DbSet<DeviceType> DeviceTypes { get; set; }
        public DbSet<DeviceModeldetail> DeviceModeldetails { get; set; }
        public DbSet<BrandDetail> BrandDetails {  get; set; }
        public DbSet<DeviceProcessorDetail> DeviceProcessorDetails { get; set; }
        public DbSet<GenerationDetail> GenerationDetails { get; set; }
        public DbSet<RAMDetail> RAMDetails { get; set; }
        public DbSet<HardDiskDetail> HardDiskDetails { get; set; }
        public DbSet<ProcurementType> ProcurementTypes { get; set; }
        public DbSet<VendorDetail> VendorDetails { get; set; }
        public DbSet<BulkFileRecivingDetail> BulkFileRecivingDetails { get; set; }
        public DbSet<AllowedFile> AllowedFiles { get; set; }
        public DbSet<PoDetail> PoDetails {  get; set; }
        public DbSet<PoItemDetail> PoItemDetails { get; set; }
        public DbSet<OTP> OTPs { get; set; }
        public DbSet<EmailLog> EmailLogs { get; set; }
        public DbSet<InventoryRole> InventoryRoles { get; set; }
        public DbSet<InquiryDetail> InquiryDetails { get; set; }
    }
}
