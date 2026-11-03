using CodeflexSystems.Library.Core;
using CodeflexSystems.SmartUI.Windows.Controls;
using CodeflexSystems.SMS.Core;
using CodeflexSystems.SMS.Models;
using CodeflexSystems.SMS.Reports;
using CodeflexSystems.SMS.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CodeflexSystems.SMS.Views
{
    /// <summary>
    /// Interaction logic for ReportListBaseView.xaml
    /// </summary>
    public partial class ReportListBaseView : UserControl
    {
        private ReportLevel oldLevel;
        private bool isCheckedAll;

        internal ReportListBaseViewModel ViewModel
        {
            get { return this.DataContext as ReportListBaseViewModel; }
        }

        public ReportListBaseView()
        {
            InitializeComponent();
            this.Loaded += (s, e) =>
            {
                if (this.ViewModel != null)
                {
                    this.oldLevel = ReportLevel.None;
                    this.ViewModel.Collection.SelectionChanged += CollectionChanged;
                    this.ViewModel.Schools.SelectionChanged += SchoolChanged;
                    this.ViewModel.AcademicYearChanged = AcademicYearChanged;
                    this.ViewModel.ActiveCollection.SelectionChanged += ActiveCollectionChanged;
                }
            };
        }

        private void ExpandingSchools(ItemsControl parent)
        {
            if (this.ViewModel.SelectedItem.DisplayLevel < ReportLevel.School)
                return;

            parent.Items.Clear();
            foreach (var value in this.ViewModel.Schools)
            {
                var item = new TreeViewCheckBoxItem();

                item.DataContext = value;
                item.Header = value.Key.SchoolName;
                item.SetBinding(TreeViewCheckBoxItem.IsCheckedProperty, "Value1");
                parent.Items.Add(item);
            }
        }

        private void ExpandingAcademicYears(ItemsControl parent)
        {
            if (this.ViewModel.SelectedItem.DisplayLevel < ReportLevel.AcademicYear)
                return;

            parent.Items.Clear();
            if (this.ViewModel.Schools.SelectedIndex == -1)
                return;

            var key = this.ViewModel.Schools.SelectedItem.Key;
            foreach (var value in this.ViewModel.AcademicYears[key.SchoolId])
            {
                var item = new TreeViewCheckBoxItem();

                item.DataContext = value;
                item.Header = value.Key.AcademicYearName;
                item.SetBinding(TreeViewCheckBoxItem.IsCheckedProperty, "Value1");
                parent.Items.Add(item);
            }
        }

        private void ExpandingCourses(ItemsControl parent)
        {
            if (this.ViewModel.SelectedItem.DisplayLevel < ReportLevel.Course)
                return;

            parent.Items.Clear();
            if (this.ViewModel.AcademicYearCollection.SelectedIndex == -1)
                return;

            var key = this.ViewModel.AcademicYearCollection.SelectedItem.Key;
            this.ViewModel.Load(key);
            foreach (var value in this.ViewModel.Courses[key.AcademicYearId])
            {
                var item = new TreeViewCheckBoxItem();

                item.DataContext = value;
                item.Header = value.Key.CourseName;
                item.SetBinding(TreeViewCheckBoxItem.IsCheckedProperty, "Value1");
                if (this.ViewModel.SelectedItem.DisplayLevel > ReportLevel.Course)
                    item.Items.Add("*");
                parent.Items.Add(item);
            }
        }

        private void ExpandingClasses(ItemsControl parent)
        {
            if (this.ViewModel.SelectedItem.DisplayLevel < ReportLevel.Class)
                return;

            var dc = parent.DataContext as KeyValues<Course, bool?>;

            parent.Items.Clear();
            this.ViewModel.Load(dc.Key);
            foreach (var value in this.ViewModel.Classes[dc.Key.CourseId])
            {
                var item = new TreeViewCheckBoxItem();

                item.DataContext = value;
                item.Header = value.Key.ClassName;
                item.SetBinding(TreeViewCheckBoxItem.IsCheckedProperty, "Value1");
                if (this.ViewModel.SelectedItem.DisplayLevel > ReportLevel.Class)
                    item.Items.Add("*");
                parent.Items.Add(item);
            }
        }

        private void ExpandingSections(ItemsControl parent)
        {
            if (this.ViewModel.SelectedItem.DisplayLevel < ReportLevel.Section)
                return;

            var dc = parent.DataContext as KeyValues<Class, bool?>;

            parent.Items.Clear();
            this.ViewModel.Load(dc.Key);
            foreach (var value in this.ViewModel.Sections[dc.Key.ClassId])
            {
                var item = new TreeViewCheckBoxItem();

                item.DataContext = value;
                item.Header = value.Key.SectionName;
                item.SetBinding(TreeViewCheckBoxItem.IsCheckedProperty, "Value1");
                if (this.ViewModel.SelectedItem.DisplayLevel > ReportLevel.Section)
                    item.Items.Add("*");
                parent.Items.Add(item);
            }
        }

        private void ExpandingStudentSections(ItemsControl parent)
        {
            if (this.ViewModel.SelectedItem.DisplayLevel < ReportLevel.StudentSection)
                return;

            var dc = parent.DataContext as KeyValues<Section, bool?>;

            parent.Items.Clear();
            this.ViewModel.Load(dc.Key);

            var filter = this.ViewModel.StudentSections[dc.Key.SectionId].AsEnumerable();
            var index = this.ViewModel.ActiveCollection.SelectedIndex;

            if (index == 0)
                filter = filter.Where(m => m.Key.Active);
            else if (index == 1)
                filter = filter.Where(m => m.Key.Active == false);
            foreach (var value in filter)
            {
                var item = new TreeViewCheckBoxItem();

                item.DataContext = value;
                item.Header = value.Key.Student.StudentFullName;
                item.SetBinding(TreeViewCheckBoxItem.IsCheckedProperty, "Value1");
                parent.Items.Add(item);
            }
        }

        private void ShowSchool(bool show)
        {
            if (show)
                this.SchoolGrid.Visibility = Visibility.Visible;
            else
                this.SchoolGrid.Visibility = Visibility.Collapsed;
        }

        private void ShowAcademicYear(bool show)
        {
            if (show)
                this.AcademicYearGrid.Visibility = Visibility.Visible;
            else
                this.AcademicYearGrid.Visibility = Visibility.Collapsed;
        }

        private void ShowActive(bool show)
        {
            if (show)
                this.ActiveGrid.Visibility = Visibility.Visible;
            else
                this.ActiveGrid.Visibility = Visibility.Collapsed;
        }

        private void ShowSectionNameFormat(bool show)
        {
            if (show)
                this.SectionNameFormatGrid.Visibility = Visibility.Visible;
            else
                this.SectionNameFormatGrid.Visibility = Visibility.Collapsed;
        }

        private void ShowStudentNameFormat(bool show)
        {
            if (show)
                this.StudentNameFormatGrid.Visibility = Visibility.Visible;
            else
                this.StudentNameFormatGrid.Visibility = Visibility.Collapsed;
        }

        private void ShowAddressFormat(bool show)
        {
            if (show)
                this.AddressFormatGrid.Visibility = Visibility.Visible;
            else
                this.AddressFormatGrid.Visibility = Visibility.Collapsed;
        }

        private TreeViewCheckBoxItem GetSelectedItem()
        {
            if (this.SchoolsTreeView.SelectedItem == null)
                return null;

            var item = this.SchoolsTreeView.SelectedItem as TreeViewCheckBoxItem;

            return item;
        }

        private bool GetItemValue(TreeViewCheckBoxItem item)
        {
            return item.IsChecked.HasValue ? item.IsChecked.Value : false;
        }

        private void SchoolsTreeView_Expanded(object sender, RoutedEventArgs e)
        {
            try
            {
                var item = e.OriginalSource as TreeViewCheckBoxItem;

                if (item == null || item.DataContext == null || (item.HasItems && item.Items[0] is TreeViewCheckBoxItem))
                    return;

                //if (item.DataContext is KeyValues<School, bool?>)
                //    ExpandingAcademicYears(item);
                //else if (item.DataContext is KeyValues<AcademicYear, bool?>)
                //    ExpandingCourses(item);
                //else
                if (item.DataContext is KeyValues<Course, bool?>)
                    ExpandingClasses(item);
                else if (item.DataContext is KeyValues<Class, bool?>)
                    ExpandingSections(item);
                else if (item.DataContext is KeyValues<Section, bool?>)
                    ExpandingStudentSections(item);
            }
            catch (Exception ex)
            {
                DialogManager.ShowErrorAndLog("Expanding Tree View", ex);
            }
            finally
            {
                App.CursorDefault();
            }
        }

        private void CollectionChanged(object sender, NotifySelectionChangedEventArgs e)
        {
            try
            {
                App.CursorWait();
                if (this.ViewModel.Collection.SelectedIndex == -1)
                {
                    this.oldLevel = ReportLevel.None;
                    this.SchoolsTreeView.Items.Clear();
                    this.SchoolsTreeView.Visibility = Visibility.Hidden;
                    ShowSchool(false);
                    ShowAcademicYear(false);
                    ShowActive(false);
                    ShowSectionNameFormat(false);
                    ShowStudentNameFormat(false);
                    ShowAddressFormat(false);
                    this.ReportInfoGroupBox.Visibility = Visibility.Hidden;

                    return;
                }

                ShowActive(this.ViewModel.SelectedItem.ShowActive);
                ShowSectionNameFormat(this.ViewModel.SelectedItem.ShowSectionNameFormat);
                ShowStudentNameFormat(this.ViewModel.SelectedItem.ShowStudentNameFormat);
                ShowAddressFormat(this.ViewModel.SelectedItem.ShowAddressFormat);
                if (this.ReportInfoGroupBox.Visibility == Visibility.Hidden)
                    this.ReportInfoGroupBox.Visibility = Visibility.Visible;

                var dl = this.ViewModel.SelectedItem.DisplayLevel;
                if (dl == this.oldLevel)
                    return;

                if (dl == ReportLevel.None)
                {
                    this.SchoolsTreeView.Visibility = Visibility.Hidden;
                    ShowSchool(false);
                    ShowAcademicYear(false);
                }
                else
                {
                    this.ViewModel.Load();
                    if (dl == ReportLevel.School)
                    {
                        ExpandingSchools(this.SchoolsTreeView);
                        ShowSchool(false);
                        ShowAcademicYear(false);
                    }
                    else if (dl == ReportLevel.AcademicYear)
                    {
                        this.ViewModel.Load(this.ViewModel.Schools.SelectedItem.Key);
                        ExpandingAcademicYears(this.SchoolsTreeView);
                        ShowSchool(true);
                        ShowAcademicYear(false);
                    }
                    else
                    {
                        this.ViewModel.Load(this.ViewModel.Schools.SelectedItem.Key);
                        ExpandingCourses(this.SchoolsTreeView);
                        ShowSchool(true);
                        ShowAcademicYear(true);
                    }
                    this.SchoolsTreeView.Visibility = Visibility.Visible;
                }

                if (this.ViewModel.Collection.SelectedIndex == -1)
                    this.oldLevel = ReportLevel.None;
                else
                    this.oldLevel = this.ViewModel.SelectedItem.DisplayLevel;
            }
            catch (Exception ex)
            {
                DialogManager.ShowErrorAndLog("Collection Changed", ex);
            }
            finally
            {
                App.CursorDefault();
            }
        }

        private void SchoolChanged(object sender, NotifySelectionChangedEventArgs e)
        {
            if (this.ViewModel.Collection.SelectedIndex == -1 ||
                this.ViewModel.SelectedItem.DisplayLevel != this.oldLevel)
                return;

            try
            {
                App.CursorWait();
                this.ViewModel.Load(this.ViewModel.Schools.SelectedItem.Key);
                if (this.ViewModel.SelectedItem.DisplayLevel == ReportLevel.AcademicYear)
                    ExpandingAcademicYears(this.SchoolsTreeView);
                //else
                //    ExpandingCourses(this.SchoolsTreeView);
            }
            catch (Exception ex)
            {
                DialogManager.ShowErrorAndLog("School Changed", ex);
            }
            finally
            {
                App.CursorDefault();
            }
        }

        private void AcademicYearChanged()
        {
            if (this.ViewModel.Collection.SelectedIndex == -1 ||
                this.ViewModel.SelectedItem.DisplayLevel != this.oldLevel)
                return;

            try
            {
                App.CursorWait();
                ExpandingCourses(this.SchoolsTreeView);
            }
            catch (Exception ex)
            {
                DialogManager.ShowErrorAndLog("Academic Year Changed", ex);
            }
            finally
            {
                App.CursorDefault();
            }
        }

        private void FindSection(TreeViewCheckBoxItem item)
        {
            if (item == null || item.DataContext == null || item.IsExpanded == false)
                return;

            if (item.DataContext is KeyValues<Section, bool?>)
                ExpandingStudentSections(item);
            else
                LoopItems(item.Items.Cast<TreeViewCheckBoxItem>());
        }

        private void LoopItems(IEnumerable<TreeViewCheckBoxItem> source)
        {
            foreach (var item in source)
                FindSection(item);
        }

        private void ActiveCollectionChanged(object sender, NotifySelectionChangedEventArgs e)
        {
            if (this.ViewModel.Collection.SelectedIndex == -1 || this.ViewModel.ActiveCollection.SelectedIndex == -1 ||
                this.ViewModel.SelectedItem.DisplayLevel < ReportLevel.StudentSection)
                return;

            try
            {
                App.CursorWait();
                LoopItems(this.SchoolsTreeView.Items.Cast<TreeViewCheckBoxItem>());
            }
            catch (Exception ex)
            {
                DialogManager.ShowErrorAndLog("Active Changed", ex);
            }
            finally
            {
                App.CursorDefault();
            }
        }

        private void CheckAllMenuItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.isCheckedAll = !this.isCheckedAll;
                foreach (var item in this.SchoolsTreeView.Items.Cast<TreeViewCheckBoxItem>())
                    item.IsChecked = this.isCheckedAll;
            }
            catch (Exception ex)
            {
                DialogManager.ShowErrorAndLog("Check All Menu Item", ex);
            }
        }

        private void CheckMenuItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var item = GetSelectedItem();
                if (item == null)
                    return;

                bool flag = !GetItemValue(item);
                item.IsChecked = flag;
            }
            catch (Exception ex)
            {
                DialogManager.ShowErrorAndLog("Check Menu Item", ex);
            }
        }

        private void UnselectMenuItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var item = GetSelectedItem();
                if (item == null)
                    return;

                item.IsSelected = false;
            }
            catch (Exception ex)
            {
                DialogManager.ShowErrorAndLog("Unselect Menu Item", ex);
            }
        }

        private void TreeViewContextMenu_Opened(object sender, RoutedEventArgs e)
        {
            try
            {
                bool flag = false;
                var item = GetSelectedItem();
                var items = this.SchoolsTreeView.Items.Cast<TreeViewCheckBoxItem>();

                if (items.All(m => m.IsChecked == true))
                    this.isCheckedAll = true;
                if (items.All(m => m.IsChecked == false))
                    this.isCheckedAll = false;
                if (item != null)
                {
                    flag = true;
                    this.CheckMenuItem.Header = GetItemValue(item) ? "Uncheck" : "Check";
                }
                this.CheckAllMenuItem.Header = this.isCheckedAll ? "Uncheck All" : "Check All";
                this.CheckMenuItem.IsEnabled = flag;
                this.UnselectMenuItem.IsEnabled = flag;
            }
            catch (Exception ex)
            {
                DialogManager.ShowErrorAndLog("Tree View Context Menu", ex);
            }
        }
    }
}
