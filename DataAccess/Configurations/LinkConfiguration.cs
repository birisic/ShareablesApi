using Domain;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Configurations
{
    internal class LinkConfiguration : EntityConfiguration<Link>
    {
        protected override void ConfigureEntity(EntityTypeBuilder<Link> builder)
        {
            builder.Property(x => x.Token)
                .HasMaxLength(255)
                .IsRequired();

            builder.HasIndex(x => x.Token).IsUnique();

            builder.Property(x => x.Expires_at)
                .IsRequired();
        }
    }
}
