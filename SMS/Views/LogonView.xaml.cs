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
    /// Interaction logic for LogonView.xaml
    /// </summary>
    public sealed partial class LogonView : SimpleWindow
    {
        internal LogonViewModel ViewModel
        {
            get { return this.DataContext as LogonViewModel; }
        }

        public LogonView()
        {
            InitializeComponent();
            this.Loaded += (s, e) =>
            {
                if (this.ViewModel == null)
                {
                    this.DataContext = ApplicationManager.Resolve<LogonViewModel>();
                    this.LogonButton.Command = new RelayCommand(o =>
                    {
                        this.DialogResult = true;
                    },
                    o => this.ViewModel.CanSave);
                }
            };
        }

        private void PasswordTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            this.ViewModel.Password = this.PasswordTextBox.Password;
        }
    }
}
