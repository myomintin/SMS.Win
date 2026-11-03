using CodeflexSystems.Library.Core;
using CodeflexSystems.Library.Extensions;
using CodeflexSystems.SMS.Core;
using CodeflexSystems.SMS.Data;
using CodeflexSystems.SMS.Extensions;
using CodeflexSystems.SMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeflexSystems.SMS.ViewModels
{
    internal class ReligionListViewModel : ListViewModel<ReligionViewModel, Religion>
    {
        #region Fields

        private List<string> defaultReligionTypes;

        #endregion

        #region Ctor, Dtor

        public ReligionListViewModel()
        {
            Initialize();
            LoadData();
            InitializeCommands();
        }

        #endregion

        #region Methods

        protected override void OnDispose()
        {
            base.OnDispose();
            DisposeUoW();
        }

        private void DisposeUoW()
        {
            if (this.UoW != null)
                this.UoW.Dispose();
        }

        internal void ClearAllCollections()
        {
            Clear();
            this.UoW.Clear();
        }

        private void Initialize()
        {
            try
            {
                this.IsLoaded = false;
                this.defaultReligionTypes = SR.GetReligions();
                this.ConvertTo = (m) =>
                {
                    var vm = new ReligionViewModel(m);
                    bool flag = this.defaultReligionTypes != null && this.defaultReligionTypes.Contains(m.ReligionName);

                    vm.DetectCanDelete = () =>
                    {
                        if (flag)
                            return false;

                        return this.UoW.FirstAsNoTracking<Religion>
                            (model => model.ReligionId == vm.Model.ReligionId).CanDelete;
                    };
                    vm.DetectCanEdit = () =>
                    {
                        if (flag)
                            return false;

                        return vm.Model != null;
                    };

                    return vm;
                };
                this.IsLoaded = true;
            }
            catch (Exception ex)
            {
                DialogManager.ShowErrorAndLog("Initialize", ex);
            }
        }

        private void LoadData()
        {
            try
            {
                this.Collection.AddRange(this.UoW.Religions
                    .To(m => this.ConvertTo(m)), false);
                this.CanAdd = this.CanRefresh = true;
                if (!this.Refreshing)
                    this.Collection.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                DialogManager.ShowErrorAndLog("Load Data", ex);
            }
        }

        protected override void OnAdd()
        {
            try
            {
                var vm = new ReligionViewModel(new Religion());

                Setup(vm);
                if (NavigationManager.ShowReligion(vm).DialogResult == true)
                {
                    Add(vm);
                }
            }
            catch (Exception ex)
            {
                DialogManager.ShowErrorAndLog("Add New", ex);
            }
        }

        protected override void OnEdit()
        {
            try
            {
                bool flag = false;
                var vm = this.Collection.SelectedItem;

                Setup(vm);
                if (NavigationManager.ShowReligion(vm).DialogResult == true)
                {
                    Edit(vm);
                }
                else
                {
                    if (vm.OldObjectState == ObjectState.Added)
                    {
                        if (vm.HasAnyErrors)
                        {
                            DialogResult result = DialogManager.AskConfirmation("Confirmation",
                                SR.GetString("EditDataAsk1", "religion"),
                                SR.GetString("EditDataAsk2"), DialogButton.YesNo);
                            if (result == DialogResult.Yes)
                                this.Collection.Remove(vm);
                            else
                            {
                                OnEdit();
                                return;
                            }
                        }
                    }
                    else
                    {
                        if (vm.HasAnyErrors)
                        {
                            DialogResult result = DialogManager.AskConfirmation("Confirmation",
                                SR.GetString("EditDataAsk3", "religion"),
                                SR.GetString("EditDataAsk4"), DialogButton.YesNo);
                            if (result == DialogResult.Yes)
                                flag = true;
                            else
                            {
                                OnEdit();
                                return;
                            }
                        }
                        else if (vm.CanSave)
                            flag = true;
                    }
                }

                if (flag)
                {
                    vm.Refresh();
                    OnRefreshBinding();
                }
            }
            catch (Exception ex)
            {
                DialogManager.ShowErrorAndLog("Edit Data", ex);
            }
        }

        protected override void OnRefresh()
        {
            try
            {
                if (this.CanSave)
                {
                    DialogResult result = DialogManager.AskConfirmation("Confirmation",
                        SR.GetString("RefreshDataAsk1"), SR.GetString("RefreshDataAsk2"), DialogButton.OKCancel);
                    if (result == DialogResult.Cancel)
                        return;
                }

                int index = this.Collection.SelectedIndex;

                App.CursorWait();
                ClearAllCollections();
                DisposeUoW();
                this.UoW = ApplicationManager.Resolve<ISMSUnitOfWork>();

                this.Refreshing = true;
                LoadData();
                this.Collection.SelectedIndex = index;
                this.Refreshing = false;
                //OnRefreshBinding();
            }
            catch (Exception ex)
            {
                DialogManager.ShowErrorAndLog("Refresh Data", ex);
            }
            finally
            {
                App.CursorDefault();
            }
        }

        internal override void OnSave()
        {
            try
            {
                App.CursorWait();
                if (this.HasAnyErrors)
                {
                    this.CanSave = false;
                    DialogManager.ShowError("Validate Data", SR.GetString("ValidateData1", "religion"));
                }
                if (this.CanSave)
                    SaveChanges();
            }
            catch (Exception ex)
            {
                DialogManager.ShowErrorAndLog("Save Data", ex);
            }
            finally
            {
                App.CursorDefault();
            }
        }

        private void Setup(ReligionViewModel vm)
        {
            if (vm == null)
                return;

            vm.SetViewName("Religion");
            if (vm.IsLoaded)
                return;

            vm.DataChanged += (s, e) =>
            {
                if (e.PropertyName == "ReligionName")
                {
                    bool flag = false;

                    foreach (var item in this.Collection.Where(m =>
                        vm.ReligionName.Equals(m.ReligionName, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        if (flag = (item != vm))
                            break;
                    }

                    if (flag)
                    {
                        vm.SetErrors(e.PropertyName, SR.GetString("AlreadyExists1", "Religion Name", vm.ReligionName));
                        vm.DetectErrors();
                    }
                }
            };
            vm.LoadData(this);
        }

        #endregion
    }
}
