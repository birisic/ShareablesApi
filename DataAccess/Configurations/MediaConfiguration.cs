using Domain;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Configurations
{
    internal class MediaConfiguration : EntityConfiguration<Media>
    {
        protected override void ConfigureEntity(EntityTypeBuilder<Media> builder)
        {
            builder.Property(x => x.Path).IsRequired();
        }
    }
}
