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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CodeflexSystems.SMS.Views
{
    /// <summary>
    /// Interaction logic for StudentEducationListView.xaml
    /// </summary>
    public sealed partial class StudentEducationListView : UserControl
    {
        private bool attach;

        internal StudentViewModel ViewModel
        {
            get { return this.DataContext as StudentViewModel; }
        }

        public StudentEducationListView()
        {
            //System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            //sw.Start();
            //Console.WriteLine("Student Educations View - Create : Start");
            InitializeComponent();
            this.Loaded += (s, e) =>
            {
                //System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
                //sw.Start();
                //Console.WriteLine("Student Educations View - Loaded : Start");
                //Console.WriteLine("VM : {0}, Command : {1}", this.ViewModel == null, this.ViewModel.RefreshEducationBindingCommand == null);
                if (this.attach == false && this.ViewModel != null)
                {
                    this.ViewModel.RefreshEducationBindingCommand = new RelayCommand(o =>
                    {
                        this.ListDataGrid.ItemsSource = null;
                        this.ListDataGrid.ItemsSource = this.ViewModel.EducationViewModels;
                    });
                    this.attach = true;
                }
                //sw.Stop();
                //Console.WriteLine("Student Educations View - Loaded : End : {0}", sw.Elapsed);
            };
            //sw.Stop();
            //Console.WriteLine("Student Educations View - Create : End : {0}", sw.Elapsed);
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
