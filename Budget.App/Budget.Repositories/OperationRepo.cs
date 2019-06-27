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

namespace Budget.Repositories
{
    public class OperationRepo : IOperationRepo<OperationDto>
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
        public List<OperationDto> FilterByAccount(int idAccount)
        {
            using (SpendingsEntities context = new SpendingsEntities())
            {
                List<Operation> fromDb = context.Operations.Where(d => d.IdAccount == idAccount).ToList();

                return Mapper.Map<List<Operation>, List<OperationDto>>(fromDb);
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
            using (SpendingsEntities context = new SpendingsEntities())
            {
                Operation fromDb = context.Operations.FirstOrDefault(d => d.Id == item.Id);

                Mapper.Map<Operation, OperationDto>(fromDb);
                context.Operations.Attach(fromDb);
                context.Entry(fromDb).State = System.Data.Entity.EntityState.Modified;

                context.SaveChanges();
            }
        }
    }
}
