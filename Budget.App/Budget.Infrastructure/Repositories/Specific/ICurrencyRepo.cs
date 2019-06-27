using Budget.Infrastructure.Repositories.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Budget.Infrastructure.Repositories.Specific
{
    public interface ICurrencyRepo<T> : IGetRepo<T>, IListRepo<T>
    {
    }
}
