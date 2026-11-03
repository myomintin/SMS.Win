using CodeflexSystems.SmartUI.Windows;
using CodeflexSystems.SmartUI.Windows.Controls;
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
using System.Windows.Shapes;

namespace CodeflexSystems.SMS.Views
{
    /// <summary>
    /// Interaction logic for UserPermissionsView.xaml
    /// </summary>
    public sealed partial class UserPermissionsView : SimpleWindow
    {
        private bool attach;

        internal UserViewModel ViewModel
        {
            get { return this.DataContext as UserViewModel; }
        }

        public UserPermissionsView()
        {
            InitializeComponent();
            this.Loaded += UserPermissionsView_Loaded;
        }

        private void UserPermissionsView_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.ViewModel == null || this.attach)
                return;

            try
            {
                this.OKButton.Command = new RelayCommand(o =>
                {
                    this.DialogResult = true;
                },
                o => this.ViewModel.CanSavePermissions);
                this.Title = string.Format("Permissions for {0}", this.ViewModel.UserName);
                foreach (var gm in ApplicationManager.Current.GroupMenus)
                {
                    bool mflag = true;
                    //var mitem = new TreeViewCheckBoxItem();
                    var mitem = new TreeViewItem();

                    mitem.Tag = gm.Key;
                    mitem.Header = gm.Value1;
                    this.MenusTreeView.Items.Add(mitem);

                    foreach (var lm in gm.Value2)
                    {
                        bool lflag = this.ViewModel.IsCanView(lm.Key);
                        var litem = new TreeViewCheckBoxItem();

                        litem.Tag = lm.Key;
                        litem.Header = lm.Value1;
                        litem.IsChecked = lflag;
                        litem.Checked += LinkMenuTreeViewCheckBoxItem_CheckChanged;
                        litem.Unchecked += LinkMenuTreeViewCheckBoxItem_CheckChanged;
                        mflag &= lflag;
                        mitem.Items.Add(litem);
                    }

                    //mitem.IsChecked = mflag;
                    //mitem.Checked += GroupMenuTreeViewCheckBoxItem_CheckChanged;
                    //mitem.Unchecked += GroupMenuTreeViewCheckBoxItem_CheckChanged;
                    mitem.ExpandSubtree();
                }
                BuildMenus();
                this.MenusTreeView.SelectedItemChanged += MenusTreeView_SelectedItemChanged;
                this.attach = true;
            }
            catch (Exception ex)
            {
                if (this.ViewModel == null)
                    throw ex;
                else
                    DialogManager.ShowErrorAndLog("Load", ex);
            }
        }

        private void MenusTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (this.attach == false)
                return;

            try
            {
                bool flag = false;
                var item = this.MenusTreeView.SelectedItem as TreeViewCheckBoxItem;

                if (item != null)
                {
                    Guid id;

                    if (Guid.TryParse(item.Tag.ToString(), out id))
                    {
                        var vm = this.ViewModel.GetUserPermissionViewModel(id);

                        if (vm != null)
                            flag = true;
                        this.MenuInfoGrid.DataContext = vm;
                    }
                }
                if (flag)
                    this.MenuInfoGrid.Visibility = Visibility.Visible;
                else
                    this.MenuInfoGrid.Visibility = Visibility.Hidden;
            }
            catch (Exception ex)
            {
                DialogManager.ShowErrorAndLog("Menu Info", ex);
            }
        }

        //private void GroupMenuTreeViewCheckBoxItem_CheckChanged(object sender, RoutedEventArgs e)
        //{
        //    var gitem = sender as TreeViewCheckBoxItem;

        //    if (gitem != null)
        //    {
        //        foreach (TreeViewCheckBoxItem item in gitem.Items)
        //            item.IsChecked = gitem.IsChecked;
        //    }
        //}

        private void LinkMenuTreeViewCheckBoxItem_CheckChanged(object sender, RoutedEventArgs e)
        {
            if (this.attach == false)
                return;

            try
            {
                var litem = sender as TreeViewCheckBoxItem;

                if (litem != null)
                    SetPermission(litem);
            }
            catch (Exception ex)
            {
                DialogManager.ShowErrorAndLog("Item Check", ex);
            }
        }

        private void PreviewMenusButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.attach == false)
                return;

            try
            {
                foreach (var mitem in this.MenusTreeView.Items.Cast<TreeViewItem>())
                {
                    foreach (var item in mitem.Items.Cast<TreeViewCheckBoxItem>())
                        SetPermission(item);
                }
                BuildMenus();
            }
            catch (Exception ex)
            {
                DialogManager.ShowErrorAndLog("Preview Menus", ex);
            }
        }

        private void BuildMenus()
        {
            ApplicationManager.Current.BuildApplicationMenus(this.GroupMenus.LinkGroups,
                this.ViewModel.UserPermissionViewModels, true);
            this.GroupMenus.SelectedLink = this.GroupMenus.SelectedLinkGroup.Links.FirstOrDefault();
        }

        private void SetPermission(TreeViewCheckBoxItem item)
        {
            Guid id;

            if (Guid.TryParse(item.Tag.ToString(), out id))
                this.ViewModel.SetCanView(id, item.IsChecked == true ? true : false);
        }
    }
}
