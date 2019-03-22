using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace ESFA.DC.NCS.EF.Interfaces
{
    public interface INcsContext : IDisposable
    {
        DbSet<NcsSubmission> NcsSubmissions { get; set; }

        DatabaseFacade Database { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
