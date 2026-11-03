using CodeflexSystems.SmartUI.Windows;
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
    /// Interaction logic for StudentFerryFeeListView.xaml
    /// </summary>
    public sealed partial class StudentFerryFeeListView : UserControl, IContent
    {
        internal StudentFerryFeeListViewModel ViewModel
        {
            get { return this.DataContext as StudentFerryFeeListViewModel; }
        }

        public StudentFerryFeeListView()
        {
            InitializeComponent();
            UIHelper.AddStateColumns(this.ListDataGrid);
            this.Loaded += (s, e) =>
            {
                if (this.ViewModel == null)
                {
                    App.CursorWait();
                    this.DataContext = ApplicationManager.Resolve<StudentFerryFeeListViewModel>();
                    this.ViewModel.RefreshBindingCommand = new RelayCommand(o =>
                    {
                        this.ListDataGrid.Items.Refresh();
                    });
                    App.CursorDefault();
                }
            };
        }

        public void OnFragmentNavigation(CodeflexSystems.SmartUI.Windows.Navigation.FragmentNavigationEventArgs e)
        { }

        public void OnNavigatedFrom(CodeflexSystems.SmartUI.Windows.Navigation.NavigationEventArgs e)
        { }

        public void OnNavigatedTo(CodeflexSystems.SmartUI.Windows.Navigation.NavigationEventArgs e)
        { }

        public void OnNavigatingFrom(CodeflexSystems.SmartUI.Windows.Navigation.NavigatingCancelEventArgs e)
        {
            this.ViewModel.ClearAllCollections();
            this.ViewModel.Dispose();
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
