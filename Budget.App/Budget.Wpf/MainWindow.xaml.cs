using AutoMapper;
using Budget.Domain.Repositories;
using Budget.Infrastructure.Repositories.Specific;
using Budget.Repositories.Utils;
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
        private readonly IAccountRepo<AccountDto> _accountRepo = RepoProvider.GetAccountRepo();
        private readonly IOperationTypeRepo<OperationTypeDto> _operationTypeRepo = RepoProvider.GetOperationTypeRepo();
        private readonly IOperationRepo<OperationDto> _operationRepo = RepoProvider.GetOperationRepo();
        public MainWindow()
        {
            Mapper.Initialize(conf => conf.AddProfile<RepoMapper>());
            InitializeComponent();
        }

        private void BtnCreateAccount_Click(object sender, RoutedEventArgs e)
        {
            AccountWindow accountWindow = new AccountWindow();
            accountWindow.Show();
            accountWindow.Closing += AccountWindow_Closing;
        }
        private void BtnCreateCategory_Click(object sender, RoutedEventArgs e)
        {
            CategoryWindow categoryWindow = new CategoryWindow();
            categoryWindow.Show();
            categoryWindow.Closing += CategoryWindow_Closing;
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
        private void CategoryWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            CategoryWindow categoryWindow = new CategoryWindow();
            if (categoryWindow != null)
            {
                categoryWindow.Closing -= CategoryWindow_Closing;
            }

            InitializeCategories();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            InitializeAccounts();
            InitializeCategories();
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

                item.lblName.Content = account.Name + " " + account.Currency;
                item.Id = account.Id;
                item.btnEditItem.Click += BtnEditAccount_Click;
                item.btnDeleteItem.Click += BtnDeleteAccount_Click;

                spAccounts.Children.Add(item);
            }
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
            accountWindow.tbCurrency.Text = accountDto.Currency;

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
    }
}
