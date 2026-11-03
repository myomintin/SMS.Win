using CodeflexSystems.SmartUI.Windows;
using CodeflexSystems.SmartUI.Windows.Input;
using CodeflexSystems.SMS.ViewModels;
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

namespace CodeflexSystems.SMS.Views
{
    /// <summary>
    /// Interaction logic for ReceiptDetailEditView.xaml
    /// </summary>
    public sealed partial class ReceiptDetailEditView : SimpleWindow
    {
        internal ReceiptDetailViewModel ViewModel
        {
            get { return this.DataContext as ReceiptDetailViewModel; }
        }

        public ReceiptDetailEditView()
        {
            InitializeComponent();
            this.Loaded += (s, e) =>
            {
                if (this.ViewModel != null)
                {
                    this.OKButton.Command = new RelayCommand(o =>
                    {
                        this.DialogResult = true;
                    },
                    o => this.ViewModel.CanSave);
                    if (this.ViewModel.CanSelectMonth)
                        FocusManager.SetFocusedElement(this, this.QuantityByteUpDown);
                    else
                        FocusManager.SetFocusedElement(this, this.AmountDecimalUpDown);
                }
            };
        }
    }
}
