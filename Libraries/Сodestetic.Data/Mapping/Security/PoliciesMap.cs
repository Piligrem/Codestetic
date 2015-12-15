using System;
using System.Data.Entity.ModelConfiguration;
using Codestetic.Web.Core.Domain.Security;

namespace Codestetic.Web.Data.Mapping.Security
{
    public class PoliciesMap : EntityTypeConfiguration<Policies>
    {
        public PoliciesMap()
        {
            this.ToTable("Policies");
            this.HasKey(c => c.Id);
        }
    }
}