using CodeflexSystems.Library.Extensions;
using CodeflexSystems.SMS.Core;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Timers;
using System.Windows;
using System.Windows.Input;

namespace CodeflexSystems.SMS
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public sealed partial class App : Application
    {
        #region Fields

        private static bool IsSTA;
        private Timer timer;
        private bool hasError;

        #endregion

        #region Properties

        #endregion

        #region Methods

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            IsSTA = System.Threading.Thread.CurrentThread.GetApartmentState() == System.Threading.ApartmentState.STA;
            // enable visual style for Windows Forms Controls
            System.Windows.Forms.Application.EnableVisualStyles();

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            App.Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;

            ApplicationManager.al();
            // Create main window first before showing other.
            App.Current.MainWindow = NavigationManager.ShowMainWindow();
#if DEBUG
            CheckApplicationOptions();
            if (!this.hasError)
            {
                ApplicationManager.ad();
                App.Current.MainWindow.Show();
            }
            if (this.hasError)
                AppExit();
#else
            CheckMutex();
            if (!this.hasError)
            {
                CheckLog();
                CheckApplicationOptions();
                if (!this.hasError)
                {
                    CheckDb();
                    if (!this.hasError)
                    {
                        CheckTimeZone();
                        if (!this.hasError)
                        {
                            CheckDateTime();
                            if (!this.hasError)
                            {
                                CheckDbRequirements();
                                if (!this.hasError)
                                {
                                    ApplicationManager.aa();
                                    if (ApplicationManager.ad())
                                    {
                                        App.Current.MainWindow.Show();
                                        SetTimer();
                                    }
                                    else
                                    {
                                        ShowError();
                                        this.hasError = true;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            if (this.hasError)
                AppExit();
#endif
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            ApplicationManager.Current.ReleaseMutex();
        }

        private void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
            this.Dispatcher.BeginInvoke(new Action(CheckApplication), new object[0]);
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = e.ExceptionObject as Exception;

            this.hasError = true;
            if (ex == null)
                DialogManager.ShowErrorAndLog("Domain Unhandled", e.ExceptionObject.ToString());
            else
                DialogManager.ShowErrorAndLog("Domain Unhandled", ex);
            AppExit();
        }

        private void Current_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            this.hasError = true;
            e.Handled = true;
            MessageBoxManager.ShowErrorAndLog("Dispatcher Unhandled", e.Exception);
            AppExit();
        }

        private void CheckMutex()
        {
            if (!ApplicationManager.Current.CanCreateNew)
            {
                DialogManager.ShowWarning("Warning", SR.GetString("AppAlreadyRunning"));
                this.hasError = true;
            }
        }

        private void CheckLog()
        {
            if (!ApplicationManager.Current.WriteToLog)
                return;

            try
            {
                foreach (var tw in Trace.Listeners.TryCast<TextWriterTraceListener>())
                {
                    var sw = tw.Writer as StreamWriter;
                    if (sw == null)
                        continue;
                    var fs = sw.BaseStream as FileStream;
                    if (fs == null)
                        continue;

                    string name = fs.Name;
                    if (File.Exists(name))
                    {
                        var fi = new FileInfo(name);

                        if (fi.Length > ApplicationManager.Current.MaxLogFileSize)
                        {
                            tw.Flush();
                            tw.Close();
                            tw.Writer = null;

                            var newName = string.Format("{0}-{1}.txt",
                                Path.Combine(fi.DirectoryName, Path.GetFileNameWithoutExtension(name)),
                                DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss"));
                            fi.MoveTo(newName);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ApplicationManager.Current.WriteToLog = false;
                DialogManager.ShowError("Logs", ex.Message);
            }
        }

        private void CheckApplicationOptions()
        {
            if (ApplicationManager.Current.Options == null)
            {
                DialogManager.ShowErrorAndLog("Application Option Error", SR.GetString("FailedToCreateAppOption"));
                this.hasError = true;
            }
        }

        private void CheckDb()
        {
            if (!ApplicationManager.Current.IsDbOK())
            {
                DialogManager.ShowErrorAndLog("Database Connection Error",
                    SR.GetString("IsNotOK", "Database connection"));
                this.hasError = true;
            }
        }

        private void CheckTimeZone()
        {
            if (ApplicationManager.Current.CheckTimeZone &&
                !ApplicationManager.Current.IsValidTimeZone())
            {
                DialogManager.ShowErrorAndLog("Time Zone Error", SR.GetString("TimeZoneShould",
                    ToUTCString(ApplicationManager.Current.TimeZone)));
                this.hasError = true;
            }
        }

        private void CheckDateTime()
        {
            if (ApplicationManager.Current.CheckDateTime)
            {
                bool flag = false;

                try
                {
                    Tuple<DateTime, int> sdt = null;
                    using (var cox = new CodeflexSystems.SMS.Data.SMSContext())
                    {
                        sdt = CodeflexSystems.Library.Data.DbHelper.GetSqlServerUtcDate(cox.Database.Connection.ConnectionString);
                    }

                    if (sdt == null)
                    {
                        DialogManager.ShowErrorAndLog("Database Connection Error",
                            SR.GetString("IsNotOK", "Database connection"));
                    }
                    else
                    {
                        DateTime dt = DateTime.UtcNow;
                        var diff = sdt.Item1 - dt;

                        flag = Math.Abs(diff.TotalSeconds) < sdt.Item2;
                        if (!flag)
                            DialogManager.ShowErrorAndLog("Date Time Error",
                                SR.GetString("CurrentDTofCoputerAndServer",
                                dt.ToLocalTime(), Environment.NewLine, sdt.Item1.ToLocalTime()));
                    }
                }
                catch { }

                if (!flag)
                    this.hasError = true;
            }
        }

        private void CheckDbRequirements()
        {
            try
            {
                ApplicationManager.Current.CheckDbRequirements();
            }
            catch (Exception ex)
            {
                DialogManager.ShowErrorAndLog("Database Initialization Error", ex);
                this.hasError = true;
            }
        }

        private void CheckApplication()
        {
            ApplicationManager.aa(false);
            ApplicationManager.ae();
            if (!ApplicationManager.ad())
            {
                this.timer.Enabled = false;
                ShowError();
                this.hasError = true;
                AppExit();
            }
        }

        private void SetTimer()
        {
            // check every 10 mins.
            if (this.timer == null)
            {
                this.timer = new Timer(600000);
                this.timer.Elapsed += OnTimedEvent;
            }
            if (!this.timer.Enabled)
                this.timer.Enabled = true;
        }

        #endregion

        #region Static Methods

        internal static void CursorWait()
        {
            SetCursor(Cursors.Wait);
        }

        internal static void CursorDefault()
        {
            SetCursor(null);
        }

        internal static void SetCursor(Cursor cursor)
        {
            if (IsSTA && Mouse.OverrideCursor != cursor)
                Mouse.OverrideCursor = cursor;
        }

        internal static void AppExit()
        {
            ApplicationManager.Current.Logoff();
            if (App.Current != null)
                App.Current.Shutdown();
        }

        private static void ShowError([CallerFilePath] string path = "", [CallerMemberName] string member = "", [CallerLineNumber] int line = 0)
        {
            string t = ApplicationManager.ai();
            string m = ApplicationManager.ak();

            if (IsSTA)
                DialogManager.ShowErrorAndLog(t, m, path: path, member: member, line: line);
            else
                MessageBoxManager.ShowErrorAndLog(t, m, path: path, member: member, line: line);
        }

        internal static void TraceToLog(string message, string path, string member, int line)
        {
            Trace.WriteLine(string.Format("computer: {0}", Environment.MachineName));
            Trace.WriteLine(string.Format("user: {0}", ApplicationManager.Current.User.Identity.Name));
            Trace.WriteLine(string.Format("date: {0}", DateTime.UtcNow.ToString("yyyy/M/d h:mm:ss tt")));
            Trace.WriteLine(string.Format("path: {0} -> {1}, {2}", GetFileName(path), member, line));
            Trace.WriteLine(message);
            Trace.WriteLine(string.Empty);
        }

        internal static void TraceToLog(List<Exception> list, string path, string member, int line)
        {
            if (list == null || list.Count == 0)
                return;

            Trace.WriteLine(string.Format("computer: {0}", Environment.MachineName));
            Trace.WriteLine(string.Format("user: {0}", ApplicationManager.Current.User.Identity.Name));
            Trace.WriteLine(string.Format("date: {0}", DateTime.UtcNow.ToString("yyyy/M/d h:mm:ss tt")));
            Trace.WriteLine(string.Format("path: {0} -> {1}, {2}", GetFileName(path), member, line));
            if (list.Count > 1)
                Trace.WriteLine(string.Format("{0}----{0}", Environment.NewLine)
                    .Combine(list.Select(m => m.Message)));
            else
                Trace.WriteLine(list[0].Message);
            Trace.WriteLine(string.Empty);
        }

        private static string GetFileName(string path)
        {
            var fn = Path.GetFileNameWithoutExtension(path);

            if (fn.IndexOf('.') != -1)
                fn = Path.GetFileNameWithoutExtension(fn);

            return fn;
        }

        private static string ToUTCString(TimeSpan ts)
        {
            if (ts == TimeSpan.Zero)
                return "(UTC)";

            string tss = ts.ToString();
            //int index = tss.LastIndexOf(":00");
            //if (index != -1)
            //    tss = tss.Substring(0, index);

            if (ts.Ticks > 0)
                return string.Format("(UTC+{0})", tss);

            return string.Format("(UTC{0})", tss);
        }

        #endregion
    }
}
