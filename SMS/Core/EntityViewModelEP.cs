using CodeflexSystems.Library.Core;
using CodeflexSystems.Library.Extensions;
using CodeflexSystems.SmartUI.Windows.Input;
using CodeflexSystems.SMS.Data;
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
    /// The implementation of the generic view model with Emails and Phones for entity.
    /// </summary>
    /// <typeparam name="T">entity</typeparam>
    //[Serializable]
    internal abstract class EntityViewModelEP<T> : EntityViewModelCM<T>, IEmailViewModel, IPhoneViewModel
        where T : class
    {
        #region Fields

        private string deleteEmailContent;
        private string deletePhoneContent;

        #endregion

        #region Properties

        public EntityViewModelCollection<EmailViewModel, Email> EmailViewModels { get; private set; }
        public EntityViewModelCollection<PhoneViewModel, Phone> PhoneViewModels { get; private set; }
        public Action<EmailViewModel> EmailPropertiesChanged { get; set; }
        public Action<PhoneViewModel> PhonePropertiesChanged { get; set; }
        public bool IsDetectedChanges { get; protected set; }

        public ICommand AddEmailCommand { get; protected set; }
        public ICommand EditEmailCommand { get; protected set; }
        public ICommand DeleteEmailCommand { get; protected set; }
        public ICommand RefreshEmailBindingCommand { get; set; }

        public ICommand AddPhoneCommand { get; protected set; }
        public ICommand EditPhoneCommand { get; protected set; }
        public ICommand DeletePhoneCommand { get; protected set; }
        public ICommand RefreshPhoneBindingCommand { get; set; }

        public string DeleteEmailContent
        {
            get { return this.deleteEmailContent; }
            protected set
            {
                if (this.deleteEmailContent != value)
                {
                    this.deleteEmailContent = value;
                    RaisePropertyChanged("DeleteEmailContent");
                }
            }
        }

        public string DeletePhoneContent
        {
            get { return this.deletePhoneContent; }
            protected set
            {
                if (this.deletePhoneContent != value)
                {
                    this.deletePhoneContent = value;
                    RaisePropertyChanged("DeletePhoneContent");
                }
            }
        }

        #endregion

        #region Ctor, Dtor

        public EntityViewModelEP(T model, IParent parent)
            : base(model, parent)
        {
            this.deleteEmailContent = "_Delete";
            this.deletePhoneContent = "_Delete";
        }

        #endregion

        #region Methods

        protected abstract EmailViewModel CreateEmailViewModel();
        protected abstract PhoneViewModel CreatePhoneViewModel();
        internal abstract void DetectChanges();

        protected virtual void InitializeCommands()
        {
            if (this.IsLoaded)
                return;

            this.AddEmailCommand = new RelayCommand<object>(o => OnAddEmail(o));
            this.EditEmailCommand = new RelayCommand<object>(o => OnEditEmail(o), o => this.EmailViewModels.CanEdit);
            this.DeleteEmailCommand = new RelayCommand(o => OnDeleteEmail(), o => this.EmailViewModels.CanDelete);

            this.AddPhoneCommand = new RelayCommand<object>(o => OnAddPhone(o));
            this.EditPhoneCommand = new RelayCommand<object>(o => OnEditPhone(o), o => this.PhoneViewModels.CanEdit);
            this.DeletePhoneCommand = new RelayCommand(o => OnDeletePhone(), o => this.PhoneViewModels.CanDelete);
        }

        protected void InitializeEmails()
        {
            if (this.EmailViewModels == null)
            {
                this.EmailViewModels = new EntityViewModelCollection<EmailViewModel, Email>();
                this.EmailViewModels.SelectionChanged += (s, e) =>
                {
                    this.DeleteEmailContent = this.EmailViewModels.ToDelete ? "Un_delete" : "_Delete";
                };
            }
            else
                this.EmailViewModels.Clear();
        }

        protected void InitializePhones()
        {
            if (this.PhoneViewModels == null)
            {
                this.PhoneViewModels = new EntityViewModelCollection<PhoneViewModel, Phone>();
                this.PhoneViewModels.SelectionChanged += (s, e) =>
                {
                    this.DeletePhoneContent = this.PhoneViewModels.ToDelete ? "Un_delete" : "_Delete";
                };
            }
            else
                this.PhoneViewModels.Clear();
        }

        public virtual void Add(EmailViewModel vm)
        {
            if (this.EmailViewModels.Any(m =>
                string.Equals(vm.EmailAddress, m.EmailAddress, StringComparison.InvariantCultureIgnoreCase)))
                throw new AlreadyExistsException(SR.GetString("AlreadyExists2", vm.Model, "Email Address"));
            
            this.EmailViewModels.Add(vm, true);
            SetDefault(vm);
            vm.CanSave = false;
            RaiseRelatedDataChanged(null);
        }

        protected virtual void OnAddEmail(object o)
        {
            try
            {
                var vm = CreateEmailViewModel();

                Setup(vm);
                if (this.EmailViewModels.Count == 0)
                    vm.Model.IsDefault = true;
                if (NavigationManager.ShowEmail(vm, o).DialogResult == true)
                {
                    Add(vm);
                }
            }
            catch (AlreadyExistsException ex)
            {
                DialogManager.ShowError("Add Email", ex);
            }
            catch (Exception ex)
            {
                DialogManager.ShowErrorAndLog("Add Email", ex);
            }
        }

        public virtual bool Delete(EmailViewModel vm)
        {
            bool flag = this.EmailViewModels.Delete(vm);

            if (flag)
            {
                vm.CanSave = false;
                RaiseRelatedDataChanged(null);
            }

            return flag;
        }

        protected virtual void OnDeleteEmail()
        {
            try
            {
                Delete(this.EmailViewModels.SelectedItem);
            }
            catch (Exception ex)
            {
                DialogManager.ShowErrorAndLog("Delete Email", ex);
            }
        }

        public virtual void Edit(EmailViewModel vm)
        {
            SetDefault(vm);
            vm.CanSave = false;
            RaiseRelatedDataChanged(null);
        }

        protected virtual void OnEditEmail(object o)
        {
            try
            {
                bool flag = false;
                var vm = this.EmailViewModels.SelectedItem;

                Setup(vm);
                if (NavigationManager.ShowEmail(vm, o).DialogResult == true)
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
                                SR.GetString("EditDataAsk1", "email"),
                                SR.GetString("EditDataAsk2"), DialogButton.YesNo);
                            if (result == DialogResult.Yes)
                                this.EmailViewModels.Remove(vm);
                            else
                            {
                                OnEditEmail(o);
                                return;
                            }
                        }
                    }
                    else
                    {
                        if (vm.HasAnyErrors)
                        {
                            DialogResult result = DialogManager.AskConfirmation("Confirmation",
                                SR.GetString("EditDataAsk3", "email"),
                                SR.GetString("EditDataAsk4"), DialogButton.YesNo);
                            if (result == DialogResult.Yes)
                                flag = true;
                            else
                            {
                                OnEditEmail(o);
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
                DialogManager.ShowErrorAndLog("Edit Email", ex);
            }
        }

        public virtual void Add(PhoneViewModel vm)
        {
            if (this.PhoneViewModels.Any(m =>
                string.Equals(vm.PhoneNumber, m.PhoneNumber, StringComparison.InvariantCultureIgnoreCase) &&
                string.Equals(vm.Extension, m.Extension, StringComparison.InvariantCultureIgnoreCase)))
                throw new AlreadyExistsException(SR.GetString("AlreadyExists2", vm.Model, "Phone Number"));
            
            this.PhoneViewModels.Add(vm, true);
            SetDefault(vm);
            vm.CanSave = false;
            RaiseRelatedDataChanged(null);
        }

        protected virtual void OnAddPhone(object o)
        {
            try
            {
                var vm = CreatePhoneViewModel();

                Setup(vm);
                if (this.PhoneViewModels.Count == 0)
                    vm.Model.IsDefault = true;
                if (NavigationManager.ShowPhone(vm, o).DialogResult == true)
                {
                    Add(vm);
                }
            }
            catch (AlreadyExistsException ex)
            {
                DialogManager.ShowError("Add Phone", ex);
            }
            catch (Exception ex)
            {
                DialogManager.ShowErrorAndLog("Add Phone", ex);
            }
        }

        public virtual bool Delete(PhoneViewModel vm)
        {
            bool flag = this.PhoneViewModels.Delete(vm);

            if (flag)
            {
                vm.CanSave = false;
                RaiseRelatedDataChanged(null);
            }

            return flag;
        }

        protected virtual void OnDeletePhone()
        {
            try
            {
                Delete(this.PhoneViewModels.SelectedItem);
            }
            catch (Exception ex)
            {
                DialogManager.ShowErrorAndLog("Delete Phone", ex);
            }
        }

        public virtual void Edit(PhoneViewModel vm)
        {
            SetDefault(vm);
            vm.CanSave = false;
            RaiseRelatedDataChanged(null);
        }

        protected virtual void OnEditPhone(object o)
        {
            try
            {
                bool flag = false;
                var vm = this.PhoneViewModels.SelectedItem;

                Setup(vm);
                if (NavigationManager.ShowPhone(vm, o).DialogResult == true)
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
                                SR.GetString("EditDataAsk1", "phone"),
                                SR.GetString("EditDataAsk2"), DialogButton.YesNo);
                            if (result == DialogResult.Yes)
                                this.PhoneViewModels.Remove(vm);
                            else
                            {
                                OnEditPhone(o);
                                return;
                            }
                        }
                    }
                    else
                    {
                        if (vm.HasAnyErrors)
                        {
                            DialogResult result = DialogManager.AskConfirmation("Confirmation",
                                SR.GetString("EditDataAsk3", "phone"),
                                SR.GetString("EditDataAsk4"), DialogButton.YesNo);
                            if (result == DialogResult.Yes)
                                flag = true;
                            else
                            {
                                OnEditPhone(o);
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
                DialogManager.ShowErrorAndLog("Edit Phone", ex);
            }
        }

        protected virtual void OnRefreshEmailBinding()
        {
            if (this.RefreshEmailBindingCommand != null)
            {
                int index = this.EmailViewModels.SelectedIndex;

                this.RefreshEmailBindingCommand.Execute(null);
                this.EmailViewModels.SelectedIndex = index;
            }
        }

        protected virtual void CallPropertiesChanged(EmailViewModel vm)
        {
            if (this.EmailPropertiesChanged == null)
                OnRefreshEmailBinding();
            else
                this.EmailPropertiesChanged(vm);
        }

        protected virtual void OnRefreshPhoneBinding()
        {
            if (this.RefreshPhoneBindingCommand != null)
            {
                int index = this.PhoneViewModels.SelectedIndex;

                this.RefreshPhoneBindingCommand.Execute(null);
                this.PhoneViewModels.SelectedIndex = index;
            }
        }

        protected virtual void CallPropertiesChanged(PhoneViewModel vm)
        {
            if (this.PhonePropertiesChanged == null)
                OnRefreshPhoneBinding();
            else
                this.PhonePropertiesChanged(vm);
        }

        private void SetDefault(EmailViewModel vm)
        {
            if (vm.IsDefault)
            {
                this.EmailViewModels.Where(m => m.IsDefault && m != vm)
                    .ForEach(m =>
                    {
                        m.IsDefault = false;
                        m.CanSave = false;
                    });
            }
        }

        private void Setup(EmailViewModel vm)
        {
            if (vm == null)
                return;

            if (vm.OldObjectState == ObjectState.Added)
                vm.ViewName = "Add Email";
            else
                vm.ViewName = "Edit Email";
            if (vm.IsLoaded)
                return;

            vm.DataChanged += (s, e) =>
            {
                if (e.PropertyName == "EmailAddress")
                {
                    bool flag = false;

                    foreach (var item in this.EmailViewModels.Where(m =>
                        string.Equals(vm.EmailAddress, m.EmailAddress, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        if (flag = (item != vm))
                            break;
                    }

                    if (flag)
                    {
                        vm.SetErrors(e.PropertyName, SR.GetString("AlreadyExists1", "Email Address", vm.EmailAddress));
                        vm.DetectErrors();
                    }
                }
            };
            vm.LoadData(this.Parent);
        }

        private void SetDefault(PhoneViewModel vm)
        {
            if (vm.IsDefault)
            {
                this.PhoneViewModels.Where(m => m.IsDefault && m != vm)
                    .ForEach(m =>
                    {
                        m.IsDefault = false;
                        m.CanSave = false;
                    });
            }
        }

        private void Setup(PhoneViewModel vm)
        {
            if (vm == null)
                return;

            if (vm.IsNew)
                vm.ViewName = "Add Phone";
            else
                vm.ViewName = "Edit Phone";
            if (vm.IsLoaded)
                return;

            vm.DataChanged += (s, e) =>
            {
                if (e.PropertyName == "PhoneNumber")
                {
                    bool flag = false;

                    foreach (var item in this.PhoneViewModels.Where(m =>
                        string.Equals(vm.PhoneNumber, m.PhoneNumber, StringComparison.InvariantCultureIgnoreCase) &&
                        string.Equals(vm.Extension, m.Extension, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        if (flag = (item != vm))
                            break;
                    }

                    if (flag)
                    {
                        vm.SetErrors(e.PropertyName, SR.GetString("AlreadyExists1", "Phone Number", vm.PhoneNumber));
                        vm.DetectErrors();
                    }
                }
            };
            vm.LoadData(this.Parent);
        }

        #endregion
    }
}
