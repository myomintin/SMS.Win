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
    /// Interaction logic for SearchStudentView.xaml
    /// </summary>
    public sealed partial class SearchStudentView : SimpleWindow
    {
        internal SearchStudentViewModel ViewModel
        {
            get { return this.DataContext as SearchStudentViewModel; }
        }

        public SearchStudentView()
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

        private void SearchListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (this.OKButton.Command.CanExecute(this.OKButton.CommandParameter))
                this.OKButton.Command.Execute(this.OKButton.CommandParameter);
        }
    }
}
