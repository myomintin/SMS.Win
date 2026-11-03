using CodeflexSystems.Library.Extensions;
using CodeflexSystems.SmartUI.Windows;
using CodeflexSystems.SMS.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CodeflexSystems.SMS.Licensing
{
    internal static class TD
    {
        internal static TaskDialogResult Show(string title, string instruction,
            TaskDialogButton button = TaskDialogButton.OK,
            TaskDialogImage image = TaskDialogImage.None,
            TaskDialogResult defaultResult = TaskDialogResult.None)
        {
            return TaskDialog.Show(null, title, instruction, null, button, image, defaultResult);
        }

        internal static TaskDialogResult Show(string title, string instruction, string content,
            TaskDialogButton button = TaskDialogButton.OK,
            TaskDialogImage image = TaskDialogImage.None,
            TaskDialogResult defaultResult = TaskDialogResult.None)
        {
            return TaskDialog.Show(null, title, instruction, content, button, image, defaultResult);
        }

        internal static TaskDialogResult ShowError(string title, string instruction,
            TaskDialogButton button = TaskDialogButton.OK,
            TaskDialogResult defaultResult = TaskDialogResult.None)
        {
            return TaskDialog.Show(null, title, instruction, null, button,
                TaskDialogImage.Error, defaultResult);
        }

        internal static TaskDialogResult ShowErrorAndLog(string title, string instruction,
            TaskDialogButton button = TaskDialogButton.OK,
            TaskDialogResult defaultResult = TaskDialogResult.None,
            [CallerFilePath] string path = "", [CallerMemberName] string member = "", [CallerLineNumber] int line = 0)
        {
            if (ApplicationManager.Current.WriteToLog)
                App.TraceToLog(instruction, path, member, line);

            return TaskDialog.Show(null, title, instruction, null, button,
                TaskDialogImage.Error, defaultResult);
        }

        internal static TaskDialogResult ShowError(string title, string instruction, string content,
            TaskDialogButton button = TaskDialogButton.OK,
            TaskDialogResult defaultResult = TaskDialogResult.None)
        {
            return TaskDialog.Show(null, title, instruction, content, button,
                TaskDialogImage.Error, defaultResult);
        }

        internal static TaskDialogResult ShowErrorAndLog(string title, string instruction, string content,
            TaskDialogButton button = TaskDialogButton.OK,
            TaskDialogResult defaultResult = TaskDialogResult.None,
            [CallerFilePath] string path = "", [CallerMemberName] string member = "", [CallerLineNumber] int line = 0)
        {
            if (ApplicationManager.Current.WriteToLog)
                App.TraceToLog(string.Format("{0}----{0}", Environment.NewLine)
                    .Combine(instruction, content), path, member, line);

            return TaskDialog.Show(null, title, instruction, content, button,
                TaskDialogImage.Error, defaultResult);
        }

        internal static TaskDialogResult ShowError(string title, Exception ex,
            TaskDialogButton button = TaskDialogButton.OK,
            TaskDialogResult defaultResult = TaskDialogResult.None)
        {
            return ShowError(title, ex.GetAllExceptions(), button, defaultResult);
        }

        internal static TaskDialogResult ShowErrorAndLog(string title, Exception ex,
            TaskDialogButton button = TaskDialogButton.OK,
            TaskDialogResult defaultResult = TaskDialogResult.None,
            [CallerFilePath] string path = "", [CallerMemberName] string member = "", [CallerLineNumber] int line = 0)
        {
            var list = ex.GetAllExceptions();

            if (ApplicationManager.Current.WriteToLog)
                App.TraceToLog(list, path, member, line);

            return ShowError(title, list, button, defaultResult);
        }

        internal static TaskDialogResult ShowError(string title, List<Exception> list,
            TaskDialogButton button = TaskDialogButton.OK,
            TaskDialogResult defaultResult = TaskDialogResult.None)
        {
            if (ApplicationManager.Current.ReverseErrors)
                list.Reverse();
            if (ApplicationManager.Current.ShowAllErrors)
            {
                string content = null;

                if (list.Count > 1)
                    content = string.Join(Environment.NewLine, list.Skip(1).Select(m => m.Message));

                return TaskDialog.Show(null, title, list.First().Message, content, button,
                    TaskDialogImage.Error, defaultResult);
            }
            else
            {
                if (ApplicationManager.Current.ReverseErrors)
                    return TaskDialog.Show(null, title, list.First().Message, null, button,
                        TaskDialogImage.Error, defaultResult);
                else
                    return TaskDialog.Show(null, title, list.Last().Message, null, button,
                        TaskDialogImage.Error, defaultResult);
            }
        }

        internal static TaskDialogResult ShowInfo(string title, string instruction,
            TaskDialogButton button = TaskDialogButton.OK,
            TaskDialogResult defaultResult = TaskDialogResult.None)
        {
            return TaskDialog.Show(null, title, instruction, null, button,
                TaskDialogImage.Information, defaultResult);
        }

        internal static TaskDialogResult ShowInfo(string title, string instruction, string content,
            TaskDialogButton button = TaskDialogButton.OK,
            TaskDialogResult defaultResult = TaskDialogResult.None)
        {
            return TaskDialog.Show(null, title, instruction, content, button,
                TaskDialogImage.Information, defaultResult);
        }

        internal static TaskDialogResult ShowWarning(string title, string instruction,
            TaskDialogButton button = TaskDialogButton.OK,
            TaskDialogResult defaultResult = TaskDialogResult.None)
        {
            return TaskDialog.Show(null, title, instruction, null, button,
                TaskDialogImage.Warning, defaultResult);
        }

        internal static TaskDialogResult ShowWarning(string title, string instruction, string content,
            TaskDialogButton button = TaskDialogButton.OK,
            TaskDialogResult defaultResult = TaskDialogResult.None)
        {
            return TaskDialog.Show(null, title, instruction, content, button,
                TaskDialogImage.Warning, defaultResult);
        }

        internal static TaskDialogResult AskConfirmation(string title, string instruction,
            TaskDialogButton button = TaskDialogButton.YesNo,
            TaskDialogResult defaultResult = TaskDialogResult.No)
        {
            return TaskDialog.Show(null, title, instruction, null, button,
                TaskDialogImage.Question, defaultResult);
        }

        internal static TaskDialogResult AskConfirmation(string title, string instruction, string content,
            TaskDialogButton button = TaskDialogButton.YesNo,
            TaskDialogResult defaultResult = TaskDialogResult.No)
        {
            return TaskDialog.Show(null, title, instruction, content, button,
                TaskDialogImage.Question, defaultResult);
        }
    }
}
