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
    /// Interaction logic for StudentEditView.xaml
    /// </summary>
    public sealed partial class StudentEditView : SimpleWindow
    {
        internal StudentViewModel ViewModel
        {
            get { return this.DataContext as StudentViewModel; }
        }

        public StudentEditView()
        {
            //System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            //sw.Start();
            //Console.WriteLine("Student Edit View - Create : Start");
            InitializeComponent();
            this.Loaded += (s, e) =>
            {
                //Console.WriteLine("Loaded Student Edit");
                if (this.ViewModel != null)
                {
                    this.OKButton.Command = new RelayCommand(o =>
                    {
                        this.DialogResult = true;
                    },
                    o => this.ViewModel.CanSave);
                }
                FocusManager.SetFocusedElement(this, this.IdTextBox);
            };
            //sw.Stop();
            //Console.WriteLine("Student Edit View - Create : End : {0}", sw.Elapsed);
        }

        private void GenderChecked(object sender, RoutedEventArgs e)
        {
            if (this.ViewModel == null)
                return;

            this.ViewModel.Gender = this.MaleRadioButton.IsChecked.Value;
        }
    }
}
