using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using application.Common;
using domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace infrastructure.Data.Interceptor
{
    public class SlugInterceptor : SaveChangesInterceptor
    {
        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
        {
            if (eventData.Context != null)
            {
                var entries = eventData.Context.ChangeTracker
                    .Entries<Series>()
                    .Where(e => e.State == EntityState.Added ||
                               (e.State == EntityState.Modified && e.Property(p => p.Title).IsModified));

                foreach (var entry in entries)
                {
                    var series = entry.Entity;
                    if (!string.IsNullOrEmpty(series.Title))
                    {
                        string baseSlug = CustomFunction.GenerateSlug(series.Title);

                        // If it's a new entity and we don't have an ID yet, just set the base slug
                        // The ID will be appended after initial save
                        if (series.Id == 0)
                        {
                            series.Slug = baseSlug;
                        }
                        else
                        {
                            series.Slug = $"{baseSlug}";
                        }
                    }
                }
            }

            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }
    }
}