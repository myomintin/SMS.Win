using CodeflexSystems.Library.Core;
using CodeflexSystems.SmartUI.Windows.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeflexSystems.SMS.Core
{
    internal abstract class SearchListViewModel<T, TModel> : SearchListBaseViewModel
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

        public SearchListViewModel()
        {
            this.ViewName = string.Format("{0} List",
                System.Text.RegularExpressions.Regex.Replace(typeof(TModel).Name, "(?=[A-Z])", " ").TrimStart());
            this.Collection = new EntityViewModelCollection<T, TModel>();
            this.Collection.SelectionChanged += (s, e) =>
            {
                this.DeleteContent = this.Collection.ToDelete ? "Un_delete" : "_Delete";
            };
            this.CanSearch = true;
        }

        #endregion

        #region Methods

        protected virtual void InitializeCommands()
        {
            try
            {
                this.SearchCommand = new RelayCommand(o => OnSearch(), o => this.CanSearch);
                this.EditCommand = new RelayCommand(o => OnEdit(), o => this.Collection.CanEdit);
                this.DeleteCommand = new RelayCommand(o => OnDelete(), o => this.Collection.CanDelete);
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
            this.CanSave = false;
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

        #endregion
    }
}
