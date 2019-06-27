using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Budget.Domain.Repositories
{
    public class CurrencyDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual List<AccountDto> Accounts { get; set; }
    }
}
