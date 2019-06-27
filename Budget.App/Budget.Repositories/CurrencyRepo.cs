using Budget.Domain.Repositories;
using Budget.Infrastructure.Repositories.Specific;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Budget.DataAccess.Model;
using AutoMapper;

namespace Budget.Repositories
{
    public class CurrencyRepo : ICurrencyRepo<CurrencyDto>
    {

        public List<CurrencyDto> All()
        {
            using (SpendingsEntities context = new SpendingsEntities())
            {
                List<Currency> fromDb = context.Currencies.ToList();

                return Mapper.Map<List<Currency>, List<CurrencyDto>>(fromDb);
            }
        }

        public CurrencyDto Get(int id)
        {
            using (SpendingsEntities context = new SpendingsEntities())
            {
                Currency toGet = context.Currencies.FirstOrDefault(d => d.Id == id);

                if (toGet == null)
                {
                    return null;
                }

                return Mapper.Map<Currency, CurrencyDto>(toGet);
            }
        }
    }
}
