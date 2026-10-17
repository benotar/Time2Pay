using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Time2Pay.DAL.Entities;
using Time2Pay.DAL.Providers.Interfaces;

namespace Time2Pay.DAL.Interceptors;

public class UpdateAuditEntityInterceptor : SaveChangesInterceptor
{
    private readonly IDateTimeOffsetProvider _dateTimeOffsetProvider;

    public UpdateAuditEntityInterceptor(IDateTimeOffsetProvider dateTimeOffsetProvider)
    {
        _dateTimeOffsetProvider = dateTimeOffsetProvider;
    }

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        if (eventData.Context is not null)
        {
            UpdateEntities(eventData.Context);
        }

        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = new CancellationToken())
    {
        if (eventData.Context is not null)
        {
            UpdateEntities(eventData.Context);
        }

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void UpdateEntities(DbContext context)
    {
        var now = _dateTimeOffsetProvider.UtcNow;

        var entries = context.ChangeTracker
            .Entries<AuditEntity>()
            .Where(entry => entry.State is EntityState.Added or EntityState.Modified)
            .ToList();

        if (entries.Count == 0)
        {
            return;
        }

        foreach (var entry in entries)
        {
            var entity = entry.Entity;

            switch (entry.State)
            {
                case EntityState.Added:
                    entity.CreatedAtUtc = now;
                    break;
                case EntityState.Modified:
                    entry.Entity.LastModifiedAtUtc = now;
                    break;
            }
        }
    }
}
