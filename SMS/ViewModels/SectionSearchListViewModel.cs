using CodeflexSystems.Library.Core;
using CodeflexSystems.Library.Extensions;
using CodeflexSystems.SMS.Core;
using CodeflexSystems.SMS.Extensions;
using CodeflexSystems.SMS.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeflexSystems.SMS.ViewModels
{
    internal sealed class SectionSearchListViewModel : SearchListOptionViewModelBDSDT<SectionViewModel, Section, SectionSearchOption>
    {
        #region Ctor, Dtor

        public SectionSearchListViewModel()
        {
            Initialize();
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
                this.SearchOptionInfos.AddRange(SearchOptionHelper.GetSectionSearchOptions()
                    .To(m => SearchOptionHelper.GetSearchInfo(m)), false);
                this.SearchOptionInfos.SelectedIndex = 1;

                this.ConvertTo = (m) => new SectionViewModel(m);
                this.RemoveAdded = true;
                this.IsLoaded = true;
            }
            catch (Exception ex)
            {
                DialogManager.ShowErrorAndLog("Initialize", ex);
            }
        }

        protected override void OnSearch()
        {
            try
            {
                App.CursorWait();
                var current = this.SearchOptionInfos.SelectedItem;
                IQueryable<Section> query = null;

                if (current.SearchValueType == SearchValueType.Boolean)
                {
                    query = GetBooleanQuery<Section>();
                }
                else if (current.SearchValueType == SearchValueType.DateTime)
                {
                    query = GetDateQuery<Section>();
                }
                else if (current.SearchValueType == SearchValueType.Text)
                {
                    if (current.Option == SectionSearchOption.CreatedBy)
                    {
                        query = GetStringQuery<Section, User, Guid?>(m => m.CreatedBy == null,
                            "UserName", m => m.CreatedBy, u => u.UserId, (m, u) => m);
                    }
                    else if (current.Option == SectionSearchOption.ModifiedBy)
                    {
                        query = GetStringQuery<Section, User, Guid?>(m => m.ModifiedBy == null,
                            "UserName", m => m.ModifiedBy, u => u.UserId, (m, u) => m);
                    }
                    else
                    {
                        query = GetStringQuery<Section>();
                    }
                }
                else if (current.SearchValueType == SearchValueType.Decimal)
                {
                    query = GetDecimalQuery<Section>();
                }
                else if (current.SearchValueType == SearchValueType.Time)
                {
                    query = GetTimeQuery<Section>(true);
                }
                else if (current.SearchValueType == SearchValueType.NoneType)
                {
                    if (current.Option == SectionSearchOption.NoUsage)
                    {
                        query = this.UoW.Where<Section>(m => m.StudentSections.Count == 0);
                    }
                }

                this.Collection.Clear();
                if (query != null)
                    this.Collection.AddRange(query.Include().Ordered().To(m => this.ConvertTo(m)), false);
            }
            catch (Exception ex)
            {
                DialogManager.ShowErrorAndLog(this.ViewName, ex);
            }
            finally
            {
                App.CursorDefault();
            }
        }

        public override bool Delete(SectionViewModel vm)
        {
            bool flag = this.Collection.Delete(vm);

            if (flag)
            {
                this.UoW.Remove(vm.Model);
                this.UoW.SaveChanges();
                this.Collection.Remove(vm);
            }

            return flag;
        }

        protected override void OnDelete()
        {
            try
            {
                App.CursorWait();
                Delete(this.Collection.SelectedItem);
            }
            catch (Exception ex)
            {
                DialogManager.ShowErrorAndLog("Delete Data", ex);
            }
            finally
            {
                App.CursorDefault();
            }
        }

        public override void Edit(SectionViewModel vm)
        {
            this.UoW.SaveChanges();
            vm.AcceptChanges();
            OnRefreshBinding();
        }

        protected override void OnEdit()
        {
            try
            {
                bool flag = false;
                var vm = this.Collection.SelectedItem;

                Setup(vm);
                if (NavigationManager.ShowSection(vm).DialogResult == true)
                    Edit(vm);
                else
                {
                    if (vm.HasAnyErrors)
                    {
                        DialogResult result = DialogManager.AskConfirmation("Confirmation",
                            SR.GetString("EditDataAsk3", "section"),
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

        internal override void OnSave()
        { }

        private void Setup(SectionViewModel vm)
        {
            if (vm == null)
                return;

            vm.SetViewName("Section");
            if (vm.IsLoaded)
                return;

            vm.DataChanged += (s, e) =>
            {
                if (e.PropertyName == "SectionName")
                {
                    bool flag = false;
                    var query = this.UoW.Where<Section>(m => vm.Model.ClassId == m.ClassId &&
                        string.Equals(vm.SectionName, m.SectionName, StringComparison.InvariantCultureIgnoreCase)).AsNoTracking();

                    foreach (var item in query)
                    {
                        if (flag = (item.SectionId != vm.Model.SectionId))
                            break;
                    }

                    if (flag)
                    {
                        vm.SetErrors(e.PropertyName, SR.GetString("AlreadyExists1", "Section Name", vm.SectionName));
                        vm.DetectErrors();
                    }
                }
            };
            vm.LoadData(this);
        }

        #endregion
    }
}
