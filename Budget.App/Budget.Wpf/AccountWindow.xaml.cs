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

namespace Budget.Wpf
{
    /// <summary>
    /// Interaction logic for EditItemWindow.xaml
    /// </summary>
    public partial class AccountWindow : Window
    {
        private readonly IAccountRepo<AccountDto> _accountRepo = RepoProvider.GetAccountRepo();
        public AccountWindow()
        {
            InitializeComponent();
        }

        public int? Id { get; set; }

        private void BtnSaveAccount_Click(object sender, RoutedEventArgs e)
        {
            AccountDto account = new AccountDto();
            account.Name = tbName.Text;
            account.Currency = tbCurrency.Text;


            if (Id == null)
            {
                _accountRepo.Add(account);
            }
            else
            {
                account.Id = Id.Value;
                _accountRepo.Update(account);
            }

            this.Close();
        }
    }
}
