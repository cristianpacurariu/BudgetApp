﻿using Budget.Domain.Repositories;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Budget.Wpf
{
    public partial class EditableTransactionControl : UserControl
    {
        public EditableTransactionControl()
        {
            InitializeComponent();
        }

        public int IdOperation { get; set; }
    }
}
