using CodeflexSystems.Library.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeflexSystems.SMS.ViewModels
{
    internal sealed class DueDatePopupViewModel : BaseViewModel
    {
        #region Fields

        private DateTime dueDate;

        #endregion

        #region Properties

        public DateTime DueDate
        {
            get { return this.dueDate; }
            set
            {
                if (this.dueDate != value)
                {
                    this.dueDate = value;
                    RaisePropertyChanged("DueDate");
                }
            }
        }

        #endregion

        #region Ctor, Dtor

        public DueDatePopupViewModel()
        {
            this.dueDate = DateTime.Today;
            this.CanSave = true;
        }

        #endregion

        #region Methods

        #endregion
    }
}
