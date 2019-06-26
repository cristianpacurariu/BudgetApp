using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Budget.Domain.Repositories;
using Budget.Infrastructure.Repositories.Specific;
using Budget.Repositories;
using Budget.App.Utils;

namespace Budget.Forms
{
    public partial class Form1 : Form
    {
        private readonly IAccountRepo<AccountDto> _accountRepo = RepoProvider.GetAccountRepo();
        private readonly IOperationTypeRepo<OperationTypeDto> _operationTypeRepo = RepoProvider.GetOperationTypeRepo();
        private readonly IOperationRepo<OperationDto> _operationRepo = RepoProvider.GetOperationRepo();

        public Form1()
        {
            InitializeComponent();
            this.Load += Form1_Load;

            List<OperationDto> filteredList = _operationRepo.FilterByAccount(1);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            AccountDto account =  _accountRepo.Get(1);
            
        }

    }
}
