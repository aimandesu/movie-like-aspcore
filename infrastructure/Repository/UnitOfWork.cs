using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using application.IRepository;
using infrastructure.Data;

namespace infrastructure.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public UnitOfWork(
            ApplicationDbContext context
        )
        {
            _context = context;
        }

        public void Dipose()
        {
            _context.Dispose();
        }

        public Task SaveAsync(CancellationToken cancellationToken)
        {
            return _context.SaveChangesAsync(cancellationToken);
        }
    }
}