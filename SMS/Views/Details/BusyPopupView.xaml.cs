using CodeflexSystems.SmartUI.Windows;
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
    /// Interaction logic for BusyPopupView.xaml
    /// </summary>
    public sealed partial class BusyPopupView : BlankWindow
    {
        public bool IsClosed { get; private set; }

        public BusyPopupView()
        {
            InitializeComponent();
            this.Top = (SystemParameters.PrimaryScreenHeight - this.Height) / 2 - 100;
            this.Left = (SystemParameters.PrimaryScreenWidth - this.Width) / 2;
            this.Closed += (s, e) =>
            {
                this.IsClosed = true;
            };
        }

        public void DoClose()
        {
            if (this.IsClosed == false)
                this.Close();
        }
    }
}
