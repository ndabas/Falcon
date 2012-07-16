using System;
using System.Data.Entity;

namespace FalconWeb.Models
{
    public class FalconContext : DbContext
    {
        public DbSet<Device> Devices { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Tracking> TrackingData { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Ignore<UserViewModel>();
            modelBuilder.Ignore<TrackingReportModel>();

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<GeoArea> GeoAreas { get; set; }
    }
}