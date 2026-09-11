using CulinaryBlog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CulinaryBlog.Infrastructure.Persistence.Configurations;

public class RecipeIngredientConfiguration : IEntityTypeConfiguration<RecipeIngredient>
{
    public void Configure(EntityTypeBuilder<RecipeIngredient> builder)
    {
        builder.ToTable("RecipeIngredients");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Quantity).HasPrecision(10, 3);
        builder.Property(x => x.Unit).HasMaxLength(50);
        builder.Property(x => x.Notes).HasMaxLength(500);

        builder.HasOne(x => x.Recipe)
            .WithMany(r => r.Ingredients)
            .HasForeignKey(x => x.RecipeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.RecipeId);

        builder.Property(x => x.RowVersion).IsRowVersion();
    }
}
