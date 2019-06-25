using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Budget.DataAccess.Model;
using Budget.Domain.Repositories;
using Budget.Infrastructure.Repositories.Specific;
using AutoMapper;

namespace Budget.Repositories
{
    public class OperationTypeRepo : IOperationTypeRepo<OperationTypeDto>
    {
        public int Add(OperationTypeDto item)
        {
            using (SpendingsEntities context = new SpendingsEntities())
            {
                //OperationType toAdd = new OperationType();
                //toAdd.Name = item.Name;
                //toAdd.Description = item.Description;
                //toAdd.IsCredit = item.IsCredit;

                OperationType toAdd = Mapper.Map<OperationType>(item);

                context.OperationTypes.Add(toAdd);
                context.SaveChanges();

                return toAdd.Id;
            }
        }

        public List<OperationTypeDto> All()
        {
            using (SpendingsEntities context = new SpendingsEntities())
            {
                List<OperationType> fromDb = context.OperationTypes.ToList();

                return Mapper.Map<List<OperationType>, List<OperationTypeDto>>(fromDb);

                //List<OperationTypeDto> dtos = new List<OperationTypeDto>();
                //foreach (OperationType item in fromDb)
                //{
                //    OperationTypeDto dto = new OperationTypeDto();
                //    dto.Name = item.Name;
                //    dto.Description = item.Description;
                //    dto.IsCredit = item.IsCredit;

                //    dtos.Add(dto);
                //}

                //return dtos;
            }
        }

        public bool Delete(int id)
        {
            using (SpendingsEntities context = new SpendingsEntities())
            {
                OperationType toDelete = context.OperationTypes.FirstOrDefault(d => d.Id == id);

                if (toDelete == null)
                {
                    return false;
                }

                context.OperationTypes.Remove(toDelete);
                context.SaveChanges();

                return true;
            }
        }

        public OperationTypeDto Get(int id)
        {
            using (SpendingsEntities context = new SpendingsEntities())
            {
                OperationType fromDb = context.OperationTypes.FirstOrDefault(d => d.Id == id);

                return Mapper.Map<OperationType, OperationTypeDto>(fromDb);

                //OperationTypeDto toGet = new OperationTypeDto();
                //toGet.Name = fromDb.Name;
                //toGet.Description = fromDb.Description;
                //toGet.IsCredit = fromDb.IsCredit;

                //return toGet;
            }
        }

        public void Update(OperationTypeDto item)
        {
            using (SpendingsEntities context = new SpendingsEntities())
            {
                OperationType OT = Mapper.Map<OperationTypeDto, OperationType>(item);
                context.OperationTypes.Attach(OT);
                context.Entry(OT).State = System.Data.Entity.EntityState.Modified;

                //OperationType fromDb = context.OperationTypes.FirstOrDefault(d => d.Id == item.Id);

                //if (fromDb == null)
                //{
                //    return;
                //}

                //fromDb.Name = item.Name;
                //fromDb.Description = item.Description;
                //fromDb.IsCredit = item.IsCredit;

                context.SaveChanges();
            }
        }
    }
}
