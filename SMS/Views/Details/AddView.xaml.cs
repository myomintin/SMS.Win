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
    /// Interaction logic for AddView.xaml
    /// </summary>
    public sealed partial class AddView : SimpleWindow
    {
        internal AddViewModel ViewModel
        {
            get { return this.DataContext as AddViewModel; }
        }

        public AddView()
        {
            InitializeComponent();
            this.Loaded += (s, e) =>
            {
                if (this.ViewModel != null)
                {
                    this.NextButton.Command = new RelayCommand(o =>
                    {
                        this.DialogResult = true;
                    },
                    o => this.ViewModel.CanSave);
                    if (this.ViewModel.IsExisting)
                        FocusManager.SetFocusedElement(this, this.ExistingRadioButton);
                    else if (this.ViewModel.IsNew)
                        FocusManager.SetFocusedElement(this, this.NewRadioButton);
                }
            };
        }
    }
}
