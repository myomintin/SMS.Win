using CodeflexSystems.Library.Core;
using CodeflexSystems.SMS.Core;
using CodeflexSystems.SMS.Extensions;
using CodeflexSystems.SMS.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeflexSystems.SMS.ViewModels
{
    internal sealed class PersonGuardianViewModel : EntityViewModelCM<PersonGuardian>
    {
        # region Fields

        private GuardianViewModel guardianViewModel;

        # endregion

        # region Properties

        public ObservableCollection<GuardianType> GuardianTypes { get; private set; }

        public GuardianViewModel GuardianViewModel
        {
            get { return this.guardianViewModel; }
            //internal set
            //{
            //    if (this.guardianViewModel != value)
            //    {
            //        this.guardianViewModel = value;
            //        if (this.guardianViewModel.Model != this.Model.Guardian)
            //            this.Model.Guardian = this.guardianViewModel.Model;
            //        RaiseDataChanged("GuardianViewModel");
            //    }
            //}
        }

        public GuardianType GuardianType
        {
            get { return this.Model.GuardianType; }
            set
            {
                if (this.Model.GuardianType != value)
                {
                    this.Model.GuardianType = value;
                    RaiseDataChanged("GuardianType");
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
                if (this.IsLoaded || this.IsNew || this.DetectCanDelete == null)
                    return this.Model.CanDelete;

                return this.DetectCanDelete();
            }
        }

        public override bool IsNew
        {
            get { return IsDummy(this.Model.CreatedDate); }
        }

        public override bool HasAnyErrors
        {
            get
            {
                if (!this.IsLoaded)
                    return this.HasErrors;

                return this.HasErrors || this.guardianViewModel.HasAnyErrors;
            }
        }

        # endregion

        # region Ctor, Dtor

        public PersonGuardianViewModel(PersonGuardian model, IParent parent)
            : base(model, parent)
        {
            if (this.Model.Guardian == null)
                this.Model.Guardian = new Guardian() { Gender = true };
            this.guardianViewModel = new GuardianViewModel(this.Model.Guardian, this.Parent);
        }

        # endregion

        # region Methods

        public override void AcceptChanges()
        {
            if (!this.IsLoaded)
                return;

            this.guardianViewModel.AcceptChanges();
            SetCreatedBy();
            SetModifiedBy();
            base.BaseAcceptChanges();
        }

        public override void Refresh()
        {
            if (!this.IsLoaded || this.OldObjectState == ObjectState.Added || this.Parent == null)
                return;

            this.Parent.UoW.RefreshToOriginalValues(this.Model);
            this.guardianViewModel.Refresh();
            base.BaseRefresh();
            this.OverallState = this.guardianViewModel.OverallState;
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

            this.guardianViewModel.ValidateRequiredProperties();
            this.CanSave = !this.HasAnyErrors;
        }

        public void LoadData()
        {
            if (this.IsLoaded)
                return;

            SetCreatedBy();
            SetModifiedBy();
            this.guardianViewModel.LoadData();
            this.GuardianTypes = this.Parent.UoW.GuardianTypes;
            if (this.Model.GuardianType == null)
                this.Model.GuardianType = this.GuardianTypes.GetDefault(this.guardianViewModel.Gender);

            this.guardianViewModel.DataChanged += (s, e) =>
            {
                RaiseDataChanged(null);
            };
            this.DataChanged += (s, e) =>
            {
                DetectErrors();
            };
            this.IsLoaded = true;
        }

        # endregion
    }
}
