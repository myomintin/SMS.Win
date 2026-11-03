using CodeflexSystems.Library.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeflexSystems.SMS.ViewModels
{
    internal sealed class ReminderPopupViewModel : BaseViewModel
    {
        #region Fields

        private DateTime dueDate;
        private DateTime paymentMonth;

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

        public DateTime PaymentMonth
        {
            get { return this.paymentMonth; }
            set
            {
                RepairDay(ref value, 1);
                if (this.paymentMonth != value)
                {
                    this.paymentMonth = value;
                    RaisePropertyChanged("PaymentMonth");
                }
            }
        }

        #endregion

        #region Ctor, Dtor

        public ReminderPopupViewModel()
        {
            this.dueDate = DateTime.Today;
            this.paymentMonth = this.dueDate.AddMonths(1);
            RepairDay(ref this.paymentMonth, 1);
            this.CanSave = true;
        }

        #endregion

        #region Methods

        #endregion
    }
}
