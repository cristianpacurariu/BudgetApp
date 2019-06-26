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
    /// Interaction logic for CategoryWindow.xaml
    /// </summary>
    public partial class CategoryWindow : Window
    {
        private readonly IOperationTypeRepo<OperationTypeDto> _operationTypeRepo = RepoProvider.GetOperationTypeRepo();
        public CategoryWindow()
        {
            InitializeComponent();
        }
        public int? Id { get; set; }

        private void BtnSaveCategory_Click(object sender, RoutedEventArgs e)
        {
            OperationTypeDto operationTypeDto = new OperationTypeDto();

            operationTypeDto.Name = tbCategoryName.Text;
            if (rbExpense.IsChecked == true)
            {
                operationTypeDto.IsCredit = false;
            }
            if (rbIncome.IsChecked == true)
            {
                operationTypeDto.IsCredit = true;
            }

            if (Id == null)
            {
                _operationTypeRepo.Add(operationTypeDto);
            }
            else
            {
                operationTypeDto.Id = Id.Value;
                _operationTypeRepo.Update(operationTypeDto);
            }

            this.Close();
        }
    }
}
