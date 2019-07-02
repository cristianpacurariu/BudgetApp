using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;

namespace Budget.Wpf
{
    public class CurrencyRadio : RadioButton
    {
        public CurrencyRadio()
        {
            GroupName = "CurrencyGroup";
            Name = "btnCurrency";
            VerticalAlignment = VerticalAlignment.Center;
            HorizontalAlignment = HorizontalAlignment.Center;
            Thickness margin = Margin;
            margin.Left = 5;
            margin.Top = 5;
            margin.Right = 5;
            margin.Bottom = 5;
            Margin = margin;

            FontSize = 14;
            FontFamily = new FontFamily("Century Gotic");

        }

        public int Id { get; set; }
    }
}
