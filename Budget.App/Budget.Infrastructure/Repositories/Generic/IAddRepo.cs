using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Budget.Infrastructure.Repositories.Generic
{
    public interface IAddRepo<T>
    {
        int Add(T item);
    }
}
