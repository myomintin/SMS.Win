using CodeflexSystems.SMS.Core;
using CodeflexSystems.SMS.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CodeflexSystems.SMS.Views
{
    /// <summary>
    /// Interaction logic for UserOptionsView.xaml
    /// </summary>
    public sealed partial class UserOptionsView : UserControl
    {
        private List<object> views;
        private int index = -1;

        internal UserOptionsViewModel ViewModel
        {
            get { return this.DataContext as UserOptionsViewModel; }
        }

        public UserOptionsView()
        {
            InitializeComponent();
            this.Loaded += (s, e) =>
            {
                if (this.ViewModel == null)
                {
                    App.CursorWait();
                    this.DataContext = ApplicationManager.Current.Options;
                    this.ViewModel.PropertyChanged += (ss, ee) =>
                    {
                        ChangedValues();
                    };
                    ChangedValues();
                    App.CursorDefault();
                }
                if (views == null)
                {
                    views = GetViews().ToList();
                }
            };
        }

        private void TransitionEffectsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (views != null)
            {
                TransitionEffectContentControl.Content = views[++index % 2];
            }
        }

        private IEnumerable<object> GetViews()
        {
            yield return ApplicationManager.Resolve<Content1View>();
            yield return ApplicationManager.Resolve<Content2View>();
        }

        private void ChangedValues()
        {
            var dt = DateTime.Now;
            decimal value = 12345678.90M;

            this.DatePatternDTP.Value = dt;
            this.TimePatternTP.Value = dt;
            this.DateTimePatternDTP.Value = dt;
            this.CurrencyPositivePatternTextBox.Text = value.ToString("C");
            this.CurrencyNegativePatternTextBox.Text = (value * -1).ToString("C");
            //Console.WriteLine(System.Globalization.DateTimeFormatInfo.CurrentInfo.FirstDayOfWeek);
            //Console.WriteLine(System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat.FirstDayOfWeek);
            //Console.WriteLine(this.ViewModel.FirstDayOfWeek);
            //Console.WriteLine(DateTimePatternDTP.FirstDayOfWeek);
            //Console.WriteLine();
        }
    }
}
