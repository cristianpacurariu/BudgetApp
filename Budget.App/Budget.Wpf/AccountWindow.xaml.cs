using Budget.Domain.Filters;
using Budget.Domain.Repositories;
using Budget.Infrastructure.Repositories.Specific;
using Budget.Wpf.Utils;
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
        private readonly IAccountRepo<AccountDto, AccountDtoFilter> _accountRepo = RepoProvider.GetAccountRepo();
        private readonly ICurrencyRepo<CurrencyDto> _currencyRepo = RepoProvider.GetCurrencyRepo();
        public AccountWindow()
        {
            InitializeComponent();
            InitializeCurrencies();
        }
        public int? Id { get; set; }
        private void InitializeCurrencies()
        {
            List<CurrencyDto> currencyDtos = _currencyRepo.All();
            foreach (CurrencyDto currencyDto in currencyDtos)
            {
                ComboItem comboItem = new ComboItem()
                {
                    Name = currencyDto.Name,
                    Id = currencyDto.Id
                };

                cbCurrency.Items.Add(comboItem);
            }
        }

        private void BtnSaveAccount_Click(object sender, RoutedEventArgs e)
        {
            AccountDto account = new AccountDto();
            account.Name = tbName.Text;

            account.IdCurrency = ((ComboItem)cbCurrency.SelectedItem).Id;

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
