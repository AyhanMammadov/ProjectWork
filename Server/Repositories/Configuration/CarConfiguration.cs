using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharedLib.Models;

namespace Server.Repositories.Configuration;
public class CarConfiguration: IEntityTypeConfiguration<Car>
{
    public void Configure(EntityTypeBuilder<Car> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Model).IsRequired().HasMaxLength(30);

        builder.Property(c => c.Description).HasDefaultValue("There is no information about this car");

        builder.Property(c => c.PathImage).IsRequired().HasMaxLength(200);
    }
}

