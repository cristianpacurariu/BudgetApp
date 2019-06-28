using Budget.Infrastructure.Repositories.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Budget.Infrastructure.Repositories.Specific
{
    public interface IAccountRepo<T, F> : IGetRepo<T>, IListRepo<T>, IAddRepo<T>, IDeleteRepo<T>, IUpdateRepo<T>, IFilterRepo<T, F>
    {
        
    }
    
}
