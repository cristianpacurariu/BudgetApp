using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Budget.Infrastructure.Repositories.Generic
{
    public interface IFilterRepo<T, F>
    {
        List<T> Filter(F filter);
    }
}
