using CodeflexSystems.Library.Core;
using CodeflexSystems.SMS.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CodeflexSystems.SMS.Core
{
    internal abstract class SearchListBaseViewModel : BaseViewModel, IParent
    {
        #region Fields

        private string deleteContent;

        #endregion

        #region Properties

        public ISMSUnitOfWork UoW { get; protected set; }
        public bool RemoveAdded { get; protected set; }
        public bool CanSearch { get; internal set; }

        public ICommand SearchCommand { get; protected set; }
        public ICommand EditCommand { get; protected set; }
        public ICommand DeleteCommand { get; protected set; }
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

        public SearchListBaseViewModel()
        {
            this.UoW = ApplicationManager.Resolve<ISMSUnitOfWork>();
            this.deleteContent = "_Delete";
            this.RemoveAdded = false;
        }

        #endregion
        
        #region Methods

        protected abstract void OnSearch();
        protected abstract void OnEdit();
        protected abstract void OnDelete();
        internal abstract void OnSave();

        protected virtual void OnRefreshBinding()
        {
            if (this.RefreshBindingCommand != null)
                this.RefreshBindingCommand.Execute(null);
        }

        #endregion
    }
}
