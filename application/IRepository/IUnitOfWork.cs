using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace application.IRepository
{
    public interface IUnitOfWork
    {
        Task SaveAsync(CancellationToken cancellationToken);
        void Dipose();
    }
}