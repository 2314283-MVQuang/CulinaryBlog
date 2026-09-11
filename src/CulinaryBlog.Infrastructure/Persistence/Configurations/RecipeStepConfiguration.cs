using CulinaryBlog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CulinaryBlog.Infrastructure.Persistence.Configurations;

public class RecipeStepConfiguration : IEntityTypeConfiguration<RecipeStep>
{
    public void Configure(EntityTypeBuilder<RecipeStep> builder)
    {
        builder.ToTable("RecipeSteps");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Title).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Description).IsRequired();
        builder.Property(x => x.ImageUrl).HasMaxLength(500);

        builder.HasOne(x => x.Recipe)
            .WithMany(r => r.Steps)
            .HasForeignKey(x => x.RecipeId)
            .OnDelete(DeleteBehavior.Cascade);

        // Trong 1 Recipe không được có 2 bước cùng StepNumber.
        builder.HasIndex(x => new { x.RecipeId, x.StepNumber }).IsUnique();

        builder.Property(x => x.RowVersion).IsRowVersion();
    }
}
