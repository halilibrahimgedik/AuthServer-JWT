using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Core.IUnitOfWork
{
    public interface IUnitOfWork
    {

        Task SaveChangesAsync();

        void SaveChanges();
    }
}
