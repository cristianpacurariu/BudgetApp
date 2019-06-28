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
    public partial class TransactionListWindow : Window
    {
        #region Fields

        private readonly ICurrencyRepo<CurrencyDto> _currencyRepo = RepoProvider.GetCurrencyRepo();
        private readonly IAccountRepo<AccountDto, AccountDtoFilter> _accountRepo = RepoProvider.GetAccountRepo();
        private readonly IOperationTypeRepo<OperationTypeDto> _operationTypeRepo = RepoProvider.GetOperationTypeRepo();
        private readonly IOperationRepo<OperationDto, OperationDtoFilter> _operationRepo = RepoProvider.GetOperationRepo();

        #endregion

        #region Constructors

        public TransactionListWindow()
        {
            InitializeComponent();

            this.Loaded += TransactionListWindow_Loaded;
        }

        #endregion

        #region Properties

        public DateTime SelectedDate { get; set; }
        public int? IdSelectedCurrency { get; set; }
        public int? IdSelectedAccount { get; set; }
        public int? IdSelectedCategory { get; set; }
        public decimal? SelectedAmmount { get; set; }

        #endregion

        #region Methods

        private void InitializeCombos()
        {
            List<CurrencyDto> currencyDtos = _currencyRepo.All();
            InitializeCurrencies(currencyDtos);

            List<AccountDto> accountDtos = _accountRepo.All();
            InitializeAccounts(accountDtos);

            List<OperationTypeDto> operationTypeDtos = _operationTypeRepo.All();
            InitializeCategories(operationTypeDtos);
        }
        private void InitializeCurrencies(List<CurrencyDto> currencyDtos)
        {
            cbCurrency.Items.Clear();
            cbCurrency.Items.Add(new ComboItem() { Name = string.Empty, Id = 0 });

            foreach (CurrencyDto currency in currencyDtos)
            {
                ComboItem comboItem = new ComboItem()
                {
                    Name = currency.Name,
                    Id = currency.Id
                };

                cbCurrency.Items.Add(comboItem);
            }
        }
        private void InitializeCategories(List<OperationTypeDto> operationTypeDtos)
        {
            cbCategory.Items.Clear();
            cbCategory.Items.Add(new ComboItem() { Name = string.Empty, Id = 0 });

            foreach (OperationTypeDto category in operationTypeDtos)
            {
                ComboItem comboItem = new ComboItem()
                {
                    Name = category.Name,
                    Id = category.Id
                };

                cbCategory.Items.Add(comboItem);
            }
        }
        private void InitializeAccounts(List<AccountDto> accountDtos)
        {
            cbAccount.Items.Clear();
            cbAccount.Items.Add(new ComboItem() { Name = string.Empty, Id = 0 });

            foreach (AccountDto account in accountDtos)
            {
                ComboItem comboItem = new ComboItem()
                {
                    Name = $"{account.Name} {account.Currency.Name}",
                    Id = account.Id
                };

                cbAccount.Items.Add(comboItem);
            }
        }
        private void InitializeTransactions()
        {
            // Get present filter
            OperationDtoFilter filter = GetFilter();

            List<OperationDto> operationDtos = _operationRepo.Filter(filter);
            operationDtos = operationDtos.OrderByDescending(d => d.Date).ToList();

            spTransactions.Children.Clear();

            foreach (OperationDto operationDto in operationDtos)
            {
                EditableTransactionControl transactionControl = new EditableTransactionControl();

                transactionControl.dpTransaction.Text = operationDto.Date.ToString("dd/MMM/yyyy");
                transactionControl.tbCurrency.Text = operationDto.Account.Currency.Name;
                transactionControl.tbAccount.Text = operationDto.Account.Name;
                transactionControl.tbAmmount.Text = operationDto.Ammount.ToString();
                transactionControl.tbCategory.Text = operationDto.OperationType.Name;
                transactionControl.tbDescription.Text = operationDto.Description;

                spTransactions.Children.Add(transactionControl);
            }
        }
        private OperationDtoFilter GetFilter()
        {
            OperationDtoFilter filter = new OperationDtoFilter();

            if ((ComboItem)cbAccount.SelectedItem != null)
            {
                int? idSelectedAccount = ((ComboItem)cbAccount.SelectedItem).Id;
                if (idSelectedAccount == 0)
                {
                    filter.IdAccount = null;
                }
                else
                {
                    filter.IdAccount = idSelectedAccount;
                }
            }
            if ((ComboItem)cbCategory.SelectedItem != null)
            {
                int? idSelectedCategory = ((ComboItem)cbCategory.SelectedItem).Id;
                if (idSelectedCategory == 0)
                {
                    filter.IdCategory = null;
                }
                else
                {
                    filter.IdCategory = idSelectedCategory;
                }
            }

            if ((ComboItem)cbCurrency.SelectedItem != null)
            {
                int? idSelectedCurrency = ((ComboItem)cbCurrency.SelectedItem).Id;
                if (idSelectedCurrency == 0)
                {
                    filter.IdCurrency = null;
                }
                else
                {
                    filter.IdCurrency = idSelectedCurrency;
                }
            }

            if (decimal.TryParse(tbFrom.Text, out decimal from))
            {
                filter.AmmountFrom = from;
            }
            if (decimal.TryParse(tbTo.Text, out decimal to))
            {
                filter.AmmountTo = to;
            }

            return filter;
        }
        private void FilterAccountsComboBox()
        {
            AccountDtoFilter filter = new AccountDtoFilter()
            {
                IdCurrency = ((ComboItem)cbCurrency.SelectedItem).Id
            };
            if (filter.IdCurrency == 0)
            {
                filter.IdCurrency = null;
            }
            List<AccountDto> accountDtos = _accountRepo.Filter(filter);
            InitializeAccounts(accountDtos);
        }

        #region Events

        private void TransactionListWindow_Loaded(object sender, RoutedEventArgs e)
        {
            InitializeCombos();
            InitializeTransactions();
        }
        private void CbCurrency_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FilterAccountsComboBox();
            InitializeTransactions();
        }
        private void CbAccount_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            InitializeTransactions();
        }
        private void CbCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            InitializeTransactions();
        }
        private void TbFrom_TextChanged(object sender, TextChangedEventArgs e)
        {
            InitializeTransactions();
        }
        private void TbTo_TextChanged(object sender, TextChangedEventArgs e)
        {
            InitializeTransactions();
        }

        #endregion

        #endregion
    }
}
