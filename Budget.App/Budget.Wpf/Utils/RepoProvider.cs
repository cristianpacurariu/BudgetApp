using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Budget.Domain.Repositories;
using Budget.Infrastructure.Repositories.Specific;
using Budget.Repositories;

namespace Budget.Wpf
{
    public static class RepoProvider
    {
        internal static IAccountRepo<AccountDto> GetAccountRepo()
        {
            return new AccountRepo();
        }

        internal static IOperationTypeRepo<OperationTypeDto> GetOperationTypeRepo()
        {
            return new OperationTypeRepo();
        }

        internal static IOperationRepo<OperationDto> GetOperationRepo()
        {
            return new OperationRepo();
        }

        internal static ICurrencyRepo<CurrencyDto> GetCurrencyRepo()
        {
            return new CurrencyRepo();
        }
    }
}
