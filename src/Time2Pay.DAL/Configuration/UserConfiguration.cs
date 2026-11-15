using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Time2Pay.DAL.Entities;
using Time2Pay.Shared;

namespace Time2Pay.DAL.Configuration;

// public class UserConfiguration : IEntityTypeConfiguration<User>
// {
//     public void Configure(EntityTypeBuilder<User> builder)
//     {
//         builder.HasKey(user => user.Id);
//
//         builder.Property(wd => wd.Id).HasMaxLength(Constants.IdMaxLength);
//
//         builder.Property(wd => wd.ExternalId).HasMaxLength(Constants.IdMaxLength);
//
//         builder.Property(user => user.Name).HasMaxLength(Constants.NameMaxLength);
//
//         builder.Property(user => user.Email).HasMaxLength(Constants.EmailMaxLength);
//
//         builder.HasIndex(user => user.Email).IsUnique();
//
//         builder.HasIndex(user => user.ExternalId).IsUnique();
//     }
// }
