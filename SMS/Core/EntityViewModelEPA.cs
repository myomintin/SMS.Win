using CodeflexSystems.Library.Core;
using CodeflexSystems.Library.Extensions;
using CodeflexSystems.SmartUI.Windows.Input;
using CodeflexSystems.SMS.Models;
using CodeflexSystems.SMS.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CodeflexSystems.SMS.Core
{
    /// <summary>
    /// The implementation of the generic view model with Addresses, Emails and Phones for entity.
    /// </summary>
    /// <typeparam name="T">entity</typeparam>
    //[Serializable]
    internal abstract class EntityViewModelEPA<T> : EntityViewModelEP<T>, IAddressViewModel
        where T : class
    {
        #region Fields

        private string deleteAddressContent;

        #endregion

        #region Properties

        public EntityViewModelCollection<PersonAddressViewModel, PersonAddress> AddressViewModels { get; private set; }
        public Action<PersonAddressViewModel> AddressPropertiesChanged { get; set; }

        public ICommand AddAddressCommand { get; protected set; }
        public ICommand EditAddressCommand { get; protected set; }
        public ICommand DeleteAddressCommand { get; protected set; }
        public ICommand RefreshAddressBindingCommand { get; set; }

        public IAddressParent AddressParent
        {
            get { return this.Parent as IAddressParent; }
        }

        public string DeleteAddressContent
        {
            get { return this.deleteAddressContent; }
            protected set
            {
                if (this.deleteAddressContent != value)
                {
                    this.deleteAddressContent = value;
                    RaisePropertyChanged("DeleteAddressContent");
                }
            }
        }

        #endregion

        #region Ctor, Dtor

        public EntityViewModelEPA(T model, IParent parent)
            : base(model, parent)
        {
            this.deleteAddressContent = "_Delete";
        }

        #endregion

        #region Methods

        protected abstract PersonAddressViewModel CreateAddressViewModel(Address model);

        protected override void InitializeCommands()
        {
            if (this.IsLoaded)
                return;

            base.InitializeCommands();
            this.AddAddressCommand = new RelayCommand<object>(o => OnAddAddress(o));
            this.EditAddressCommand = new RelayCommand<object>(o => OnEditAddress(o), o => this.AddressViewModels.CanEdit);
            this.DeleteAddressCommand = new RelayCommand(o => OnDeleteAddress(), o => this.AddressViewModels.CanDelete);
        }

        protected virtual void InitializeAddresses()
        {
            if (this.AddressViewModels == null)
            {
                this.AddressViewModels = new EntityViewModelCollection<PersonAddressViewModel, PersonAddress>();
                this.AddressViewModels.SelectionChanged += (s, e) =>
                {
                    this.DeleteAddressContent = this.AddressViewModels.ToDelete ? "Un_delete" : "_Delete";
                };
            }
            else
                this.AddressViewModels.Clear();
        }

        public virtual void Add(PersonAddressViewModel vm)
        {
            if (!vm.AddressViewModel.IsNew)
            {
                Guid id = vm.Model.Address.AddressId;
                if (this.AddressViewModels.Any(m => id == m.Model.Address.AddressId))
                    throw new AlreadyExistsException(SR.GetString("AlreadyExists2", vm.Model, "Address"));
            }

            this.AddressViewModels.Add(vm, true);
            SetDefault(vm);
            vm.CanSave = false;
            RaiseRelatedDataChanged(null);
        }

        protected virtual void OnAddAddress(object o)
        {
            try
            {
                var avm = this.AddressParent.AddAddressViewModel;

                if (NavigationManager.ShowAdd(avm, o).DialogResult == false)
                    return;

                if (avm.IsNew)
                {
                    var vm = CreateAddressViewModel(new Address());

                    Setup(vm);
                    if (this.AddressViewModels.Count == 0)
                        vm.Model.IsDefault = true;
                    if (NavigationManager.ShowPersonAddress(vm, o).DialogResult == true)
                    {
                        Add(vm);
                    }
                }
                else
                {
                    var asvm = this.AddressParent.AddressSelector;

                    if (NavigationManager.ShowSearchAddress(asvm, o).DialogResult == true &&
                        asvm.Collection.SelectedIndex != -1)
                    {
                        var value = asvm.Collection.SelectedItem;
                        var model = this.Parent.UoW.FirstOrDefault<Address>(m => m.AddressId == value.AddressId);

                        if (model != null)
                        {
                            var vm = CreateAddressViewModel(model);

                            Setup(vm);
                            vm.DetectErrors();
                            if (NavigationManager.ShowPersonAddress(vm, o).DialogResult == true)
                            {
                                Add(vm);
                            }
                        }
                    }
                }
            }
            catch (AlreadyExistsException ex)
            {
                DialogManager.ShowError("Add Address", ex);
            }
            catch (Exception ex)
            {
                DialogManager.ShowErrorAndLog("Add Address", ex);
            }
        }

        public virtual bool Delete(PersonAddressViewModel vm)
        {
            bool flag = this.AddressViewModels.Delete(vm);

            if (flag)
            {
                vm.CanSave = false;
                RaiseRelatedDataChanged(null);
            }

            return flag;
        }

        protected virtual void OnDeleteAddress()
        {
            try
            {
                Delete(this.AddressViewModels.SelectedItem);
            }
            catch (Exception ex)
            {
                DialogManager.ShowErrorAndLog("Delete Address", ex);
            }
        }

        public virtual void Edit(PersonAddressViewModel vm)
        {
            SetDefault(vm);
            vm.CanSave = false;
            RaiseRelatedDataChanged(null);
        }

        protected virtual void OnEditAddress(object o)
        {
            try
            {
                bool flag = false;
                var vm = this.AddressViewModels.SelectedItem;

                Setup(vm);
                if (NavigationManager.ShowPersonAddress(vm, o).DialogResult == true)
                {
                    Edit(vm);
                }
                else
                {
                    if (vm.OldObjectState == ObjectState.Added)
                    {
                        if (vm.HasAnyErrors)
                        {
                            DialogResult result = DialogManager.AskConfirmation("Confirmation",
                                SR.GetString("EditDataAsk1", "address"),
                                SR.GetString("EditDataAsk2"), DialogButton.YesNo);
                            if (result == DialogResult.Yes)
                                this.AddressViewModels.Remove(vm);
                            else
                            {
                                OnEditAddress(o);
                                return;
                            }
                        }
                    }
                    else
                    {
                        if (vm.HasAnyErrors)
                        {
                            DialogResult result = DialogManager.AskConfirmation("Confirmation",
                                SR.GetString("EditDataAsk3", "address"),
                                SR.GetString("EditDataAsk4"), DialogButton.YesNo);
                            if (result == DialogResult.Yes)
                                flag = true;
                            else
                            {
                                OnEditAddress(o);
                                return;
                            }
                        }
                        else if (vm.CanSave)
                            flag = true;
                    }
                }

                if (flag)
                {
                    vm.Refresh();
                    CallPropertiesChanged(vm);
                }
            }
            catch (Exception ex)
            {
                DialogManager.ShowErrorAndLog("Edit Address", ex);
            }
        }

        protected virtual void OnRefreshAddressBinding()
        {
            if (this.RefreshAddressBindingCommand != null)
            {
                int index = this.AddressViewModels.SelectedIndex;

                this.RefreshAddressBindingCommand.Execute(null);
                this.AddressViewModels.SelectedIndex = index;
            }
        }

        protected virtual void CallPropertiesChanged(PersonAddressViewModel vm)
        {
            if (this.AddressPropertiesChanged == null)
                OnRefreshAddressBinding();
            else
                this.AddressPropertiesChanged(vm);
        }

        private void SetDefault(PersonAddressViewModel vm)
        {
            if (vm.IsDefault)
            {
                this.AddressViewModels.Where(m => m.IsDefault && m != vm)
                    .ForEach(m =>
                    {
                        m.IsDefault = false;
                        m.CanSave = false;
                    });
            }
        }

        private void Setup(PersonAddressViewModel vm)
        {
            if (vm == null)
                return;

            if (vm.OldObjectState == ObjectState.Added)
                vm.ViewName = "Add Address";
            else
                vm.ViewName = "Edit Address";
            if (vm.IsLoaded)
                return;

            vm.LoadData(this.Parent);
        }

        #endregion
    }
}
