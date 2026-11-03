using CodeflexSystems.Library.DataAnnotations;
using CodeflexSystems.Library.Extensions;
using CodeflexSystems.SMS.Core;
using CodeflexSystems.SMS.Extensions;
using CodeflexSystems.SMS.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeflexSystems.SMS.ViewModels
{
    internal sealed class SectionViewModel : EntityViewModelCM<Section>
    {
        #region Fields

        private Currency currency;
        private DateTime? startDateTime;
        private DateTime? endDateTime;
        private bool addCurrency;

        #endregion

        #region Properties

        public ObservableCollection<Currency> Currencies { get; private set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Section Name is required.")]
        [MaxLength(128, ErrorMessage = "Section Name is too long.")]
        public string SectionName
        {
            get { return this.Model.SectionName; }
            set
            {
                if (this.Model.SectionName != value)
                {
                    ValidateProperty(value, "SectionName");
                    this.Model.SectionName = value;
                    RaiseDataChanged("SectionName");
                }
            }
        }

        [MaxLength(25, ErrorMessage = "Room is too long.")]
        public string Room
        {
            get { return this.Model.Room; }
            set
            {
                Repair(ref value);
                if (this.Model.Room != value)
                {
                    ValidateProperty(value, "Room");
                    this.Model.Room = value;
                    RaiseDataChanged("Room");
                }
            }
        }

        [MaxLength(50, ErrorMessage = "Open Day is too long.")]
        public string OpenDay
        {
            get { return this.Model.OpenDay; }
            set
            {
                Repair(ref value);
                if (this.Model.OpenDay != value)
                {
                    ValidateProperty(value, "OpenDay");
                    this.Model.OpenDay = value;
                    RaiseDataChanged("OpenDay");
                }
            }
        }

        public DateTime? StartDateTime
        {
            get { return this.startDateTime; }
            set
            {
                if (this.startDateTime != value)
                {
                    this.startDateTime = value;
                    //if (this.startDateTime.HasValue)
                    //{
                    //    var dt = this.startDateTime.Value;

                    //    this.Model.StartTime = new TimeSpan(dt.Hour, dt.Minute, dt.Second);
                    //}
                    //else
                    //    this.Model.StartTime = null;
                    this.Model.StartTime = GetTimeSpan(this.startDateTime);
                    RaiseDataChanged("StartTime");
                    RaisePropertyChanged("StartDateTime");
                }
            }
        }

        public DateTime? EndDateTime
        {
            get { return this.endDateTime; }
            set
            {
                if (this.endDateTime != value)
                {
                    this.endDateTime = value;
                    this.Model.EndTime = GetTimeSpan(this.endDateTime);
                    RaiseDataChanged("EndTime");
                    RaisePropertyChanged("EndDateTime");
                }
            }
        }

        public DateTime? CommencingDate
        {
            get { return this.Model.CommencingDate; }
            set
            {
                if (this.Model.CommencingDate != value)
                {
                    this.Model.CommencingDate = value;
                    RaiseDataChanged("CommencingDate");
                }
            }
        }

        public DateTime? CompletionDate
        {
            get { return this.Model.CompletionDate; }
            set
            {
                if (this.Model.CompletionDate != value)
                {
                    this.Model.CompletionDate = value;
                    RaiseDataChanged("CompletionDate");
                }
            }
        }

        [MinValue(0, ErrorMessage = "Fee must be at least 0.")]
        public decimal? Fee
        {
            get { return this.Model.Fee; }
            set
            {
                RepairCurrency(ref value);
                if (this.Model.Fee != value)
                {
                    ValidateProperty(value, "Fee");
                    this.Model.Fee = value;
                    RaiseDataChanged("Fee");
                }
            }
        }

        public bool Monthly
        {
            get { return this.Model.Monthly; }
            set
            {
                if (this.Model.Monthly != value)
                {
                    this.Model.Monthly = value;
                    RaiseDataChanged("Monthly");
                }
            }
        }

        [MaxLength(250, ErrorMessage = "Description is too long.")]
        public string Description
        {
            get { return this.Model.Description; }
            set
            {
                Repair(ref value);
                if (this.Model.Description != value)
                {
                    ValidateProperty(value, "Description");
                    this.Model.Description = value;
                    RaiseDataChanged("Description");
                }
            }
        }

        public Currency Currency
        {
            get { return this.currency; }
            set
            {
                if (this.currency != value)
                {
                    this.currency = value;
                    if (this.currency != null && !IsDummy(this.currency.CurrencyId))
                        this.Model.Currency = this.currency;
                    else
                        this.Model.Currency = null;
                    RaiseDataChanged("Currency");
                }
            }
        }

        public DateTime CreatedDate
        {
            get { return this.Model.CreatedDate; }
        }

        public DateTime ModifiedDate
        {
            get { return this.Model.ModifiedDate; }
        }

        public DateTime CreatedDateLT
        {
            get { return this.Model.CreatedDate.ToLocalTime(); }
        }

        public DateTime ModifiedDateLT
        {
            get { return this.Model.ModifiedDate.ToLocalTime(); }
        }

        public override bool CanDelete
        {
            get
            {
                if (this.IsNew || this.DetectCanDelete == null)
                    return this.Model.CanDelete;

                return this.DetectCanDelete();
            }
        }

        public override bool IsNew
        {
            get { return IsDummy(this.Model.SectionId); }
        }

        public Class Class
        {
            get { return this.Model.Class; }
            set
            {
                if (this.Model.Class != value)
                {
                    this.Model.Class = value;
                    RaiseDataChanged("Class");
                }
            }
        }

        public Course Course
        {
            get
            {
                var value = this.Class;

                if (value == null)
                    return null;
                return value.Course;
            }
        }

        public AcademicYear AcademicYear
        {
            get
            {
                var value = this.Course;

                if (value == null)
                    return null;
                return value.AcademicYear;
            }
        }

        public School School
        {
            get
            {
                var value = this.AcademicYear;

                if (value == null)
                    return null;
                return value.School;
            }
        }

        //public Course Course
        //{
        //    get { return this.Class.Course; }
        //}

        //public AcademicYear AcademicYear
        //{
        //    get { return this.Course.AcademicYear; }
        //}

        //public School School
        //{
        //    get { return this.AcademicYear.School; }
        //}

        #endregion

        #region Ctor, Dtor

        public SectionViewModel(Section model)
            : base(model)
        {
            if (this.Model.StartTime.HasValue)
                this.startDateTime = new DateTime(this.Model.StartTime.Value.Ticks);
            if (this.Model.EndTime.HasValue)
                this.endDateTime = new DateTime(this.Model.EndTime.Value.Ticks);
        }

        #endregion

        #region Methods

        public override void AcceptChanges()
        {
            if (!this.IsLoaded)
                return;

            SetCreatedBy();
            SetModifiedBy();
            BaseAcceptChanges();
            if (this.addCurrency && this.Model.Currency != null)
            {
                Guid id = this.Model.Currency.CurrencyId;
                if (this.Parent.UoW.Currencies.Any(m => m.CurrencyId == id))
                {
                    this.Currencies = this.Parent.UoW.Currencies.WithDummy();
                    this.addCurrency = false;
                }
            }
        }

        protected override void SetCreatedBy()
        {
            if (string.IsNullOrEmpty(this.CreatedBy))
                this.CreatedBy = this.Parent.UoW.GetUserNameById(this.Model.CreatedBy);
        }

        protected override void SetModifiedBy()
        {
            this.ModifiedBy = this.Parent.UoW.GetUserNameById(this.Model.ModifiedBy);
        }

        public void DetectErrors()
        {
            if (!this.IsLoaded)
                return;

            ValidateRequiredProperties();
            this.CanSave = !this.HasAnyErrors;
        }

        public void ValidateRequiredProperties()
        {
            if (string.IsNullOrEmpty(this.SectionName))
                ValidateProperty(this.SectionName, "SectionName");
        }

        public void LoadData(IParent parent)
        {
            if (this.IsLoaded)
                return;

            this.Parent = parent;
            SetCreatedBy();
            SetModifiedBy();
            this.Currencies = this.Parent.UoW.Currencies.WithDummy();
            this.currency = this.Model.Currency;
            if (this.currency == null)
                this.currency = this.Currencies.FirstOrDefault();
            else if (!this.IsNew)
                this.addCurrency = this.Parent.UoW.AddCurrencyById(this.Currencies, this.currency.CurrencyId);
            //this.Currencies = this.Parent.UoW.Currencies.WithDummy();
            //this.currency = this.Model.Currency;
            //if (this.currency == null)
            //    this.currency = this.Currencies.GetDefault();
            this.DataChanged += (s, e) =>
            {
                DetectErrors();
            };
            this.IsLoaded = true;
        }

        //private void RepairTimeOfDay(ref TimeSpan? value)
        //{
        //    if (value.HasValue && value.Value.TotalDays >= 1)
        //        value = new DateTime(value.Value.Ticks).TimeOfDay;
        //}

        #endregion
    }
}
