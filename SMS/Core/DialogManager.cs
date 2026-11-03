using CodeflexSystems.Library.Extensions;
using CodeflexSystems.SmartUI.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CodeflexSystems.SMS.Core
{
    #region Enums

    internal enum DialogButton
    {
        Close = 0x00,
        OK = 0x10,
        OKCancel = 0x20,
        YesNoCancel = 0x30,
        YesNo = 0x40,
        RetryCancel = 0x50
    }

    internal enum DialogImage
    {
        None = 0x00,
        Hand = 0x10,
        Question = 0x20,
        Exclamation = 0x30,
        Asterisk = 0x40,
        Stop = 0x50,
        Error = 0x60,
        Warning = 0x70,
        Information = 0x80,
        Window = 0x90,
        WindowRoundGreen = 0x100,
        WindowRoundWhite = 0x110,
        Shield = 0x120
    }

    internal enum DialogResult
    {
        None = 0x00,
        OK = 0x10,
        Cancel = 0x20,
        Yes = 0x30,
        No = 0x40,
        Retry = 0x50
    }

    #endregion

    internal static class DialogManager
    {
        public static DialogResult Show(string title, string instruction,
            DialogButton button = DialogButton.OK,
            DialogImage icon = DialogImage.None,
            DialogResult defaultResult = DialogResult.None)
        {
            return ConvertResult(TaskDialog.Show(null, title, instruction, null,
                ConvertButton(button), ConvertImage(icon), ConvertResult(defaultResult)));
        }

        public static DialogResult Show(string title, string instruction, string content,
            DialogButton button = DialogButton.OK,
            DialogImage icon = DialogImage.None,
            DialogResult defaultResult = DialogResult.None)
        {
            return ConvertResult(TaskDialog.Show(null, title, instruction, content,
                ConvertButton(button), ConvertImage(icon), ConvertResult(defaultResult)));
        }

        public static DialogResult ShowError(string title, string instruction,
            DialogButton button = DialogButton.OK,
            DialogResult defaultResult = DialogResult.None)
        {
            return ConvertResult(TaskDialog.Show(null, title, instruction, null,
                   ConvertButton(button), TaskDialogImage.Error, ConvertResult(defaultResult)));
        }

        public static DialogResult ShowErrorAndLog(string title, string instruction,
            DialogButton button = DialogButton.OK,
            DialogResult defaultResult = DialogResult.None,
            [CallerFilePath] string path = "", [CallerMemberName] string member = "", [CallerLineNumber] int line = 0)
        {
            if (ApplicationManager.Current.WriteToLog)
                App.TraceToLog(instruction, path, member, line);

            return ConvertResult(TaskDialog.Show(null, title, instruction, null,
                ConvertButton(button), TaskDialogImage.Error, ConvertResult(defaultResult)));
        }

        public static DialogResult ShowError(string title, string instruction, string content,
            DialogButton button = DialogButton.OK,
            DialogResult defaultResult = DialogResult.None)
        {
            return ConvertResult(TaskDialog.Show(null, title, instruction, content,
                   ConvertButton(button), TaskDialogImage.Error, ConvertResult(defaultResult)));
        }

        public static DialogResult ShowErrorAndLog(string title, string instruction, string content,
            DialogButton button = DialogButton.OK,
            DialogResult defaultResult = DialogResult.None,
            [CallerFilePath] string path = "", [CallerMemberName] string member = "", [CallerLineNumber] int line = 0)
        {
            if (ApplicationManager.Current.WriteToLog)
                App.TraceToLog(string.Format("{0}----{0}", Environment.NewLine)
                    .Combine(instruction, content), path, member, line);

            return ConvertResult(TaskDialog.Show(null, title, instruction, content,
                ConvertButton(button), TaskDialogImage.Error, ConvertResult(defaultResult)));
        }

        public static DialogResult ShowError(string title, Exception ex,
            DialogButton button = DialogButton.OK,
            DialogResult defaultResult = DialogResult.None)
        {
            return ShowError(title, ex.GetAllExceptions(), button, defaultResult);
        }

        public static DialogResult ShowErrorAndLog(string title, Exception ex,
            DialogButton button = DialogButton.OK,
            DialogResult defaultResult = DialogResult.None,
            [CallerFilePath] string path = "", [CallerMemberName] string member = "", [CallerLineNumber] int line = 0)
        {
            var list = ex.GetAllExceptions();

            if (ApplicationManager.Current.WriteToLog)
                App.TraceToLog(list, path, member, line);

            return ShowError(title, list, button, defaultResult);
        }

        public static DialogResult ShowError(string title, List<Exception> list,
            DialogButton button = DialogButton.OK,
            DialogResult defaultResult = DialogResult.None)
        {
            if (ApplicationManager.Current.ReverseErrors)
                list.Reverse();
            if (ApplicationManager.Current.ShowAllErrors)
            {
                string content = null;

                if (list.Count > 1)
                    content = string.Join(Environment.NewLine, list.Skip(1).Select(m => m.Message));

                return ConvertResult(TaskDialog.Show(null, title, list.First().Message, content,
                       ConvertButton(button), TaskDialogImage.Error, ConvertResult(defaultResult)));
            }
            else
            {
                if (ApplicationManager.Current.ReverseErrors)
                    return ConvertResult(TaskDialog.Show(null, title, list.First().Message, null,
                           ConvertButton(button), TaskDialogImage.Error, ConvertResult(defaultResult)));
                else
                    return ConvertResult(TaskDialog.Show(null, title, list.Last().Message, null,
                           ConvertButton(button), TaskDialogImage.Error, ConvertResult(defaultResult)));
            }
        }

        public static void WriteToLog(string instruction, string content,
            [CallerFilePath] string path = "", [CallerMemberName] string member = "", [CallerLineNumber] int line = 0)
        {
            if (ApplicationManager.Current.WriteToLog)
            {
                if (string.IsNullOrEmpty(content))
                    App.TraceToLog(instruction, path, member, line);
                else
                    App.TraceToLog(string.Format("{0}----{0}", Environment.NewLine)
                        .Combine(instruction, content), path, member, line);
            }
        }

        public static DialogResult ShowInfo(string title, string instruction,
            DialogButton button = DialogButton.OK,
            DialogResult defaultResult = DialogResult.None)
        {
            return ConvertResult(TaskDialog.Show(null, title, instruction, null,
                ConvertButton(button), TaskDialogImage.Information, ConvertResult(defaultResult)));
        }

        public static DialogResult ShowInfo(string title, string instruction, string content,
            DialogButton button = DialogButton.OK,
            DialogResult defaultResult = DialogResult.None)
        {
            return ConvertResult(TaskDialog.Show(null, title, instruction, content,
                ConvertButton(button), TaskDialogImage.Information, ConvertResult(defaultResult)));
        }

        public static DialogResult ShowWarning(string title, string instruction,
            DialogButton button = DialogButton.OK,
            DialogResult defaultResult = DialogResult.None)
        {
            return ConvertResult(TaskDialog.Show(null, title, instruction, null,
                ConvertButton(button), TaskDialogImage.Warning, ConvertResult(defaultResult)));
        }

        public static DialogResult ShowWarning(string title, string instruction, string content,
            DialogButton button = DialogButton.OK,
            DialogResult defaultResult = DialogResult.None)
        {
            return ConvertResult(TaskDialog.Show(null, title, instruction, content,
                ConvertButton(button), TaskDialogImage.Warning, ConvertResult(defaultResult)));
        }

        public static DialogResult AskConfirmation(string title, string instruction,
            DialogButton button = DialogButton.YesNo,
            DialogResult defaultResult = DialogResult.No)
        {
            return ConvertResult(TaskDialog.Show(null, title, instruction, null,
                ConvertButton(button), TaskDialogImage.Question, ConvertResult(defaultResult)));
        }

        public static DialogResult AskConfirmation(string title, string instruction, string content,
            DialogButton button = DialogButton.YesNo,
            DialogResult defaultResult = DialogResult.No)
        {
            return ConvertResult(TaskDialog.Show(null, title, instruction, content,
                ConvertButton(button), TaskDialogImage.Question, ConvertResult(defaultResult)));
        }

        public static DialogResult AskConfirmationToSaveData()
        {
            return ConvertResult(TaskDialog.Show(null, "Confirmation",
                SR.GetString("SaveDataAsk1"), SR.GetString("SaveDataAsk2"),
                TaskDialogButton.YesNoCancel, TaskDialogImage.Question, TaskDialogResult.Cancel));
        }

        private static TaskDialogButton ConvertButton(DialogButton button)
        {
            return (TaskDialogButton)(int)button;
        }

        private static TaskDialogImage ConvertImage(DialogImage icon)
        {
            return (TaskDialogImage)(int)icon;
        }

        private static TaskDialogResult ConvertResult(DialogResult result)
        {
            return (TaskDialogResult)(int)result;
        }

        private static DialogResult ConvertResult(TaskDialogResult result)
        {
            return (DialogResult)(int)result;
        }
    }

    //internal static class DialogManager
    //{
    //    public static DialogResult Show(string title, string instruction)
    //    {
    //        return ConvertResult(TaskDialog.Show(null, title, instruction, null,
    //            TaskDialogButton.OK, TaskDialogImage.None, TaskDialogResult.None));
    //    }

    //    public static DialogResult Show(string title, string instruction, string content)
    //    {
    //        return ConvertResult(TaskDialog.Show(null, title, instruction, content,
    //            TaskDialogButton.OK, TaskDialogImage.None, TaskDialogResult.None));
    //    }

    //    public static DialogResult Show(string title, string instruction, string content,
    //        DialogButton button)
    //    {
    //        return ConvertResult(TaskDialog.Show(null, title, instruction, content,
    //            ConvertButton(button), TaskDialogImage.None, TaskDialogResult.None));
    //    }

    //    public static DialogResult Show(string title, string instruction, string content,
    //        DialogButton button, DialogImage icon)
    //    {
    //        return ConvertResult(TaskDialog.Show(null, title, instruction, content,
    //            ConvertButton(button), ConvertImage(icon), TaskDialogResult.None));
    //    }

    //    public static DialogResult Show(string title, string instruction, string content,
    //        DialogButton button, DialogImage icon, DialogResult defaultResult)
    //    {
    //        return ConvertResult(TaskDialog.Show(null, title, instruction, content,
    //            ConvertButton(button), ConvertImage(icon), ConvertResult(defaultResult)));
    //    }

    //    public static DialogResult ShowError(string title, string instruction,
    //        DialogButton button = DialogButton.OK)
    //    {
    //        return ConvertResult(TaskDialog.Show(null, title, instruction, null,
    //               ConvertButton(button), TaskDialogImage.Error, TaskDialogResult.None));
    //    }

    //    public static DialogResult ShowErrorAndLog(string title, string instruction,
    //        DialogButton button = DialogButton.OK,
    //        [CallerFilePath] string path = "", [CallerMemberName] string member = "", [CallerLineNumber] int line = 0)
    //    {
    //        if (ApplicationManager.Current.WriteToLog)
    //            App.TraceToLog(instruction, path, member, line);

    //        return ConvertResult(TaskDialog.Show(null, title, instruction, null,
    //            ConvertButton(button), TaskDialogImage.Error, TaskDialogResult.None));
    //    }

    //    public static DialogResult ShowError(string title, string instruction, string content,
    //        DialogButton button = DialogButton.OK)
    //    {
    //        return ConvertResult(TaskDialog.Show(null, title, instruction, content,
    //               ConvertButton(button), TaskDialogImage.Error, TaskDialogResult.None));
    //    }

    //    public static DialogResult ShowErrorAndLog(string title, string instruction, string content,
    //        DialogButton button = DialogButton.OK,
    //        [CallerFilePath] string path = "", [CallerMemberName] string member = "", [CallerLineNumber] int line = 0)
    //    {
    //        if (ApplicationManager.Current.WriteToLog)
    //            App.TraceToLog(string.Format("{0}----{0}", Environment.NewLine)
    //                .Combine(instruction, content), path, member, line);

    //        return ConvertResult(TaskDialog.Show(null, title, instruction, content,
    //            ConvertButton(button), TaskDialogImage.Error, TaskDialogResult.None));
    //    }

    //    public static DialogResult ShowError(string title, Exception ex)
    //    {
    //        return ShowError(title, ex.GetAllExceptions());
    //    }

    //    public static DialogResult ShowErrorAndLog(string title, Exception ex,
    //        [CallerFilePath] string path = "", [CallerMemberName] string member = "", [CallerLineNumber] int line = 0)
    //    {
    //        var list = ex.GetAllExceptions();

    //        if (ApplicationManager.Current.WriteToLog)
    //            App.TraceToLog(list, path, member, line);

    //        return ShowError(title, list);
    //    }

    //    public static DialogResult ShowError(string title, List<Exception> list)
    //    {
    //        if (ApplicationManager.Current.ReverseErrors)
    //            list.Reverse();
    //        if (ApplicationManager.Current.ShowAllErrors)
    //        {
    //            string content = null;

    //            if (list.Count > 1)
    //                content = string.Join(Environment.NewLine, list.Skip(1).Select(m => m.Message));

    //            return ConvertResult(TaskDialog.ShowError(title, list.First().Message, content));
    //        }
    //        else
    //        {
    //            if (ApplicationManager.Current.ReverseErrors)
    //                return ConvertResult(TaskDialog.ShowError(title, list.First().Message));
    //            else
    //                return ConvertResult(TaskDialog.ShowError(title, list.Last().Message));
    //        }
    //    }

    //    public static void WriteToLog(string instruction, string content,
    //        [CallerFilePath] string path = "", [CallerMemberName] string member = "", [CallerLineNumber] int line = 0)
    //    {
    //        if (ApplicationManager.Current.WriteToLog)
    //        {
    //            if (string.IsNullOrEmpty(content))
    //                App.TraceToLog(instruction, path, member, line);
    //            else
    //                App.TraceToLog(string.Format("{0}----{0}", Environment.NewLine)
    //                    .Combine(instruction, content), path, member, line);
    //        }
    //    }

    //    public static DialogResult ShowInfo(string title, string instruction,
    //        DialogButton button = DialogButton.OK)
    //    {
    //        return ConvertResult(TaskDialog.Show(null, title, instruction, null,
    //            ConvertButton(button), TaskDialogImage.Information, TaskDialogResult.None));
    //    }

    //    public static DialogResult ShowInfo(string title, string instruction, string content,
    //        DialogButton button = DialogButton.OK)
    //    {
    //        return ConvertResult(TaskDialog.Show(null, title, instruction, content,
    //            ConvertButton(button), TaskDialogImage.Information, TaskDialogResult.None));
    //    }

    //    public static DialogResult ShowWarning(string title, string instruction,
    //        DialogButton button = DialogButton.OK)
    //    {
    //        return ConvertResult(TaskDialog.Show(null, title, instruction, null,
    //            ConvertButton(button), TaskDialogImage.Warning, TaskDialogResult.None));
    //    }

    //    public static DialogResult ShowWarning(string title, string instruction, string content,
    //        DialogButton button = DialogButton.OK)
    //    {
    //        return ConvertResult(TaskDialog.Show(null, title, instruction, content,
    //            ConvertButton(button), TaskDialogImage.Warning, TaskDialogResult.None));
    //    }

    //    public static DialogResult AskConfirmation(string title, string instruction,
    //        DialogButton button = DialogButton.YesNo)
    //    {
    //        return ConvertResult(TaskDialog.Show(null, title, instruction, null,
    //            ConvertButton(button), TaskDialogImage.Question, TaskDialogResult.No));
    //    }

    //    public static DialogResult AskConfirmation(string title, string instruction, string content,
    //        DialogButton button = DialogButton.YesNo)
    //    {
    //        return ConvertResult(TaskDialog.Show(null, title, instruction, content,
    //            ConvertButton(button), TaskDialogImage.Question, TaskDialogResult.No));
    //    }

    //    public static DialogResult AskConfirmationToSaveData()
    //    {
    //        return ConvertResult(TaskDialog.Show(null, "Confirmation",
    //            SR.GetString("SaveDataAsk1"), SR.GetString("SaveDataAsk2"),
    //            TaskDialogButton.YesNoCancel, TaskDialogImage.Question, TaskDialogResult.Cancel));
    //    }

    //    private static TaskDialogButton ConvertButton(DialogButton button)
    //    {
    //        return (TaskDialogButton)(int)button;
    //    }

    //    private static TaskDialogImage ConvertImage(DialogImage icon)
    //    {
    //        return (TaskDialogImage)(int)icon;
    //    }

    //    private static TaskDialogResult ConvertResult(DialogResult result)
    //    {
    //        return (TaskDialogResult)(int)result;
    //    }

    //    private static DialogResult ConvertResult(TaskDialogResult result)
    //    {
    //        return (DialogResult)(int)result;
    //    }
    //}
}
