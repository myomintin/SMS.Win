using CodeflexSystems.Library.Core;
using CodeflexSystems.SMS.Data;
using CodeflexSystems.SMS.Models;
using CodeflexSystems.SMS.ViewModels;
using System;
using System.Windows.Input;

namespace CodeflexSystems.SMS.Core
{
    internal interface IEmailViewModel
    {
        EntityViewModelCollection<EmailViewModel, Email> EmailViewModels { get; }
        ICommand RefreshEmailBindingCommand { get; set; }
        Action<EmailViewModel> EmailPropertiesChanged { get; set; }
    }

    internal interface IPhoneViewModel
    {
        EntityViewModelCollection<PhoneViewModel, Phone> PhoneViewModels { get; }
        ICommand RefreshPhoneBindingCommand { get; set; }
        Action<PhoneViewModel> PhonePropertiesChanged { get; set; }
    }

    internal interface IAddressViewModel
    {
        EntityViewModelCollection<PersonAddressViewModel, PersonAddress> AddressViewModels { get; }
        ICommand RefreshAddressBindingCommand { get; set; }
        Action<PersonAddressViewModel> AddressPropertiesChanged { get; set; }
    }

    internal interface IGuardianViewModel
    {
        EntityViewModelCollection<PersonGuardianViewModel, PersonGuardian> GuardianViewModels { get; }
        ICommand RefreshGuardianBindingCommand { get; set; }
        Action<PersonGuardianViewModel> GuardianPropertiesChanged { get; set; }
    }

    internal interface ILocationAddressViewModel
    {
        //LocationAddressViewModel AddressViewModel { get; set; }
        AddressViewModel AddressViewModel { get; }
    }

    internal interface IParent
    {
        ISMSUnitOfWork UoW { get; }
        bool RemoveAdded { get; }
    }

    internal interface IAddressParent : IParent
    {
        AddViewModel AddAddressViewModel { get; }
        SearchAddressViewModel AddressSelector { get; }
    }

    internal interface IGuardianParent : IParent
    {
        AddViewModel AddGuardianViewModel { get; }
        SearchGuardianViewModel GuardianSelector { get; }
    }

    internal interface IReceiptParent : IParent
    {
        SearchStudentSectionInfoViewModel StudentSectionSelector { get; }
        bool AddAdmissionFee { get; }
        decimal AdmissionFee { get; }
        int FromMonth { get; }
        byte Quantity { get; }
    }

    internal interface IStudentParent : IParent
    {
        SearchSectionInfoViewModel EducationSelector { get; }
        int TotalStudents { get; set; }
    }

    internal interface IStudentSectionParent : IParent
    {
        SearchStudentSectionInfoViewModel StudentSectionSelector { get; }
    }

    internal interface IUserParent : IParent
    {
        UserResetPasswordViewModel UserResetPasswordViewModel { get; }
    }
}
