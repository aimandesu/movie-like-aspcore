using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Dtos.Series;
using api.Helpers;
using api.Interfaces;
using api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Repositories
{
    public class SeriesRepository : ISeriesRepository
    {
        private readonly ApplicationDbContext _context;
        public SeriesRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Series?> CreateSeries(Series series, IFormFile thumbnail)
        {
            if (thumbnail != null && thumbnail.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                Directory.CreateDirectory(uploadsFolder); 

                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(thumbnail.FileName); 
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using var stream = new FileStream(filePath, FileMode.Create);
                await thumbnail.CopyToAsync(stream);

                series.Thumbnail = $"/uploads/{uniqueFileName}"; 
            }

            await _context.Series.AddAsync(series);
            await _context.SaveChangesAsync();
            return series;
        }


        public async Task<Series?> DeleteSeries(int id)
        {
            var series = await _context.Series.FirstOrDefaultAsync(e => e.Id == id);

            if(series == null)
            {
                return null;
            }

            if (!string.IsNullOrWhiteSpace(series.Thumbnail))
            {
                var thumbnailPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", series.Thumbnail.TrimStart('/'));

                if (File.Exists(thumbnailPath))
                {
                    File.Delete(thumbnailPath);
                }
            }

            _context.Series.Remove(series);
            await _context.SaveChangesAsync();
            return series;

        }

        public async Task<List<Series>> GetAllSeries(SeriesQueryObject queryObject)
        {
            var query = _context.Series
                .Include(s => s.SeriesCategories)
                .ThenInclude(sc => sc.Category)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(queryObject.Category))
            {
                query = query.Where(s =>
                        s.SeriesCategories.Any(sc => 
                        sc.Category.Name == queryObject.Category)
                        );
            }

            var skipNumber = (queryObject.PageNumber - 1) * queryObject.PageSize;

            return await query.Skip(skipNumber).Take(queryObject.PageSize).ToListAsync();
        }

        public async Task<Series?> GetSeries(int id)
        {
            return await _context.Series.Include(e => e.Episodes).FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<Series?> UpdateSeries(int id, CreateUpdateSeriesDto dto, IFormFile? thumbnail = null)
        {
            var existingSeries = await _context.Series.FirstOrDefaultAsync(e => e.Id == id);

            if (existingSeries == null)
            {
                return null;
            }

            existingSeries.Title = dto.Title;
            existingSeries.Description = dto.Description;

            if (thumbnail != null && thumbnail.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = $"{Guid.NewGuid()}_{thumbnail.FileName}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using var stream = new FileStream(filePath, FileMode.Create);
                await thumbnail.CopyToAsync(stream);

                existingSeries.Thumbnail = $"/uploads/{uniqueFileName}";
            }

            await _context.SaveChangesAsync();
            return existingSeries;
        }

    }
}