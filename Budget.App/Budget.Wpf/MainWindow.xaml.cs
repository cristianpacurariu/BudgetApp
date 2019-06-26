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

        private void AccountWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // to avoid RAM memory leaks
            AccountWindow accountWindow = sender as AccountWindow;
            if (accountWindow != null)
            {
                accountWindow.Closing -= AccountWindow_Closing;
            }

            InitializeData();
        }

        private void BtnCreateCategory_Click(object sender, RoutedEventArgs e)
        {
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            InitializeData();
        }
        private void InitializeData()
        {
            //unsub from events
            foreach (var item in spAccounts.Children)
            {
                EditableItemControl control = (EditableItemControl)item;
                control.btnEditItem.Click -= BtnEditItem_Click;
                control.btnDeleteItem.Click -= BtnDeleteItem_Click;
            }

            spAccounts.Children.Clear();
            spCategories.Children.Clear();

            //get accounts from db
            List<AccountDto> accounts = _accountRepo.All();

            foreach (AccountDto account in accounts)
            {
                EditableItemControl item = new EditableItemControl();

                item.lblName.Content = account.Name + " " + account.Currency;
                item.Id = account.Id;
                item.btnEditItem.Click += BtnEditItem_Click;
                item.btnDeleteItem.Click += BtnDeleteItem_Click;

                spAccounts.Children.Add(item);
            }

            // get categories from db
            List<OperationTypeDto> categories = _operationTypeRepo.All();

            foreach (OperationTypeDto category in categories)
            {
                EditableItemControl item = new EditableItemControl();

                item.lblName.Content = category.Name;
                item.Id = category.Id;

                spCategories.Children.Add(item);
            }
        }
        private void BtnDeleteItem_Click(object sender, RoutedEventArgs e)
        {
            AccountDto accountDto = GetClickedAccount(sender);

            try
            {
                _accountRepo.Delete(accountDto.Id);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to delete account. It has been used in other operations.");
            }

            InitializeData();
        }

        private void BtnEditItem_Click(object sender, RoutedEventArgs e)
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
        private AccountDto GetClickedAccount(object sender)
        {
            //get button parent until we reach the user control (Editable Item Control)
            DependencyObject ucParent = ((Button)sender).Parent;
            while (!(ucParent is UserControl))
            {
                ucParent = LogicalTreeHelper.GetParent(ucParent);
            }

            // cast to specific type from UserControl
            EditableItemControl userControl = ((EditableItemControl)ucParent);

            //Get from Db the account with the id of the UserControl
            AccountDto accountDto = _accountRepo.Get(userControl.Id);
            return accountDto;
        }
    }
}
