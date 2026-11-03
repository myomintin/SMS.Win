using CodeflexSystems.Library.Core;
using CodeflexSystems.SMS.Data;
using CodeflexSystems.SMS.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeflexSystems.SMS.ViewModels
{
    internal sealed class StudentInfoPopupViewModel : BaseViewModel
    {
        #region Fields

        #endregion

        #region Properties

        public EntityCollection<string> AddressCollection { get; private set; }
        public EntityCollection<string> EmailCollection { get; private set; }
        public EntityCollection<string> PhoneCollection { get; private set; }
        public ObservableCollection<KeyValues<GuardianType, bool>> GuardianTypeCollection { get; private set; }
        public bool ShowAddress { get; set; }
        public bool ShowEmail { get; set; }
        public bool ShowPhone { get; set; }
        public bool ShowGuardian { get; set; }

        public bool AllAddresses
        {
            get
            {
                if (!this.IsLoaded)
                    return false;

                return this.AddressCollection.SelectedIndex == 1;
            }
        }

        public bool AllEmails
        {
            get
            {
                if (!this.IsLoaded)
                    return false;

                return this.EmailCollection.SelectedIndex == 1;
            }
        }

        public bool AllPhones
        {
            get
            {
                if (!this.IsLoaded)
                    return false;

                return this.PhoneCollection.SelectedIndex == 1;
            }
        }

        public bool CheckAnyGuardianTypes
        {
            get
            {
                if (!this.IsLoaded)
                    return false;

                return this.GuardianTypeCollection.Any(m => m.Value1 == true);
            }
        }

        #endregion

        #region Ctor, Dtor

        public StudentInfoPopupViewModel()
        {
            this.AddressCollection = new EntityCollection<string>();
            this.EmailCollection = new EntityCollection<string>();
            this.PhoneCollection = new EntityCollection<string>();
            this.GuardianTypeCollection = new ObservableCollection<KeyValues<GuardianType, bool>>();
            this.ShowAddress = this.ShowEmail = this.ShowPhone = this.ShowGuardian = true;
        }

        #endregion

        #region Methods

        public void Clear()
        {
            this.AddressCollection.Clear();
            this.EmailCollection.Clear();
            this.PhoneCollection.Clear();
            this.GuardianTypeCollection.Clear();
            this.IsLoaded = false;
            this.CanSave = false;
        }

        public void LoadData(ISMSUnitOfWork uow)
        {
            if (this.IsLoaded)
                return;

            this.AddressCollection.AddRange("First or Default", "All");
            this.AddressCollection.SelectedIndex = 1;

            this.EmailCollection.AddRange("First or Default", "All");
            this.EmailCollection.SelectedIndex = 1;

            this.PhoneCollection.AddRange("First or Default", "All");
            this.PhoneCollection.SelectedIndex = 1;

            if (uow != null)
            {
                foreach (var item in uow.GuardianTypes)
                {
                    var value = new KeyValues<GuardianType, bool>(item, true);
                    value.PropertyChanged += (s, e) =>
                    {
                        if (e.PropertyName == "Value1")
                            this.CanSave = this.CheckAnyGuardianTypes;
                    };
                    this.GuardianTypeCollection.Add(value);
                }
                this.IsLoaded = true;
                this.CanSave = this.CheckAnyGuardianTypes;
            }
        }

        public void SetShow(bool showAddress, bool showEmail, bool showPhone, bool showGuardian)
        {
            this.ShowAddress = showAddress;
            this.ShowEmail = showEmail;
            this.ShowPhone = showPhone;
            this.ShowGuardian = showGuardian;
        }

        #endregion
    }
}
