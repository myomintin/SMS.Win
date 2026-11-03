using CodeflexSystems.Library.Core;
using CodeflexSystems.SMS.Data;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CodeflexSystems.SMS.Core
{
    internal abstract class ListBaseViewModel : BaseViewModel, IParent
    {
        #region Fields

        private string deleteContent;

        #endregion

        #region Properties

        public ISMSUnitOfWork UoW { get; protected set; }
        public bool RemoveAdded { get; protected set; }
        public bool Refreshing { get; protected set; }
        public bool CanAdd { get; protected set; }
        public bool CanRefresh { get; protected set; }

        public bool IncludeFilterTypes { get; protected set; }
        public bool IncludeFilterStates { get; protected set; }
        public Type[] FilterTypes { get; protected set; }
        public EntityState[] FilterStates { get; protected set; }
        public SaveMode SaveMode { get; protected set; }

        public ICommand AddCommand { get; protected set; }
        public ICommand EditCommand { get; protected set; }
        public ICommand DeleteCommand { get; protected set; }
        public ICommand RefreshCommand { get; protected set; }
        public ICommand SaveCommand { get; protected set; }
        public ICommand RefreshBindingCommand { get; internal set; }

        public string DeleteContent
        {
            get { return this.deleteContent; }
            protected set
            {
                if (this.deleteContent != value)
                {
                    this.deleteContent = value;
                    RaisePropertyChanged("DeleteContent");
                }
            }
        }

        #endregion

        #region Ctor, Dtor

        public ListBaseViewModel()
        {
            this.UoW = ApplicationManager.Resolve<ISMSUnitOfWork>();
            this.deleteContent = "_Delete";
            this.RemoveAdded = false;
            this.Refreshing = false;
            this.IncludeFilterTypes = true;
            this.IncludeFilterStates = true;
            this.FilterStates = new EntityState[] { EntityState.Added, EntityState.Deleted, EntityState.Modified };
            this.SaveMode = SaveMode.SaveChanges | SaveMode.AcceptChanges;
        }

        #endregion

        #region Methods

        protected abstract void OnAdd();
        protected abstract void OnEdit();
        protected abstract void OnDelete();
        protected abstract void OnRefresh();
        internal abstract void OnSave();

        protected virtual void OnRefreshBinding()
        {
            if (this.RefreshBindingCommand != null)
                this.RefreshBindingCommand.Execute(null);
        }

        [Conditional("DEBUG")]
        protected void PrintChanges()
        {
            Console.WriteLine("Changes");
            Console.WriteLine(this.UoW.GetStringFromChangeTracker(this.IncludeFilterTypes, this.FilterTypes, this.IncludeFilterStates, this.FilterStates));
            Console.WriteLine();
        }

        [Conditional("DEBUG")]
        protected void ShowChanges()
        {
            DialogManager.Show("Changes", this.UoW.GetStringFromChangeTracker(this.IncludeFilterTypes, this.FilterTypes, this.IncludeFilterStates, this.FilterStates));
        }

        #endregion
    }
}
