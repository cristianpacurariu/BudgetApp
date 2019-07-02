using AutoMapper;
using Budget.Domain.Filters;
using Budget.Domain.Repositories;
using Budget.Infrastructure.Repositories.Specific;
using Budget.Repositories.Utils;
using Budget.Wpf.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Controls.DataVisualization.Charting;

namespace Budget.Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IAccountRepo<AccountDto, AccountDtoFilter> _accountRepo = RepoProvider.GetAccountRepo();
        private readonly IOperationTypeRepo<OperationTypeDto> _operationTypeRepo = RepoProvider.GetOperationTypeRepo();
        private readonly IOperationRepo<OperationDto, OperationDtoFilter> _operationRepo = RepoProvider.GetOperationRepo();
        private readonly ICurrencyRepo<CurrencyDto> _currencyRepo = RepoProvider.GetCurrencyRepo();
        private const string DateFormat = "MMMM yyyy";

        public MainWindow()
        {
            Mapper.Initialize(conf => conf.AddProfile<RepoMapper>());
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            InitializeAccounts();
            InitializeCategories();
            InitializeCurrencies();
            InitializeAnalitics();
            this.Closing += MainWindow_Closing;
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.Closing -= MainWindow_Closing;
            btnNext.Click -= BtnNext_Click;
            btnPrevious.Click -= BtnPrevious_Click;

            List<CurrencyRadio> radios = spCurrencyFilter.Children.OfType<CurrencyRadio>().ToList();
            foreach (CurrencyRadio radio in radios)
            {
                radio.Click -= BtnCurrency_Click;
            }
        }

        #region Chart

        private void InitializeAnalitics()
        {
            CurrencyRadio first = spCurrencyFilter.Children.OfType<CurrencyRadio>().FirstOrDefault();

            if (first != null)
            {
                first.IsChecked = true;
            }

            InitializeChart();

            btnNext.Click += BtnNext_Click;
            btnPrevious.Click += BtnPrevious_Click;
        }
        private void InitializeChart()
        {
            //filter by currency
            OperationDtoFilter filter = GetChartFilter();
            List<OperationDto> filteredList = _operationRepo.Filter(filter);

            //filter by category
            List<OperationTypeDto> categories = _operationTypeRepo.All();

            List<ChartItem> values = new List<ChartItem>();
            foreach (OperationTypeDto category in categories)
            {
                ChartItem chartItem = new ChartItem()
                {
                    Name = category.Name,
                    Value = filteredList
                                .Where(d => d.IdOperationType == category.Id)
                                .Sum(d => d.Ammount)
                };
                values.Add(chartItem);
            }

            pieSeries.ItemsSource = values;
        }

        private CurrencyRadio GetClickedCurrency()
        {
            return spCurrencyFilter.Children
                .OfType<CurrencyRadio>()
                .Where(d => d.IsChecked == true)
                .FirstOrDefault();
        }
        private OperationDtoFilter GetChartFilter()
        {
            OperationDtoFilter filter = new OperationDtoFilter();
            filter.IdCurrency = GetClickedCurrency().Id;
            filter.DateFrom = new DateTime(tbMonth.Date.Year, tbMonth.Date.Month, 1);
            filter.DateTo = filter.DateFrom.Value.AddMonths(1).AddSeconds(-1);

            return filter;
        }

        private void BtnPrevious_Click(object sender, RoutedEventArgs e)
        {
            tbMonth.DecrementMonth();
            InitializeChart();
        }
        private void BtnNext_Click(object sender, RoutedEventArgs e)
        {
            tbMonth.IncredementMonth();
            InitializeChart();
        }
        
        #endregion

        #region Currencies

        private void InitializeCurrencies()
        {
            spCurrencyFilter.Children.Clear();
            List<CurrencyDto> currencyDtos = _currencyRepo.All();
            foreach (CurrencyDto currency in currencyDtos)
            {
                CurrencyRadio currencyRadio = new CurrencyRadio();
                currencyRadio.Id = currency.Id;
                currencyRadio.Content = $"{currency.Name}";
                spCurrencyFilter.Children.Add(currencyRadio);

                currencyRadio.Click += BtnCurrency_Click;
            }
        }
        private void BtnCurrency_Click(object sender, RoutedEventArgs e)
        {
            InitializeChart();
        }

        #endregion

        #region Accounts
        private void BtnCreateAccount_Click(object sender, RoutedEventArgs e)
        {
            AccountWindow accountWindow = new AccountWindow();
            accountWindow.Show();
            accountWindow.Closing += AccountWindow_Closing;
        }
        private void AccountWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // to avoid RAM memory leaks
            AccountWindow accountWindow = sender as AccountWindow;
            if (accountWindow != null)
            {
                accountWindow.Closing -= AccountWindow_Closing;
            }

            InitializeAccounts();
        }
        private void InitializeAccounts()
        {
            //unsub from events
            foreach (var item in spAccounts.Children)
            {
                EditableItemControl control = (EditableItemControl)item;
                control.btnEditItem.Click -= BtnEditAccount_Click;
                control.btnDeleteItem.Click -= BtnDeleteAccount_Click;
            }

            spAccounts.Children.Clear();

            //get accounts from db
            List<AccountDto> accounts = _accountRepo.All();

            foreach (AccountDto account in accounts)
            {
                EditableItemControl item = new EditableItemControl();

                item.lblName.Content = account.Name + " " + account.Currency.Name;
                item.Id = account.Id;
                item.btnEditItem.Click += BtnEditAccount_Click;
                item.btnDeleteItem.Click += BtnDeleteAccount_Click;

                spAccounts.Children.Add(item);
            }
        }

        private AccountDto GetClickedAccount(object sender)
        {
            //get button parent until we reach the user control (Editable Item Control)
            DependencyObject ucParent = ((Button)sender).Parent;
            while (!(ucParent is UserControl))
            {
                ucParent = LogicalTreeHelper.GetParent(ucParent);
            }

            // cast to specific type from UserControl
            EditableItemControl userControl = (EditableItemControl)ucParent;

            //Get from Db the account with the id of the UserControl
            AccountDto accountDto = _accountRepo.Get(userControl.Id);
            return accountDto;
        }
        private void BtnEditAccount_Click(object sender, RoutedEventArgs e)
        {
            AccountDto accountDto = GetClickedAccount(sender);

            //Initialize EditWindow
            AccountWindow accountWindow = new AccountWindow();
            accountWindow.Id = accountDto.Id;
            accountWindow.tbName.Text = accountDto.Name;

            foreach (var item in accountWindow.cbCurrency.Items)
            {
                if (((ComboItem)item).Id == accountDto.IdCurrency)
                {
                    accountWindow.cbCurrency.SelectedItem = item;
                    break;
                }
            }

            accountWindow.Show();
            accountWindow.Closing += AccountWindow_Closing;
        }
        private void BtnDeleteAccount_Click(object sender, RoutedEventArgs e)
        {
            AccountDto accountDto = GetClickedAccount(sender);

            try
            {
                _accountRepo.Delete(accountDto.Id);
            }
            catch
            {
                MessageBox.Show("Unable to delete account. It has been used for other transactions.");
            }

            InitializeAccounts();
        }

        #endregion

        #region Categories

        private void BtnCreateCategory_Click(object sender, RoutedEventArgs e)
        {
            CategoryWindow categoryWindow = new CategoryWindow();
            categoryWindow.Show();
            categoryWindow.Closing += CategoryWindow_Closing;
        }
        private void CategoryWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            CategoryWindow categoryWindow = new CategoryWindow();
            if (categoryWindow != null)
            {
                categoryWindow.Closing -= CategoryWindow_Closing;
            }

            InitializeCategories();
        }
        private void InitializeCategories()
        {
            //unsub from events
            foreach (var item in spCategories.Children)
            {
                EditableItemControl control = (EditableItemControl)item;
                control.btnEditItem.Click -= BtnEditCategory_Click;
                control.btnDeleteItem.Click -= BtnDeleteCategory_Click;
            }

            spCategories.Children.Clear();

            // get categories from db
            List<OperationTypeDto> categories = _operationTypeRepo.All();

            foreach (OperationTypeDto category in categories)
            {
                EditableItemControl item = new EditableItemControl();

                item.lblName.Content = category.Name;
                item.Id = category.Id;
                item.btnEditItem.Click += BtnEditCategory_Click;
                item.btnDeleteItem.Click += BtnDeleteCategory_Click;

                spCategories.Children.Add(item);
            }
        }
        private OperationTypeDto GetClickedCategory(object sender)
        {
            //get button parent until we reach the user control (Editable Item Control)
            DependencyObject ucParent = ((Button)sender).Parent;
            while (!(ucParent is UserControl))
            {
                ucParent = LogicalTreeHelper.GetParent(ucParent);
            }

            // cast to specific type from UserControl
            EditableItemControl userControl = (EditableItemControl)ucParent;

            //Get from Db the account with the id of the UserControl
            OperationTypeDto category = _operationTypeRepo.Get(userControl.Id);
            return category;
        }
        private void BtnEditCategory_Click(object sender, RoutedEventArgs e)
        {
            OperationTypeDto category = GetClickedCategory(sender);

            CategoryWindow categoryWindow = new CategoryWindow();
            categoryWindow.Id = category.Id;
            categoryWindow.tbCategoryName.Text = category.Name;

            if (category.IsCredit)
            {
                categoryWindow.rbIncome.IsChecked = true;
            }
            else
            {
                categoryWindow.rbExpense.IsChecked = true;
            }

            categoryWindow.Show();
            categoryWindow.Closing += CategoryWindow_Closing;
        }
        private void BtnDeleteCategory_Click(object sender, RoutedEventArgs e)
        {
            OperationTypeDto category = GetClickedCategory(sender);

            try
            {
                _operationTypeRepo.Delete(category.Id);
            }
            catch
            {
                MessageBox.Show("Unable to delete category. It has been used for other transactions");
            }

            InitializeCategories();
        }

        #endregion

        #region Spendings

        private void BtnViewSpendings_Click(object sender, RoutedEventArgs e)
        {
            OperationsListWindow transactions = new OperationsListWindow();
            transactions.Show();
            transactions.Closing += Transactions_Closing;
        }

        private void Transactions_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //unsub from events
            ((OperationsListWindow)sender).Closing -= Transactions_Closing;

            this.InitializeChart();
        }

        #endregion



    }
}
