using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Budget.Infrastructure.Repositories.Specific;
using Budget.Domain.Repositories;
using Budget.DataAccess.Model;
using AutoMapper;

namespace Budget.Repositories
{
    public class OperationRepo : IOperationRepo<OperationDto>
    {
        public int Add(OperationDto item)
        {
            using (SpendingsEntities context = new SpendingsEntities())
            {
                Operation toAdd = Mapper.Map<OperationDto, Operation>(item);

                //Operation toAdd = new Operation();
                //toAdd.Ammount = item.Ammount;
                //toAdd.Date = item.Date;
                //toAdd.Description = item.Description;
                //toAdd.IdAccount = item.IdAccount;
                //toAdd.IdOperationType = item.IdOperationType;

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

                //List<OperationDto> dtos = new List<OperationDto>();

                //foreach (Operation operation in operations)
                //{
                //    OperationDto dto = new OperationDto();
                //    dto.IdAccount = operation.IdAccount;
                //    dto.IdOperationType = operation.IdOperationType;
                //    dto.Ammount = operation.Ammount;
                //    dto.Date = operation.Date;
                //    dto.Description = operation.Description;

                //    dtos.Add(dto);
                //}

                //return dtos;
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

                //OperationDto toGet = new OperationDto();
                //toGet.IdAccount = fromDb.IdAccount;
                //toGet.IdOperationType = fromDb.IdOperationType;
                //toGet.Ammount = fromDb.Ammount;
                //toGet.Date = fromDb.Date;
                //toGet.Description = fromDb.Description;

                //return toGet;
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


                //if (fromDb == null)
                //{
                //    return;
                //}

                //fromDb.IdAccount = item.IdAccount;
                //fromDb.IdOperationType = item.IdOperationType;
                //fromDb.Ammount = item.Ammount;
                //fromDb.Date = item.Date;
                //fromDb.Description = item.Description;

                context.SaveChanges();
            }
        }
    }
}
