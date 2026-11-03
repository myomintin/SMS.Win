using CodeflexSystems.Library.Reflection;
using CodeflexSystems.SmartUI.Windows;
using CodeflexSystems.SMS.Core;
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
    /// Interaction logic for AboutView.xaml
    /// </summary>
    public sealed partial class AboutView : UserControl, IContent
    {
        private string title;
        private string product;
        private string version;
        private string copyright;

        public AboutView()
        {
            InitializeComponent();
            this.Loaded += AboutView_Loaded;
        }

        private void AboutView_Loaded(object sender, RoutedEventArgs e)
        {
            App.CursorWait();
            try
            {
                var assemblyInfo = new AssemblyInfo(System.Reflection.Assembly.GetExecutingAssembly());
                if (assemblyInfo == null)
                    assemblyInfo = new AssemblyInfo("SMS.exe");

                this.title = assemblyInfo.GetAssemblyTitles().FirstOrDefault();
                this.product = assemblyInfo.GetAssemblyProducts().FirstOrDefault();
                this.version = assemblyInfo.GetAssemblyVersions().FirstOrDefault();
                this.copyright = assemblyInfo.GetAssemblyCopyrights().FirstOrDefault();

                if (string.IsNullOrEmpty(this.title))
                    this.title = "Codeflex Systems SMS";
                if (string.IsNullOrEmpty(this.product))
                    this.product = "Codeflex Systems Schools Management System";
                if (string.IsNullOrEmpty(this.version))
                    this.version = "1.0.0.0";
                if (string.IsNullOrEmpty(this.copyright))
                    this.copyright = "© 2017 Codeflex Systems. All rights reserved.";

//                this.TechInfo.BBCode =
//@"[i]The technologies we proudly used.[/i]
//
//» Microsoft® .NET Framework
//» Microsoft® ADO.NET Entity Framework
//» Microsoft® SQL Server
//» Microsoft® Unity Application Block
//» MVVM Pattern
//» [url=http://mui.codeplex.com]First Floor Modern UI[/url]
//» [url=http://wpftoolkit.codeplex.com]Xceed Extended WPF Toolkit[/url]";
            }
            catch (Exception ex)
            {
                DialogManager.ShowErrorAndLog("Load", ex);
            }
            ApplicationManager.aa(false);
            RefreshAboutInfo();
            App.CursorDefault();
        }

        private void RefreshAboutInfo()
        {
            if (ApplicationManager.ad())
            {
                string info =
@"About {0}

» {1}
» {2}
» {3}
» {4}
» {5}
» {6}
» {7}
» {8}
» Register Since {9}";
                this.AboutInfo.BBCode = string.Format(info,
                    this.title, this.product, this.version, this.copyright,
                    ApplicationManager.ar(),
                    ApplicationManager.am(),
                    ApplicationManager.an(),
                    ApplicationManager.ao(),
                    ApplicationManager.ap(),
                    ApplicationManager.aq());
            }
            else
            {
                string info =
@"About {0}

» {1}
» {2}
» {3}
» {4}";
                this.AboutInfo.BBCode = string.Format(info,
                    this.title, this.product, this.version, this.copyright,
                    ApplicationManager.ar());
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ApplicationManager.ac();
            RefreshAboutInfo();
        }

        public void OnFragmentNavigation(CodeflexSystems.SmartUI.Windows.Navigation.FragmentNavigationEventArgs e)
        { }

        public void OnNavigatedFrom(CodeflexSystems.SmartUI.Windows.Navigation.NavigationEventArgs e)
        { }

        public void OnNavigatedTo(CodeflexSystems.SmartUI.Windows.Navigation.NavigationEventArgs e)
        { }

        public void OnNavigatingFrom(CodeflexSystems.SmartUI.Windows.Navigation.NavigatingCancelEventArgs e)
        {
            if (!ApplicationManager.ad())
            {
                var result = DialogManager.AskConfirmation(ApplicationManager.ai(),
                    ApplicationManager.aj(),
                    string.Format("[b]Yes[/b] to leave this page.{0}[b]No[/b] to cancel leaving and stay this page.", Environment.NewLine));
            
                if (result == DialogResult.No)
                    e.Cancel = true;
                else
                    App.AppExit();
            }
        }
    }
}
