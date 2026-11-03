using CodeflexSystems.Library.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CodeflexSystems.SMS.Core
{
    public static class MessageBoxManager
    {
        public static MessageBoxResult Show(string title, string text,
            MessageBoxButton button = MessageBoxButton.OK,
            MessageBoxImage image = MessageBoxImage.None,
            MessageBoxResult defaultResult = MessageBoxResult.None)
        {
            return MessageBox.Show(null, text, title, button, image, defaultResult);
        }

        public static MessageBoxResult ShowError(string title, string text,
            MessageBoxButton button = MessageBoxButton.OK,
            MessageBoxResult defaultResult = MessageBoxResult.None)
        {
            return MessageBox.Show(null, text, title, button, MessageBoxImage.Error, defaultResult);
        }

        public static MessageBoxResult ShowErrorAndLog(string title, string text,
            MessageBoxButton button = MessageBoxButton.OK,
            MessageBoxResult defaultResult = MessageBoxResult.None,
            [CallerFilePath] string path = "", [CallerMemberName] string member = "", [CallerLineNumber] int line = 0)
        {
            if (ApplicationManager.Current.WriteToLog)
                App.TraceToLog(text, path, member, line);

            return MessageBox.Show(null, text, title, button, MessageBoxImage.Error, defaultResult);
        }

        public static MessageBoxResult ShowError(string title, Exception ex,
            MessageBoxButton button = MessageBoxButton.OK,
            MessageBoxResult defaultResult = MessageBoxResult.None)
        {
            return ShowError(title, ex.GetAllExceptions(), button, defaultResult);
        }

        public static MessageBoxResult ShowErrorAndLog(string title, Exception ex,
            MessageBoxButton button = MessageBoxButton.OK,
            MessageBoxResult defaultResult = MessageBoxResult.None,
            [CallerFilePath] string path = "", [CallerMemberName] string member = "", [CallerLineNumber] int line = 0)
        {
            var list = ex.GetAllExceptions();

            if (ApplicationManager.Current.WriteToLog)
                App.TraceToLog(list, path, member, line);

            return ShowError(title, list, button, defaultResult);
        }

        public static MessageBoxResult ShowError(string title, List<Exception> list,
            MessageBoxButton button = MessageBoxButton.OK,
            MessageBoxResult defaultResult = MessageBoxResult.None)
        {
            if (ApplicationManager.Current.ReverseErrors)
                list.Reverse();
            if (ApplicationManager.Current.ShowAllErrors)
            {
                string text = string.Join(Environment.NewLine, list.Select(m => m.Message));

                return MessageBox.Show(null, text, title, button, MessageBoxImage.Error, defaultResult);
            }
            else
            {
                if (ApplicationManager.Current.ReverseErrors)
                    return MessageBox.Show(null, list.First().Message, title, button,
                        MessageBoxImage.Error, defaultResult);
                else
                    return MessageBox.Show(null, list.Last().Message, title, button,
                        MessageBoxImage.Error, defaultResult);
            }
        }

        public static MessageBoxResult ShowInfo(string title, string text,
            MessageBoxButton button = MessageBoxButton.OK,
            MessageBoxResult defaultResult = MessageBoxResult.None)
        {
            return MessageBox.Show(null, text, title, button, MessageBoxImage.Information, defaultResult);
        }

        public static MessageBoxResult ShowWarning(string title, string text,
            MessageBoxButton button = MessageBoxButton.OK,
            MessageBoxResult defaultResult = MessageBoxResult.None)
        {
            return MessageBox.Show(null, text, title, button, MessageBoxImage.Warning, defaultResult);
        }

        public static MessageBoxResult AskConfirmation(string title, string text,
            MessageBoxButton button = MessageBoxButton.YesNo,
            MessageBoxResult defaultResult = MessageBoxResult.No)
        {
            return MessageBox.Show(null, text, title, button, MessageBoxImage.Question, defaultResult);
        }
    }
}
