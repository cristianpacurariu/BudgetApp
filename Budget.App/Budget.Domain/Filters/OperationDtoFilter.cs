using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Budget.Domain.Filters
{
    public class OperationDtoFilter
    {
        public int? IdAccount { get; set; }
        public int? IdCategory { get; set; }
        public int? IdCurrency { get; set; }

        public decimal? AmmountFrom { get; set; }
        public decimal? AmmountTo { get; set; }

        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }

    }
}
