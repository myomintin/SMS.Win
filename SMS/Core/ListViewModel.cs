using CodeflexSystems.Library.Core;
using CodeflexSystems.Library.Extensions;
using CodeflexSystems.SmartUI.Windows.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeflexSystems.SMS.Core
{
    internal abstract class ListViewModel<T, TModel> : ListBaseViewModel
        where T : EntityViewModel<TModel>
        where TModel : class
    {
        #region Fields

        private Func<TModel, T> convertTo;

        #endregion

        #region Properties

        public EntityViewModelCollection<T, TModel> Collection { get; private set; }

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

        public bool HasAnyErrors
        {
            get { return this.Collection.HasAnyErrors; }
        }

        #endregion

        #region Ctor, Dtor

        public ListViewModel()
        {
            this.ViewName = string.Format("{0} List",
                System.Text.RegularExpressions.Regex.Replace(typeof(TModel).Name, "(?=[A-Z])", " ").TrimStart());
            this.Collection = new EntityViewModelCollection<T, TModel>();
            this.Collection.SelectionChanged += (s, e) =>
            {
                this.DeleteContent = this.Collection.ToDelete ? "Un_delete" : "_Delete";
            };
        }

        #endregion

        #region Methods

        protected virtual void InitializeCommands()
        {
            try
            {
                this.AddCommand = new RelayCommand(o => OnAdd(), o => this.CanAdd);
                this.EditCommand = new RelayCommand(o => OnEdit(), o => this.Collection.CanEdit);
                this.DeleteCommand = new RelayCommand(o => OnDelete(), o => this.Collection.CanDelete);
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
            this.Collection.Clear();
            this.CanAdd = this.CanRefresh = this.CanSave = false;
        }

        public virtual void Add(T vm)
        {
            this.Collection.Add(vm, true);
            vm.CanSave = false;
            this.CanSave = true;
        }

        public virtual bool Delete(T vm)
        {
            bool flag = this.Collection.Delete(vm);

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
                Delete(this.Collection.SelectedItem);
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
            this.Collection.Where(vm => vm.ObjectState == ObjectState.Deleted && vm.OldObjectState != ObjectState.Added)
                .ForEach(vm => this.UoW.Remove(vm.Model));
            this.Collection.Where(vm => vm.ObjectState == ObjectState.Added)
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
                this.Collection.AcceptAllChanges();
#else
            this.UoW.SaveChanges();
            this.Collection.AcceptAllChanges();
#endif
            this.CanSave = false;
            OnRefreshBinding();
        }

        #endregion
    }
}
