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
    /// Interaction logic for ReceiptEditView.xaml
    /// </summary>
    public sealed partial class ReceiptEditView : SimpleWindow
    {
        private bool attach;

        internal ReceiptViewModel ViewModel
        {
            get { return this.DataContext as ReceiptViewModel; }
        }

        public ReceiptEditView()
        {
            InitializeComponent();
            this.Loaded += (s, e) =>
            {
                if (this.attach == false && this.ViewModel != null)
                {
                    this.ViewModel.RefreshReceiptDetailBindingCommand = new RelayCommand(o =>
                    {
                        this.ListDataGrid.ItemsSource = null;
                        this.ListDataGrid.ItemsSource = this.ViewModel.ReceiptDetailViewModels;
                    });
                    this.OKButton.Command = new RelayCommand(o =>
                    {
                        this.DialogResult = true;
                    },
                    o => this.ViewModel.CanSave);
                    this.attach = true;
                }
            };
        }

        private void ListDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (this.EditButton.Command.CanExecute(this.EditButton.CommandParameter))
                this.EditButton.Command.Execute(this.EditButton.CommandParameter);
        }
    }
}
