using CodeflexSystems.SmartUI.Windows.Input;
using CodeflexSystems.SmartUI.Windows.Navigation;
using System;

namespace CodeflexSystems.SMS
{
    internal sealed class SMSLinkNavigator : DefaultLinkNavigator
    {
        public SMSLinkNavigator()
            : base()
        {
            var sv = App.Current.MainWindow as Views.ShellView;

            if (sv != null)
            {
                this.Commands.Add(new Uri("cmd://logon"), new RelayCommand(o => sv.Logon()));
            }
        }
    }
}
