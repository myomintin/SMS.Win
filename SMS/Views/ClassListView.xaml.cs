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
    /// Interaction logic for ClassListView.xaml
    /// </summary>
    public sealed partial class ClassListView : UserControl, IContent
    {
        internal ClassListViewModel ViewModel
        {
            get { return this.DataContext as ClassListViewModel; }
        }

        public ClassListView()
        {
            InitializeComponent();
            UIHelper.AddStateColumns(this.ListDataGrid);
            this.Loaded += (s, e) =>
            {
                if (this.ViewModel == null)
                {
                    App.CursorWait();
                    this.DataContext = ApplicationManager.Resolve<ClassListViewModel>();
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
            var result = DialogResult.None;

            try
            {
                if (this.ViewModel.CanSave)
                {
                    result = DialogManager.AskConfirmationToSaveData();

                    if (result == DialogResult.Yes)
                    {
                        this.ViewModel.OnSave();
                        e.Cancel = this.ViewModel.CanSave;
                    }
                    else if (result == DialogResult.Cancel)
                        e.Cancel = true;
                }
            }
            catch (Exception ex)
            {
                DialogManager.ShowErrorAndLog("Navigation Page", ex);
            }
            finally
            {
                if (!e.Cancel)
                {
                    this.ViewModel.ClearAllCollections();
                    this.ViewModel.Dispose();
                }
            }
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
