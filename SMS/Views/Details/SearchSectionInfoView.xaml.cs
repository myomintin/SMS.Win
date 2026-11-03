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
    /// Interaction logic for SearchSectionInfoView.xaml
    /// </summary>
    public sealed partial class SearchSectionInfoView : SimpleWindow
    {
        internal SearchSectionInfoViewModel ViewModel
        {
            get { return this.DataContext as SearchSectionInfoViewModel; }
        }

        public SearchSectionInfoView()
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
                }
            };
        }

        private void ListDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (this.OKButton.Command.CanExecute(this.OKButton.CommandParameter))
                this.OKButton.Command.Execute(this.OKButton.CommandParameter);
        }

        private void ListDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.ListDataGrid.SelectedItem == null)
                return;

            this.ListDataGrid.ScrollIntoView(this.ListDataGrid.SelectedItem);
        }
    }
}
