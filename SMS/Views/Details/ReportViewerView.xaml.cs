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
    /// Interaction logic for ReportViewerView.xaml
    /// </summary>
    public sealed partial class ReportViewerView : SimpleWindow
    {
        private bool attach;

        public ReportViewerView()
        {
            InitializeComponent();
            this.Loaded += (s, e) =>
            {
                if (this.attach == false)
                {
                    this.Viewer.BorderStyle = System.Windows.Forms.BorderStyle.None;
                    this.Viewer.ShowCloseButton = false;
                    this.Viewer.ShowGroupTreeButton = false;
                    this.Viewer.ShowLogo = false;
                    this.Viewer.ShowParameterPanelButton = false;
                    this.Viewer.ToolPanelView = CrystalDecisions.Windows.Forms.ToolPanelViewType.None;
                    if (this.DataContext != null &&  this.DataContext is CrystalDecisions.CrystalReports.Engine.ReportClass)
                        this.Viewer.ReportSource = this.DataContext;
                    this.attach = true;
                }
            };
        }
    }
}
