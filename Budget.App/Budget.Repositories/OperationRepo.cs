using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Budget.Infrastructure.Repositories.Specific;
using Budget.Domain.Repositories;
using Budget.DataAccess.Model;
using AutoMapper;
using System.Data.Entity;
using Budget.Domain.Filters;

namespace Budget.Repositories
{
    public class OperationRepo : IOperationRepo<OperationDto, OperationDtoFilter>
    {
        public int Add(OperationDto item)
        {
            using (SpendingsEntities context = new SpendingsEntities())
            {
                Operation toAdd = Mapper.Map<OperationDto, Operation>(item);

                context.Operations.Add(toAdd);
                context.SaveChanges();

                return toAdd.Id;
            }
        }
        public List<OperationDto> All()
        {
            using (SpendingsEntities context = new SpendingsEntities())
            {
                List<Operation> operations = context.Operations.ToList();
                return Mapper.Map<List<Operation>, List<OperationDto>>(operations);
            }
        }
        public bool Delete(int id)
        {
            using (SpendingsEntities context = new SpendingsEntities())
            {
                Operation toDelete = context.Operations.FirstOrDefault(d => d.Id == id);

                if (toDelete == null)
                {
                    return false;
                }

                context.Operations.Remove(toDelete);
                context.SaveChanges();

                return true;
            }
        }
        public OperationDto Get(int id)
        {
            using (SpendingsEntities context = new SpendingsEntities())
            {
                Operation fromDb = context.Operations.FirstOrDefault(d => d.Id == id);

                if (fromDb == null)
                {
                    return null;
                }

                return Mapper.Map<Operation, OperationDto>(fromDb);
            }
        }
        public void Update(OperationDto item)
        {
            Operation operation = Mapper.Map<OperationDto, Operation>(item);

            using (SpendingsEntities context = new SpendingsEntities())
            {
                context.Operations.Attach(operation);
                context.Entry(operation).State = EntityState.Modified;

                context.SaveChanges();
            }
        }
        public List<OperationDto> Filter(OperationDtoFilter filter)
        {
            using (SpendingsEntities context = new SpendingsEntities())
            {
                IQueryable<Operation> query = context.Operations;

                if (filter.IdAccount != null)
                {
                    query = query.Where(d => d.IdAccount == filter.IdAccount.Value);
                }

                if (filter.IdCategory != null)
                {
                    query = query.Where(d => d.IdOperationType == filter.IdCategory.Value);
                }

                if (filter.IdCurrency != null)
                {
                    query = query.Where(d => d.Account.IdCurrency == filter.IdCurrency);
                }

                if (filter.AmmountFrom != null)
                {
                    query = query.Where(d => d.Ammount >= filter.AmmountFrom);
                }

                if (filter.AmmountTo != null)
                {
                    query = query.Where(d => d.Ammount <= filter.AmmountTo);
                }

                if (filter.DateFrom != null)
                {
                    query = query.Where(d => d.Date >= filter.DateFrom);
                }

                if (filter.DateTo != null)
                {
                    query = query.Where(d => d.Date <= filter.DateTo);
                }

                List<Operation> fromDb = query.ToList();

                return Mapper.Map<List<Operation>, List<OperationDto>>(fromDb);
            }

        }
    }
}
