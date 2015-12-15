using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;
using Codestetic.Web.Core.Domain.GPS;
using Codestetic.Web.Core.Domain.Users;
using Codestetic.Web.Core.Domain.Common;
using Codestetic.Web.Core.Domain.Devices;

namespace Codestetic.Web.Data.Mapping.Users
{
    public class UserMap : EntityTypeConfiguration<User>
    {
        public UserMap()
        {
            this.ToTable("User");
            this.HasKey(c => c.Id);
            this.Property(u => u.CreatedOnUtc).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed);
            this.Property(u => u.Username).HasMaxLength(250);
            this.Property(u => u.Email).HasMaxLength(250);

            this.Ignore(u => u.PasswordFormat);

            // Role - Many to Many
            this.HasMany<UserRole>(u => u.UserRoles)
                .WithMany()
                .Map(m => m.ToTable("UserRole_Mapping"));

            this.HasMany<Address>(c => c.Addresses)
                .WithMany()
                .Map(m => 
                {
                    m.ToTable("UserAddresses_Mapping");
                });
            
            //this.HasMany<Address>(c => c.Addresses)
            //    .WithMany()
            //    .Map(m => m.ToTable("Address"));
            //this.HasOptional<Address>(c => c.BillingAddress);

            // Device - Many to Many
            this.HasMany<Device>(u => u.Devices)
                .WithMany(d => d.Users)
                .Map(m =>
                {
                    m.ToTable("UserDevice_Mapping");
                    m.MapLeftKey("UserId");
                    m.MapRightKey("DeviceId");
                });
            // Device - 0..1 to Many
            //this.HasOptional(u => u.Devices)
            //    .WithMany()
            //    .HasForeignKey(u => u.Id);
        }
    }
}