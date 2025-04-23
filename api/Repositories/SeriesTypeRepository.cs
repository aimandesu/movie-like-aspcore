using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Interfaces;
using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Repositories
{
    public class SeriesTypeRepository : ISeriesTypeRepository
    {
        private readonly ApplicationDbContext _context;
        public SeriesTypeRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<List<SeriesType>> GetAllAsync()
        {
            return await _context.SeriesTypes.ToListAsync();
        }
        
    }
}