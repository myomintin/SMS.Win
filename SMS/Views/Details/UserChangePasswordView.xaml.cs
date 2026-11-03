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
    /// Interaction logic for UserChangePasswordView.xaml
    /// </summary>
    public sealed partial class UserChangePasswordView : SimpleWindow
    {
        internal UserChangePasswordViewModel ViewModel
        {
            get { return this.DataContext as UserChangePasswordViewModel; }
        }

        public UserChangePasswordView()
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

        private void CurrentPasswordTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (this.ViewModel == null)
                return;

            this.ViewModel.CurrentPassword = this.CurrentPasswordTextBox.Password;
        }

        private void NewPasswordTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (this.ViewModel == null)
                return;

            this.ViewModel.NewPassword = this.NewPasswordTextBox.Password;
        }

        private void ConfirmPasswordTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (this.ViewModel == null)
                return;

            this.ViewModel.ConfirmPassword = this.ConfirmPasswordTextBox.Password;
        }
    }
}
