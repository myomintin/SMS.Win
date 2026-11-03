using CodeflexSystems.Library.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeflexSystems.SMS.ViewModels
{
    internal sealed class AddViewModel : BaseViewModel
    {
        #region Fields

        private string existingContent;
        private string newContent;
        private bool isExisting;
        private bool isNew;
        private bool enableExisting;
        private bool enableNew;

        #endregion

        #region Properties

        public string ExistingContent
        {
            get { return this.existingContent; }
            set
            {
                if (this.existingContent != value)
                {
                    this.existingContent = value;
                    RaisePropertyChanged("ExistingContent");
                }
            }
        }

        public string NewContent
        {
            get { return this.newContent; }
            set
            {
                if (this.newContent != value)
                {
                    this.newContent = value;
                    RaisePropertyChanged("NewContent");
                }
            }
        }

        public bool IsExisting
        {
            get { return this.isExisting; }
            set
            {
                if (!this.enableExisting)
                    value = false;
                if (this.isExisting != value)
                {
                    this.isExisting = value;
                    RaisePropertyChanged("IsExisting");
                    if (this.isExisting == this.isNew)
                    {
                        this.isNew = !this.isExisting;
                        RaisePropertyChanged("IsNew");
                    }
                    this.CanSave = this.isExisting || this.isNew;
                }
            }
        }

        public bool IsNew
        {
            get { return this.isNew; }
            set
            {
                if (!this.enableNew)
                    value = false;
                if (this.isNew != value)
                {
                    this.isNew = value;
                    RaisePropertyChanged("IsNew");
                    if (this.isNew == this.isExisting)
                    {
                        this.isExisting = !this.isNew;
                        RaisePropertyChanged("IsExisting");
                    }
                    this.CanSave = this.isExisting || this.isNew;
                }
            }
        }

        public bool EnableExisting
        {
            get { return this.enableExisting; }
            set
            {
                if (this.enableExisting != value)
                {
                    this.enableExisting = value;
                    RaisePropertyChanged("EnableExisting");
                    if (!this.enableExisting && this.isExisting)
                        this.IsExisting = false;
                }
            }
        }

        public bool EnableNew
        {
            get { return this.enableNew; }
            set
            {
                if (this.enableNew != value)
                {
                    this.enableNew = value;
                    RaisePropertyChanged("EnableNew");
                    if (!this.enableNew && this.isNew)
                        this.IsNew = false;
                }
            }
        }

        #endregion

        #region Ctor, Dtor

        public AddViewModel()
        {
            this.enableExisting = this.enableNew = true;
        }

        #endregion

        #region Methods

        public static AddViewModel CreateAddAddress()
        {
            var vm = new AddViewModel();

            vm.PropertyChangedEnabled = false;
            vm.ViewName = "Add Address";
            vm.ExistingContent = "Existing Address";
            vm.NewContent = "New Address";
            vm.PropertyChangedEnabled = true;

            return vm;
        }

        public static AddViewModel CreateAddGuardian()
        {
            var vm = new AddViewModel();

            vm.PropertyChangedEnabled = false;
            vm.ViewName = "Add Guardian";
            vm.ExistingContent = "Existing Guardian";
            vm.NewContent = "New Guardian";
            vm.PropertyChangedEnabled = true;

            return vm;
        }

        public static AddViewModel CreateAddStudent()
        {
            var vm = new AddViewModel();

            vm.PropertyChangedEnabled = false;
            vm.ViewName = "Add Student";
            vm.ExistingContent = "Existing Student";
            vm.NewContent = "New Student";
            vm.PropertyChangedEnabled = true;

            return vm;
        }

        #endregion
    }

    //internal sealed class AddViewModel : BaseViewModel
    //{
    //    #region Fields

    //    private string existingContent;
    //    private string newContent;
    //    private bool isExisting;
    //    private bool isNew;

    //    #endregion

    //    #region Properties

    //    public string ExistingContent
    //    {
    //        get { return this.existingContent; }
    //        set
    //        {
    //            if (this.existingContent != value)
    //            {
    //                this.existingContent = value;
    //                RaisePropertyChanged("ExistingContent");
    //            }
    //        }
    //    }

    //    public string NewContent
    //    {
    //        get { return this.newContent; }
    //        set
    //        {
    //            if (this.newContent != value)
    //            {
    //                this.newContent = value;
    //                RaisePropertyChanged("NewContent");
    //            }
    //        }
    //    }

    //    public bool IsExisting
    //    {
    //        get { return this.isExisting; }
    //        set
    //        {
    //            if (this.isExisting != value)
    //            {
    //                this.isExisting = value;
    //                RaisePropertyChanged("IsExisting");
    //                if (this.isExisting == this.isNew)
    //                {
    //                    this.isNew = !this.isExisting;
    //                    RaisePropertyChanged("IsNew");
    //                }
    //                this.CanSave = this.isExisting || this.isNew;
    //            }
    //        }
    //    }

    //    public bool IsNew
    //    {
    //        get { return this.isNew; }
    //        set
    //        {
    //            if (this.isNew != value)
    //            {
    //                this.isNew = value;
    //                RaisePropertyChanged("IsNew");
    //                if (this.isNew == this.isExisting)
    //                {
    //                    this.isExisting = !this.isNew;
    //                    RaisePropertyChanged("IsExisting");
    //                }
    //                this.CanSave = this.isExisting || this.isNew;
    //            }
    //        }
    //    }

    //    #endregion

    //    #region Methods

    //    public static AddViewModel CreateAddAddress()
    //    {
    //        var vm = new AddViewModel();

    //        vm.PropertyChangedEnabled = false;
    //        vm.ViewName = "Add Address";
    //        vm.ExistingContent = "Existing Address";
    //        vm.NewContent = "New Address";
    //        vm.PropertyChangedEnabled = true;

    //        return vm;
    //    }

    //    public static AddViewModel CreateAddGuardian()
    //    {
    //        var vm = new AddViewModel();

    //        vm.PropertyChangedEnabled = false;
    //        vm.ViewName = "Add Guardian";
    //        vm.ExistingContent = "Existing Guardian";
    //        vm.NewContent = "New Guardian";
    //        vm.PropertyChangedEnabled = true;

    //        return vm;
    //    }

    //    public static AddViewModel CreateAddStudent()
    //    {
    //        var vm = new AddViewModel();

    //        vm.PropertyChangedEnabled = false;
    //        vm.ViewName = "Add Student";
    //        vm.ExistingContent = "Existing Student";
    //        vm.NewContent = "New Student";
    //        vm.PropertyChangedEnabled = true;

    //        return vm;
    //    }

    //    #endregion
    //}
}
