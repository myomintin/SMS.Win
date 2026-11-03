using CodeflexSystems.SmartUI.Windows.Input;
using CodeflexSystems.SMS.Core;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CodeflexSystems.SMS.Views
{
    /// <summary>
    /// Interaction logic for PhoneListView.xaml
    /// </summary>
    public sealed partial class PhoneListView : UserControl
    {
        private bool attach;

        internal IPhoneViewModel ViewModel
        {
            get { return this.DataContext as IPhoneViewModel; }
        }

        public PhoneListView()
        {
            InitializeComponent();
            UIHelper.AddStateColumns(this.ListDataGrid);
            this.Loaded += (s, e) =>
            {
                if (this.attach == false && this.ViewModel != null)
                {
                    this.ViewModel.RefreshPhoneBindingCommand = new RelayCommand(o =>
                    {
                        this.ListDataGrid.ItemsSource = null;
                        this.ListDataGrid.ItemsSource = this.ViewModel.PhoneViewModels;
                    });
                    //this.ViewModel.PhonePropertiesChanged = (vm) =>
                    //{
                    //    vm.RaisePropertyChanged("PhoneType");
                    //    vm.RaisePropertyChanged("PhoneNumber");
                    //    vm.RaisePropertyChanged("Extension");
                    //    vm.RaisePropertyChanged("CreatedDateLT");
                    //    vm.RaisePropertyChanged("ModifiedDateLT");
                    //};
                    this.attach = true;
                }
            };
        }

        private void ListDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (this.EditButton.Command.CanExecute(this.EditButton.CommandParameter))
                this.EditButton.Command.Execute(this.EditButton.CommandParameter);
        }

        private void ListDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.ListDataGrid.SelectedItem == null)
                return;

            this.ListDataGrid.ScrollIntoView(this.ListDataGrid.SelectedItem);
        }
    }
}
