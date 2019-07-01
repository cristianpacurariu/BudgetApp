using Budget.Domain.Repositories;
using Budget.Infrastructure.Repositories.Specific;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Budget.Wpf.Utils;
using Budget.Domain.Filters;

namespace Budget.Wpf
{
    public partial class OperationWindow : Window
    {
        private readonly IAccountRepo<AccountDto, AccountDtoFilter> _accountRepo = RepoProvider.GetAccountRepo();
        private readonly IOperationTypeRepo<OperationTypeDto> _operationTypeRepo = RepoProvider.GetOperationTypeRepo();
        private readonly IOperationRepo<OperationDto, OperationDtoFilter> _operationRepo = RepoProvider.GetOperationRepo();
        public OperationWindow()
        {
            InitializeComponent();
            InitializeData();
        }

        public int Id { get; set; }

        private void InitializeData()
        {
            // Initialize Accounts
            List<AccountDto> accountDtos = _accountRepo.All();

            foreach (AccountDto accountDto in accountDtos)
            {
                ComboItem comboItem = new ComboItem()
                {
                    Name = $"{accountDto.Name} {accountDto.Currency.Name}",
                    Id = accountDto.Id
                };
                cbAccount.Items.Add(comboItem);
            }

            //Initialize Categories
            List<OperationTypeDto> operationTypeDtos = _operationTypeRepo.All();

            foreach (OperationTypeDto operationTypeDto in operationTypeDtos)
            {
                ComboItem comboItem = new ComboItem()
                {
                    Name = operationTypeDto.Name,
                    Id = operationTypeDto.Id
                };
                cbCategory.Items.Add(comboItem);
            }
        }
        private void BtnSaveOperation_Click(object sender, RoutedEventArgs e)
        {
            if (this.Id != 0)
            {
                OperationDto editOperation = new OperationDto()
                {
                    Id = this.Id,
                    IdAccount = ((ComboItem)cbAccount.SelectedItem).Id,
                    IdOperationType = ((ComboItem)cbCategory.SelectedItem).Id,
                    Date = (DateTime)datePicker.SelectedDate,
                    Description = tbDescription.Text,
                };

                if (decimal.TryParse(tbAmmount.Text, out decimal result))
                {
                    editOperation.Ammount = result;
                }
                else
                {
                    MessageBox.Show("Please insert a valid number for the Ammount");
                    return;
                }

                _operationRepo.Update(editOperation);
                MessageBox.Show("Operation updated succesfully");
            }
            else
            {
                OperationDto newOperationDto = new OperationDto()
                {
                    IdAccount = ((ComboItem)cbAccount.SelectedItem).Id,
                    IdOperationType = ((ComboItem)cbCategory.SelectedItem).Id,
                    Date = (DateTime)datePicker.SelectedDate,
                    Description = tbDescription.Text,
                };

                if (decimal.TryParse(tbAmmount.Text, out decimal result))
                {
                    newOperationDto.Ammount = result;
                }
                else
                {
                    MessageBox.Show("Please insert a valid number for the Ammount");
                    return;
                }

                _operationRepo.Add(newOperationDto);
                MessageBox.Show("Operation added succesfully");
            }
            this.Close();
        }
    }
}
