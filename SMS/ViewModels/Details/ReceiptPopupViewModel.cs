using CodeflexSystems.Library.Core;
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
    internal sealed class ReceiptPopupViewModel : BaseViewModel
    {
        #region Fields

        private DateTime receiptDate;
        private DateTime toDate;

        #endregion

        #region Properties

        public EntityCollection<Currency> Currencies { get; private set; }
        public EntityCollection<School> Schools { get; private set; }
        public string DateString { get; set; }
        public string FormatString { get; set; }
        public bool ShowToDate { get; set; }

        public DateTime ReceiptDate
        {
            get { return this.receiptDate; }
            set
            {
                if (this.receiptDate != value)
                {
                    this.receiptDate = value;
                    RaisePropertyChanged("ReceiptDate");
                    if (this.receiptDate > this.toDate)
                        this.ToDate = this.receiptDate.AddMonths(1);
                }
            }
        }

        public DateTime ToDate
        {
            get { return this.toDate; }
            set
            {
                if (this.toDate != value)
                {
                    this.toDate = value;
                    RaisePropertyChanged("ToDate");
                }
            }
        }

        #endregion

        #region Ctor, Dtor

        public ReceiptPopupViewModel()
        {
            this.Currencies = new EntityCollection<Currency>();
            this.Schools = new EntityCollection<School>();
            this.receiptDate = DateTime.Today;
            this.toDate = this.receiptDate.AddMonths(1);
            Setup(0);
        }

        #endregion

        #region Methods

        public void LoadData(ISMSUnitOfWork uow)
        {
            if (this.IsLoaded)
                return;

            if (uow != null)
            {
                this.Currencies.AddRange(uow.CurrenciesAsNoTracking.WithDummy("<All>"), false);
                this.Currencies.SelectedIndex = 0;
                this.Schools.AddRange(uow.SchoolsAsNoTracking.WithDummy("<All>"), false);
                this.Schools.SelectedIndex = 0;
                this.IsLoaded = true;
                this.CanSave = true;
            }
        }

        public void Clear()
        {
            this.Currencies.Clear();
            this.Schools.Clear();
            this.IsLoaded = false;
            this.CanSave = false;
        }

        public void Setup(byte type)
        {
            if (type == 1)
            {
                this.DateString = "_Month For";
                this.FormatString = "MMMM yyyy";
                this.ShowToDate = false;
            }
            else if (type == 2)
            {
                this.DateString = "From _Month";
                this.FormatString = "MMMM yyyy";
                this.ShowToDate = true;
            }
            else
            {
                this.DateString = "Receipt _Date";
                if (ApplicationManager.Current.Options == null)
                    this.FormatString = "d/M/yyyy";
                else
                    this.FormatString = ApplicationManager.Current.Options.DatePattern;
                this.ShowToDate = false;
            }
        }

        #endregion
    }
}
