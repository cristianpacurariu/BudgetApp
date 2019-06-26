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
    /// Interaction logic for EditableItemControl.xaml
    /// </summary>
    public partial class EditableItemControl : UserControl
    {
        public EditableItemControl()
        {
            InitializeComponent();
        }
        public int Id { get; set; }

    }
}
