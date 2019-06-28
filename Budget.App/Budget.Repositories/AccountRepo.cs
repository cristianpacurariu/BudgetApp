using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Budget.DataAccess.Model;
using Budget.Infrastructure.Repositories.Specific;
using Budget.Domain.Repositories;
using AutoMapper;
using System.Data.Entity;
using Budget.Domain.Filters;

namespace Budget.Repositories
{
    public class AccountRepo : IAccountRepo<AccountDto, AccountDtoFilter>
    {
        public int Add(AccountDto item)
        {
            using (SpendingsEntities context = new SpendingsEntities())
            {
                //Account toAdd = new Account();
                //toAdd.Name = item.Name;
                //toAdd.Currency = item.Currency;

                Account toAdd = Mapper.Map<AccountDto, Account>(item);

                context.Accounts.Add(toAdd);
                context.SaveChanges();

                return toAdd.Id;
            }
        }
        public List<AccountDto> All()
        {
            using (SpendingsEntities context = new SpendingsEntities())
            {
                List<Account> fromDb = context.Accounts.ToList();
                return Mapper.Map<List<Account>, List<AccountDto>>(fromDb);

                //List<AccountDto> result = new List<AccountDto>();

                //foreach (Account account in fromDb)
                //{
                //    AccountDto dto = new AccountDto();
                //    dto.Name = account.Name;
                //    dto.Currency = account.Currency;
                //    dto.Id = account.Id;

                //    result.Add(dto);
                //}

                //return result;
            }
        }
        public bool Delete(int id)
        {
            using (SpendingsEntities context = new SpendingsEntities())
            {
                Account toDelete = context.Accounts.FirstOrDefault(d => d.Id == id);

                if (toDelete == null)
                {
                    return false;
                }

                context.Accounts.Remove(toDelete);
                context.SaveChanges();

                return true;
            }
        }
        public AccountDto Get(int id)
        {
            using (SpendingsEntities context = new SpendingsEntities())
            {
                Account fromDb = context.Accounts.FirstOrDefault(d => d.Id == id);
                if (fromDb == null)
                {
                    return null;
                }

                return Mapper.Map<Account, AccountDto>(fromDb);

                //AccountDto result = new AccountDto();
                //result.Name = fromDb.Name;
                //result.Currency = fromDb.Currency;
                //result.Id = fromDb.Id;

                //return result;
            }
        }
        public void Update(AccountDto item)
        {
            //map to Account
            Account account = Mapper.Map<AccountDto, Account>(item);

            using (SpendingsEntities context = new SpendingsEntities())
            {
                //find in database and attach to the account object
                //account.Id select top 1 from Accounts where Id == account.Id
                context.Accounts.Attach(account);
                //mark entity as modified
                context.Entry(account).State = EntityState.Modified;

                //mark certain properties as modified
                //context.Entry(account).Property(d => d.Currency).IsModified = true;
                //context.Entry(account).Property(d => d.Name).IsModified = true;

                //save modifications
                context.SaveChanges();

            }
        }
        public List<AccountDto> Filter(AccountDtoFilter filter)
        {
            using (SpendingsEntities context = new SpendingsEntities())
            {
                IQueryable<Account> query = context.Accounts;

                if (filter.IdCurrency != null)
                {
                    query = query.Where(d => d.IdCurrency == filter.IdCurrency);
                }

                List<Account> accounts = query.ToList();

                return Mapper.Map<List<Account>, List<AccountDto>>(accounts);
            }
        }

    }
}
