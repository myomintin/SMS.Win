using CodeflexSystems.Library.Core;
using CodeflexSystems.Library.Extensions;
using CodeflexSystems.SmartUI.Windows.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeflexSystems.SMS.Core
{
    internal abstract class ListViewModelPaging<T, TModel> : ListBaseViewModel
        where T : EntityViewModel<TModel>
        where TModel : class
    {
        #region Fields

        private Func<TModel, T> convertTo;

        #endregion

        #region Properties

        public EntityViewModelPagingCollection<T, TModel> Paging { get; private set; }

        public Func<TModel, T> ConvertTo
        {
            get { return this.convertTo; }
            set
            {
                if (value == null)
                    this.convertTo = null;
                else
                    this.convertTo = value.GetInvocationList().LastOrDefault() as Func<TModel, T>;
            }
        }

        public ObservableCollection<T> Collection
        {
            get { return this.Paging.Collection; }
        }

        public bool HasAnyErrors
        {
            get { return this.Paging.HasAnyErrors; }
        }

        #endregion

        #region Ctor, Dtor

        public ListViewModelPaging(int pageSize = 0)
        {
            this.ViewName = string.Format("{0} List",
                System.Text.RegularExpressions.Regex.Replace(typeof(TModel).Name, "(?=[A-Z])", " ").TrimStart());
            if (pageSize < ApplicationManager.Current.Options.PageSize)
                pageSize = ApplicationManager.Current.Options.PageSize;
            this.Paging = new EntityViewModelPagingCollection<T, TModel>(pageSize);
            this.Paging.SelectionChanged += (s, e) =>
            {
                this.DeleteContent = this.Paging.ToDelete ? "Un_delete" : "_Delete";
            };
        }

        #endregion

        #region Methods

        protected virtual void InitializeCommands()
        {
            try
            {
                this.AddCommand = new RelayCommand(o => OnAdd(), o => this.CanAdd);
                this.EditCommand = new RelayCommand(o => OnEdit(), o => this.Paging.CanEdit);
                this.DeleteCommand = new RelayCommand(o => OnDelete(), o => this.Paging.CanDelete);
                this.RefreshCommand = new RelayCommand(o => OnRefresh(), o => this.CanRefresh);
                this.SaveCommand = new RelayCommand(o => OnSave(), o => this.CanSave);
            }
            catch (Exception ex)
            {
                DialogManager.ShowErrorAndLog("Initialize Commands", ex);
            }
        }

        public virtual void Clear()
        {
            this.Paging.Clear();
            this.CanAdd = this.CanRefresh = this.CanSave = false;
        }

        public virtual void Add(T vm)
        {
            this.Paging.Add(vm, true);
            vm.CanSave = false;
            this.CanSave = true;
        }

        public virtual bool Delete(T vm)
        {
            bool flag = this.Paging.Delete(vm);

            if (flag)
            {
                vm.CanSave = false;
                this.CanSave = true;
            }

            return flag;
        }

        protected override void OnDelete()
        {
            try
            {
                Delete(this.Paging.SelectedItem);
            }
            catch (Exception ex)
            {
                DialogManager.ShowErrorAndLog("Delete Data", ex);
            }
        }

        public virtual void Edit(T vm)
        {
            vm.CanSave = false;
            this.CanSave = true;
        }

        protected virtual void DetectChanges()
        {
            this.Paging.Where(vm => vm.ObjectState == ObjectState.Deleted && vm.OldObjectState != ObjectState.Added)
                .ForEach(vm => this.UoW.Remove(vm.Model));
            this.Paging.Where(vm => vm.ObjectState == ObjectState.Added)
                .ForEach(vm => this.UoW.Add(vm.Model));
        }

        public virtual void SaveChanges()
        {
            DetectChanges();
#if DEBUG
            if ((this.SaveMode & SaveMode.PrintConsole) == SaveMode.PrintConsole)
                PrintChanges();
            if ((this.SaveMode & SaveMode.PrintUI) == SaveMode.PrintUI)
                ShowChanges();
            if ((this.SaveMode & SaveMode.SaveChanges) == SaveMode.SaveChanges)
                this.UoW.SaveChanges();
            if ((this.SaveMode & SaveMode.AcceptChanges) == SaveMode.AcceptChanges)
                this.Paging.AcceptAllChanges();
#else
            this.UoW.SaveChanges();
            this.Paging.AcceptAllChanges();
#endif
            this.CanSave = false;
            OnRefreshBinding();
        }

        #endregion
    }
}
