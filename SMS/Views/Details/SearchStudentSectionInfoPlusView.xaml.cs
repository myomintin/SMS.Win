using CodeflexSystems.SmartUI.Windows;
using CodeflexSystems.SmartUI.Windows.Input;
using CodeflexSystems.SMS.Core;
using CodeflexSystems.SMS.Extensions;
using CodeflexSystems.SMS.Models;
using CodeflexSystems.SMS.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
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
    /// Interaction logic for SearchStudentSectionInfoPlusView.xaml
    /// </summary>
    public sealed partial class SearchStudentSectionInfoPlusView : SimpleWindow
    {
        private Guid lastId;
        private DataTemplate studentSectionTemplate;

        internal SearchStudentSectionInfoViewModel ViewModel
        {
            get { return this.DataContext as SearchStudentSectionInfoViewModel; }
        }

        public SearchStudentSectionInfoPlusView()
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
                    this.lastId = Guid.Empty;
                    this.ViewModel.Collection.SelectionChanged += Collection_SelectionChanged;
                    this.studentSectionTemplate = this.FindResource("StudentSectionInfoTreeViewItemTemplate") as DataTemplate;
                }
            };
        }

        private void SearchListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (this.OKButton.Command.CanExecute(this.OKButton.CommandParameter))
                this.OKButton.Command.Execute(this.OKButton.CommandParameter);
        }

        private void Collection_SelectionChanged(object sender, CodeflexSystems.Library.Core.NotifySelectionChangedEventArgs e)
        {
            if (this.ViewModel.Collection.SelectedIndex == -1)
            {
                this.StudentTreeView.Items.Clear();
                this.lastId = Guid.Empty;
                return;
            }

            var current = this.ViewModel.Collection.SelectedItem;
            if (this.lastId == current.Student.StudentId)
                return;

            try
            {
                App.CursorWait();
                var uow = this.ViewModel.UoW;

                this.StudentTreeView.Items.Clear();
                foreach (var sg in current.Student.StudentGuardians)
                {
                    var sgitem = new TreeViewItem();

                    sgitem.Header = string.Format("{0} ( {1} )", sg.Guardian.TitleGuardianName, sg.GuardianType);
                    foreach (var s in uow.Where<StudentGuardian>
                        (m => m.GuardianId == sg.GuardianId).Select(m => m.Student).AsNoTracking())
                    {
                        var sitem = new TreeViewItem();

                        sitem.DataContext = s;
                        sitem.Header = string.Format("{0}, {1}", s.Id, s.StudentFullName);
                        sitem.Items.Add("*");
                        sgitem.Items.Add(sitem);
                    }
                    sgitem.IsExpanded = true;
                    this.StudentTreeView.Items.Add(sgitem);
                }
                this.lastId = current.Student.StudentId;
            }
            catch (Exception ex)
            {
                DialogManager.ShowErrorAndLog("Expanding Guardians", ex);
            }
            finally
            {
                App.CursorDefault();
            }
        }

        private void StudentTreeView_Expanded(object sender, RoutedEventArgs e)
        {
            try
            {
                var item = e.OriginalSource as TreeViewItem;
                var flag = false;

                if (item.Tag != null)
                    bool.TryParse(item.Tag.ToString(), out flag);
                if (item == null || item.DataContext == null || flag)
                    return;

                App.CursorWait();
                if (item.DataContext is Student)
                {
                    var dc = item.DataContext as Student;

                    item.Items.Clear();
                    if (this.studentSectionTemplate == null)
                    {
                        foreach (var ss in dc.StudentSections)
                        {
                            var ssitem = new TreeViewItem();

                            ssitem.Header = ss.Section.SectionNameWithOpenDayStartEndTime;
                            item.Items.Add(ssitem);
                        }
                    }
                    else
                    {
                        item.ItemsSource = this.ViewModel.UoW.GetStudentSectionInfos
                            (m => m.StudentId == dc.StudentId).Ordered().AsNoTracking().ToList();
                        item.ItemTemplate = this.studentSectionTemplate;
                    }
                    item.Tag = true;
                }
            }
            catch (Exception ex)
            {
                DialogManager.ShowErrorAndLog("Expanding Sections", ex);
            }
            finally
            {
                App.CursorDefault();
            }
        }
    }
}
