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
        private void InitializeCategories(List<OperationTypeDto> operationTypeDtos)
        {
            cbCategory.Items.Clear();
            cbCategory.Items.Add(new ComboItem() { Name = "All Categories", Id = 0 });

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
            cbAccount.Items.Add(new ComboItem() { Name = "All Accounts", Id = 0 });

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

            // Currency filter
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

            // Account filter
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

            // Category filter
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

            // Ammount filter
            if (decimal.TryParse(tbFrom.Text, out decimal from))
            {
                filter.AmmountFrom = from;
            }
            if (decimal.TryParse(tbTo.Text, out decimal to))
            {
                filter.AmmountTo = to;
            }

            // Date filter

            filter.DateFrom = dpFrom.DisplayDate.Date;
            filter.DateTo = dpTo.DisplayDate.Date;


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
            cbCurrency.Items.Add(new ComboItem() { Name = "All Currencies", Id = 0 });

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

        #region Events

        private void TransactionListWindow_Loaded(object sender, RoutedEventArgs e)
        {
            InitializeCombos();
            InitializeTransactions();

            cbCurrency.SelectedIndex = 0;
            cbAccount.SelectedIndex = 0;
            cbCategory.SelectedIndex = 0;

            tbFrom.Text = "0";

            dpFrom.SelectedDate = new DateTime(DateTime.Now.Year, 1, 1);
            dpTo.SelectedDate = DateTime.Today;

        }
        private void CbCurrency_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FilterAccountsComboBox();
            InitializeTransactions();
            cbAccount.SelectedIndex = 0;
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
            if (int.TryParse(tbFrom.Text, out int result))
            {
                tbFrom.Text = result.ToString();
            }
            else
            {
                MessageBox.Show("The ammount cannot contain text. Please insert a valid integer number from 0 - 10000");
                if (tbFrom.Text.Length > 0)
                {
                    //erase last digit
                    tbFrom.Text = tbFrom.Text.Substring(0, tbFrom.Text.Length - 1);
                    //set cursor at the end of text
                    tbFrom.SelectionStart = tbFrom.Text.Length;
                }
            }
            InitializeTransactions();
        }
        private void TbTo_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (int.TryParse(tbTo.Text, out int result))
            {
                tbTo.Text = result.ToString();
            }
            else
            {
                MessageBox.Show("The ammount cannot contain text. Please insert a valid integer number from 0 - 10000");
                if (tbTo.Text.Length > 0)
                {
                    //erase last digit
                    tbTo.Text = tbTo.Text.Substring(0, tbTo.Text.Length - 1);
                    //set cursor at the end of text
                    tbTo.SelectionStart = tbTo.Text.Length;
                }
            }
            InitializeTransactions();
        }
        private void DpFrom_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            InitializeTransactions();
        }
        private void DpTo_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            InitializeTransactions();
        }

        #endregion

        #endregion

    }
}
