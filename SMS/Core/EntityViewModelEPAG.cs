using CodeflexSystems.Library.Core;
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
    /// The implementation of the generic view model with Guardians, Addresses, Emails and Phones for entity.
    /// </summary>
    /// <typeparam name="T">entity</typeparam>
    internal abstract class EntityViewModelEPAG<T> : EntityViewModelEPA<T>, IGuardianViewModel
        where T : class
    {
        # region Fields

        private string deleteGuardianContent;

        # endregion

        # region Properties

        public EntityViewModelCollection<PersonGuardianViewModel, PersonGuardian> GuardianViewModels { get; private set; }
        public Action<PersonGuardianViewModel> GuardianPropertiesChanged { get; set; }

        public ICommand AddGuardianCommand { get; protected set; }
        public ICommand EditGuardianCommand { get; protected set; }
        public ICommand DeleteGuardianCommand { get; protected set; }
        public ICommand RefreshGuardianBindingCommand { get; set; }

        public IGuardianParent GuardianParent
        {
            get { return this.Parent as IGuardianParent; }
        }

        public string DeleteGuardianContent
        {
            get { return this.deleteGuardianContent; }
            protected set
            {
                if (this.deleteGuardianContent != value)
                {
                    this.deleteGuardianContent = value;
                    RaisePropertyChanged("DeleteGuardianContent");
                }
            }
        }

        # endregion

        # region Ctor, Dtor

        public EntityViewModelEPAG(T model, IParent parent)
            : base(model, parent)
        {
            this.deleteGuardianContent = "_Delete";
        }

        # endregion

        # region Methods

        protected abstract PersonGuardianViewModel CreateGuardianViewModel(Guardian model);

        protected override void InitializeCommands()
        {
            if (this.IsLoaded)
                return;

            base.InitializeCommands();
            this.AddGuardianCommand = new RelayCommand<object>(o => OnAddGuardian(o));
            this.EditGuardianCommand = new RelayCommand<object>(o => OnEditGuardian(o), o => this.GuardianViewModels.CanEdit);
            this.DeleteGuardianCommand = new RelayCommand(o => OnDeleteGuardian(), o => this.GuardianViewModels.CanDelete);
        }

        protected virtual void InitializeGuardians()
        {
            if (this.GuardianViewModels == null)
            {
                this.GuardianViewModels = new EntityViewModelCollection<PersonGuardianViewModel, PersonGuardian>();
                this.GuardianViewModels.SelectionChanged += (s, e) =>
                {
                    this.DeleteGuardianContent = this.GuardianViewModels.ToDelete ? "Un_delete" : "_Delete";
                };
            }
            else
                this.GuardianViewModels.Clear();
        }

        public virtual void Add(PersonGuardianViewModel vm)
        {
            if (!vm.GuardianViewModel.IsNew)
            {
                Guid id = vm.Model.Guardian.GuardianId;
                if (this.GuardianViewModels.Any(m => id == m.Model.Guardian.GuardianId))
                    throw new AlreadyExistsException(SR.GetString("AlreadyExists2", vm.Model, "Guardian"));
            }

            this.GuardianViewModels.Add(vm, true);
            vm.CanSave = false;
            RaiseRelatedDataChanged(null);
        }

        protected virtual void OnAddGuardian(object o)
        {
            try
            {
                var avm = this.GuardianParent.AddGuardianViewModel;

                if (NavigationManager.ShowAdd(avm, o).DialogResult == false)
                    return;

                if (avm.IsNew)
                {
                    var vm = CreateGuardianViewModel(new Guardian { Gender = true });

                    Setup(vm);
                    if (NavigationManager.ShowPersonGuardian(vm, o).DialogResult == true)
                    {
                        Add(vm);
                    }
                }
                else
                {
                    var gsvm = this.GuardianParent.GuardianSelector;

                    if (NavigationManager.ShowSearchGuardian(gsvm, o).DialogResult == true &&
                        gsvm.Collection.SelectedIndex != -1)
                    {
                        var value = gsvm.Collection.SelectedItem;
                        var model = this.Parent.UoW.FirstOrDefault<Guardian>(m => m.GuardianId == value.GuardianId);

                        if (model != null)
                        {
                            var vm = CreateGuardianViewModel(model);

                            Setup(vm);
                            vm.DetectErrors();
                            if (NavigationManager.ShowPersonGuardian(vm, o).DialogResult == true)
                            {
                                Add(vm);
                            }
                        }
                    }
                }
            }
            catch (AlreadyExistsException ex)
            {
                DialogManager.ShowError("Add Guardian", ex);
            }
            catch (Exception ex)
            {
                DialogManager.ShowErrorAndLog("Add Guardian", ex);
            }
        }

        public virtual bool Delete(PersonGuardianViewModel vm)
        {
            bool flag = this.GuardianViewModels.Delete(vm);

            if (flag)
            {
                vm.CanSave = false;
                RaiseRelatedDataChanged(null);
            }

            return flag;
        }

        protected virtual void OnDeleteGuardian()
        {
            try
            {
                Delete(this.GuardianViewModels.SelectedItem);
            }
            catch (Exception ex)
            {
                DialogManager.ShowErrorAndLog("Delete Guardian", ex);
            }
        }

        public virtual void Edit(PersonGuardianViewModel vm)
        {
            vm.CanSave = false;
            RaiseRelatedDataChanged(null);
        }

        protected virtual void OnEditGuardian(object o)
        {
            try
            {
                bool flag = false;
                var vm = this.GuardianViewModels.SelectedItem;

                Setup(vm);
                if (NavigationManager.ShowPersonGuardian(vm, o).DialogResult == true)
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
                                SR.GetString("EditDataAsk1", "guardian"),
                                SR.GetString("EditDataAsk2"), DialogButton.YesNo);
                            if (result == DialogResult.Yes)
                                this.GuardianViewModels.Remove(vm);
                            else
                            {
                                OnEditGuardian(o);
                                return;
                            }
                        }
                    }
                    else
                    {
                        if (vm.HasAnyErrors)
                        {
                            DialogResult result = DialogManager.AskConfirmation("Confirmation",
                                SR.GetString("EditDataAsk3", "guardian"),
                                SR.GetString("EditDataAsk4"), DialogButton.YesNo);
                            if (result == DialogResult.Yes)
                                flag = true;
                            else
                            {
                                OnEditGuardian(o);
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
                DialogManager.ShowErrorAndLog("Edit Guardian", ex);
            }
        }

        protected virtual void OnRefreshGuardianBinding()
        {
            if (this.RefreshGuardianBindingCommand != null)
            {
                int index = this.GuardianViewModels.SelectedIndex;

                this.RefreshGuardianBindingCommand.Execute(null);
                this.GuardianViewModels.SelectedIndex = index;
            }
        }

        protected virtual void CallPropertiesChanged(PersonGuardianViewModel vm)
        {
            if (this.GuardianPropertiesChanged == null)
                OnRefreshPhoneBinding();
            else
                this.GuardianPropertiesChanged(vm);
        }

        private void Setup(PersonGuardianViewModel vm)
        {
            if (vm == null)
                return;

            if (vm.OldObjectState == ObjectState.Added)
                vm.ViewName = "Add Guardian";
            else
                vm.ViewName = "Edit Guardian";
            if (vm.IsLoaded)
                return;

            vm.LoadData();
        }

        # endregion
    }
}
