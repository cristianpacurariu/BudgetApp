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
    public partial class OperationsListWindow : Window
    {
        #region Fields

        private readonly ICurrencyRepo<CurrencyDto> _currencyRepo = RepoProvider.GetCurrencyRepo();
        private readonly IAccountRepo<AccountDto, AccountDtoFilter> _accountRepo = RepoProvider.GetAccountRepo();
        private readonly IOperationTypeRepo<OperationTypeDto> _operationTypeRepo = RepoProvider.GetOperationTypeRepo();
        private readonly IOperationRepo<OperationDto, OperationDtoFilter> _operationRepo = RepoProvider.GetOperationRepo();

        #endregion

        #region Constructors

        public OperationsListWindow()
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
        private void InitializeCombos()
        {
            List<CurrencyDto> currencyDtos = _currencyRepo.All();
            InitializeCurrencies(currencyDtos);

            List<AccountDto> accountDtos = _accountRepo.All();
            InitializeAccounts(accountDtos);

            List<OperationTypeDto> operationTypeDtos = _operationTypeRepo.All();
            InitializeCategories(operationTypeDtos);
        }
        private void InitializeTransactions()
        {
            //unsub from events
            foreach (var item in spTransactions.Children)
            {
                EditableTransactionControl control = (EditableTransactionControl)item;
                control.btnEditOperation.Click -= BtnEditOperation_Click;
                control.btnDeleteOperation.Click -= BtnDeleteOperation_Click;
            }

            spTransactions.Children.Clear();

            // Get present filter
            OperationDtoFilter filter = GetFilter();

            List<OperationDto> operationDtos = _operationRepo.Filter(filter);
            operationDtos = operationDtos.OrderByDescending(d => d.Date).ToList();

            foreach (OperationDto operationDto in operationDtos)
            {
                EditableTransactionControl transactionControl = new EditableTransactionControl();

                transactionControl.IdOperation = operationDto.Id;

                transactionControl.dpTransaction.Text = operationDto.Date.ToString("dd/MMM/yyyy");
                transactionControl.tbCurrency.Text = operationDto.Account.Currency.Name;
                transactionControl.tbAccount.Text = operationDto.Account.Name;
                transactionControl.tbAmmount.Text = operationDto.Ammount.ToString();
                transactionControl.tbCategory.Text = operationDto.OperationType.Name;
                transactionControl.tbDescription.Text = operationDto.Description;

                transactionControl.btnEditOperation.Click += BtnEditOperation_Click;
                transactionControl.btnDeleteOperation.Click += BtnDeleteOperation_Click;

                spTransactions.Children.Add(transactionControl);
            }
        }
        private void TransactionListWindow_Loaded(object sender, RoutedEventArgs e)
        {
            InitializeCombos();
            cbCurrency.SelectedIndex = 0;
            cbAccount.SelectedIndex = 0;
            cbCategory.SelectedIndex = 0;

            tbFrom.Text = "0";

            dpFrom.SelectedDate = new DateTime(DateTime.Now.Year, 1, 1);
            dpTo.SelectedDate = DateTime.Today;
        }

        private void BtnNewSpending_Click(object sender, RoutedEventArgs e)
        {
            OperationWindow newSpendingWindow = new OperationWindow();
            newSpendingWindow.Show();

            newSpendingWindow.Closing += NewSpendingWindow_Closing;
        }
        private void NewSpendingWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            InitializeTransactions();
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
            filter.DateFrom = dpFrom.SelectedDate;
            filter.DateTo = dpTo.SelectedDate;

            return filter;
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

        private OperationDto GetClickedOperation(object sender)
        {
            DependencyObject parent = ((Button)sender).Parent;

            while (!(parent is EditableTransactionControl))
            {
                parent = LogicalTreeHelper.GetParent(parent);
            }

            int opId = ((EditableTransactionControl)parent).IdOperation;

            OperationDto operationDto = _operationRepo.Get(opId);
            return operationDto;

            ////get button parent until we reach the user control (Editable Transaction Control)
            //DependencyObject ucParent = ((Button)sender).Parent;
            //while (!(ucParent is UserControl))
            //{
            //    ucParent = LogicalTreeHelper.GetParent(ucParent);
            //}

            //// cast to specific type from UserControl
            //EditableTransactionControl userControl = (EditableTransactionControl)ucParent;

            ////Get from Db the account with the id of the UserControl
            //OperationDto operationDto = _operationRepo.Get(userControl.IdOperation);
            //return operationDto;
        }

        private void BtnEditOperation_Click(object sender, RoutedEventArgs e)
        {
            OperationDto clickedOperation = GetClickedOperation(sender);

            OperationWindow operationWindow = new OperationWindow();
            operationWindow.Id = clickedOperation.Id;

            // Populate text boxes and date
            operationWindow.datePicker.SelectedDate = clickedOperation.Date;
            operationWindow.tbAmmount.Text = clickedOperation.Ammount.ToString();
            operationWindow.tbDescription.Text = clickedOperation.Description;

            // Populate accounts combo box
            foreach (ComboItem item in operationWindow.cbAccount.Items)
            {
                if (item.Id == clickedOperation.IdAccount)
                {
                    operationWindow.cbAccount.SelectedItem = item;
                }
            }

            // Populate categories combo box
            foreach (ComboItem item in operationWindow.cbCategory.Items)
            {
                if (item.Id == clickedOperation.IdOperationType)
                {
                    operationWindow.cbCategory.SelectedItem = item;
                }
            }

            operationWindow.Show();

            //OperationDto editedOperation = new OperationDto()
            //{
            //    Id = clickedOperation.Id,
            //    //Date = ,
            //    Ammount = (decimal.Parse(operationWindow.tbAmmount.Text)),
            //    IdOperationType = ((ComboItem)operationWindow.cbCategory.SelectedItem).Id,
            //    Description = operationWindow.tbDescription.Text
            //};
            //_operationRepo.Update(editedOperation);

            operationWindow.Closing += OperationWindow_Closing;

        }
        private void OperationWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // to avoid RAM memory leaks
            OperationWindow operationWindow = sender as OperationWindow;
            if (operationWindow != null)
            {
                operationWindow.Closing -= OperationWindow_Closing;
            }
            InitializeTransactions();
        }

        private void BtnDeleteOperation_Click(object sender, RoutedEventArgs e)
        {
            OperationDto clickedOperation = GetClickedOperation(sender);
            _operationRepo.Delete(clickedOperation.Id);
            InitializeTransactions();
        }

        #endregion

    }
}
