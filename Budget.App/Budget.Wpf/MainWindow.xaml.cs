using AutoMapper;
using Budget.Domain.Filters;
using Budget.Domain.Repositories;
using Budget.Infrastructure.Repositories.Specific;
using Budget.Repositories.Utils;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

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
        }

        private void InitializeAnalitics()
        {
            List<OperationDto> operations = _operationRepo.All();
            List<AccountDto> accounts = new List<AccountDto>();
            List<OperationTypeDto> categories = new List<OperationTypeDto>();

            foreach (OperationDto operationDto in operations)
            {
                if (!accounts.Contains(operationDto.Account))
                {
                    accounts.Add(operationDto.Account);
                }
                if (!categories.Contains(operationDto.OperationType))
                {
                    categories.Add(operationDto.OperationType);
                }
            }
            for (int i = 1; i <= 12 ; i++)
            {
                MonthlyControl monthlyControl = new MonthlyControl();

                monthlyControl.tbMonth.Text = $"Month {i}";

                foreach (AccountDto account in accounts)
                {
                    OverviewItemControl overviewItem = new OverviewItemControl();
                    overviewItem.tbLabel.Text = $"{account.Name} {account.Currency.Name}";

                    overviewItem.tbSum.Text = operations.Where(d => d.Account == account)
                                                        .Sum(d => d.Ammount)
                                                        .ToString();

                    monthlyControl.spAccView.Children.Add(overviewItem);
                }

                foreach (OperationTypeDto category in categories)
                {
                    OverviewItemControl overviewItem = new OverviewItemControl();
                    overviewItem.tbLabel.Text = $"{category.Name}";

                    overviewItem.tbSum.Text = operations.Where(d => d.OperationType == category)
                                                        .Sum(d => d.Ammount)
                                                        .ToString();

                    monthlyControl.spCatView.Children.Add(overviewItem);
                }

                spDashboard.Children.Add(monthlyControl);
            }
            

        }

        private void InitializeCurrencies()
        {
            spCurrencyFilter.Children.Clear();
            List<CurrencyDto> currencyDtos = _currencyRepo.All();
            foreach (CurrencyDto currency in currencyDtos)
            {
                EditableCurrencyControl control = new EditableCurrencyControl();
                control.Id = currency.Id;
                control.btnCurrency.Content = $"{currency.Name}";
                spCurrencyFilter.Children.Add(control);

                control.btnCurrency.Click += BtnCurrency_Click;
            }
        }
        private void BtnCurrency_Click(object sender, RoutedEventArgs e)
        {
            //CurrencyDto clickedCurrency = GetClickedCurrency(sender);

            string message = ((Button)sender).Content.ToString();
            MessageBox.Show($"Currency {message} pressed.");
        }
        private CurrencyDto GetClickedCurrency(object sender)
        {
            DependencyObject parent = ((Button)sender).Parent;

            while (!(parent is EditableCurrencyControl))
            {
                parent = LogicalTreeHelper.GetParent(parent);
            }

            int currencyId = ((EditableCurrencyControl)parent).Id;

            CurrencyDto currencyDto = _currencyRepo.Get(currencyId);
            return currencyDto;
        }


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
            TransactionListWindow transactions = new TransactionListWindow();
            transactions.Show();
        }

        #endregion



    }
}
