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
    /// Interaction logic for UserNewView.xaml
    /// </summary>
    public sealed partial class UserNewView : SimpleWindow
    {
        internal UserViewModel ViewModel
        {
            get { return this.DataContext as UserViewModel; }
        }

        public UserNewView()
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

        private void PasswordTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (this.ViewModel == null)
                return;

            this.ViewModel.UserParent.UserResetPasswordViewModel.Password = this.PasswordTextBox.Password;
        }

        private void ConfirmPasswordTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (this.ViewModel == null)
                return;

            this.ViewModel.UserParent.UserResetPasswordViewModel.ConfirmPassword = this.ConfirmPasswordTextBox.Password;
        }
    }
}
