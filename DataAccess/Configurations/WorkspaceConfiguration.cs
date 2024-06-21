using Domain;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Configurations
{
    internal class WorkspaceConfiguration : EntityConfiguration<Workspace>
    {
        protected override void ConfigureEntity(EntityTypeBuilder<Workspace> builder)
        {
            builder.Property(w => w.Name)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(w => w.Type)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(20);

            builder.HasOne(w => w.Owner)
                .WithMany(u => u.Workspaces)
                .HasForeignKey(w => w.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(w => w.Parent)
                .WithMany(p => p.Children)
                .HasForeignKey(w => w.ParentId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
