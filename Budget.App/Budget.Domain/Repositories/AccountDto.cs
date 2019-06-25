using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Budget.Domain.Repositories
{
    public class AccountDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Currency { get; set; }
    }
}
