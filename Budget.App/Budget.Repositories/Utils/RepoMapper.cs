using AutoMapper;
using Budget.DataAccess.Model;
using Budget.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Budget.Repositories.Utils
{
    public class RepoMapper : Profile
    {
        public RepoMapper()
        {
            this.CreateMap<AccountDto, Account>().ReverseMap();
            this.CreateMap<OperationDto, Operation>().ReverseMap();
            this.CreateMap<OperationTypeDto, OperationType>().ReverseMap();
        }

    }
}
