using domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace infrastructure.Data.Interceptor
{
    public class SeriesFormatInterceptor : SaveChangesInterceptor
    {
        public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {
            if (eventData.Context != null)
            {
                var context = eventData.Context;
                var seriesToUpdate = new Dictionary<int, Series>();

                // Identify Series related to added, deleted, or modified Episodes
                var episodeEntries = context.ChangeTracker.Entries<Episode>()
                    .Where(e => e.State == EntityState.Added || e.State == EntityState.Deleted);

                foreach (var entry in episodeEntries)
                {
                    if (entry.Entity.SeriesId != 0 && !seriesToUpdate.ContainsKey(entry.Entity.SeriesId))
                    {
                        var series = await context.Set<Series>()
                            .Include(s => s.Episodes)
                            .FirstOrDefaultAsync(s => s.Id == entry.Entity.SeriesId, cancellationToken);
                        if (series != null)
                        {
                            seriesToUpdate[series.Id] = series;
                        }
                    }
                }

                // Also check for newly added Series
                // var addedSeriesEntries = context.ChangeTracker.Entries<Series>()
                //     .Where(e => e.State == EntityState.Added && e.Entity.Episodes != null);

                // foreach (var entry in addedSeriesEntries)
                // {
                //     if (!seriesToUpdate.ContainsKey(entry.Entity.Id))
                //     {
                //         seriesToUpdate[entry.Entity.Id] = entry.Entity;
                //     }
                // }

                // Update the SeriesFormat for the identified Series
                foreach (var (_, series) in seriesToUpdate)
                {
                    series.SeriesFormat = series.Episodes.Count switch
                    {
                        0 => SeriesFormat.None,
                        1 => SeriesFormat.Single,
                        _ => SeriesFormat.Series
                    };
                }
            }

            return await base.SavingChangesAsync(eventData, result, cancellationToken);
        }
    }
}