using CodeflexSystems.Library.Core;
using CodeflexSystems.Library.Extensions;
using CodeflexSystems.Library.Reflection;
using CodeflexSystems.SmartUI.Presentation;
using CodeflexSystems.SmartUI.Windows.Controls;
using CodeflexSystems.SmartUI.Windows.Input;
using CodeflexSystems.SMS.Core;
using CodeflexSystems.SMS.Data;
using CodeflexSystems.SMS.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace CodeflexSystems.SMS.ViewModels
{
    internal sealed class UserOptionsViewModel : BaseViewModel
    {
        #region Fields

        private UserOption model;
        private Color defaultAccentColor;
        private Color accentColor;
        private Link defaultTheme;
        private Link theme;
        private string dateTimePattern;

        #endregion

        #region Properties

        public ObservableCollection<Color> AccentColors { get; private set; }
        public LinkCollection Themes { get; private set; }
        public ObservableCollection<TransitionEffect> TransitionEffects { get; private set; }
        public ObservableCollection<FontSize> FontSizes { get; private set; }
        public ObservableCollection<string> DatePatterns { get; private set; }
        public ObservableCollection<string> TimePatterns { get; private set; }
        public EntityCollection<string> DateTimePatterns { get; private set; }
        public ObservableCollection<DayOfWeek> FirstDayOfWeeks { get; private set; }
        public EntityCollection<string> CurrencyPositivePatterns { get; private set; }
        public EntityCollection<string> CurrencyNegativePatterns { get; private set; }
        public ICommand SaveCommand { get; private set; }
        public ICommand ResetCommand { get; private set; }

        public Color AccentColor
        {
            get { return this.accentColor; }
            set
            {
                if (this.accentColor != value)
                {
                    this.accentColor = value;
                    this.model.AccentColorA = value.A;
                    this.model.AccentColorR = value.R;
                    this.model.AccentColorG = value.G;
                    this.model.AccentColorB = value.B;
                    this.AccentColors.AddIfNotExist(this.AccentColor);
                    AppearanceManager.Current.AccentColor = value;
                    RaisePropertyChanged("AccentColor");
                }
            }
        }

        public Link Theme
        {
            get { return this.theme; }
            set
            {
                int index = GetThemeId(value);
                if (index != -1 && this.theme != value)
                {
                    this.theme = value;
                    this.model.ThemeIndex = (byte)index;
                    // and update the actual theme
                    AppearanceManager.Current.ThemeSource = value.Source;
                    RaisePropertyChanged("Theme");
                }
            }
        }

        public TransitionEffect TransitionEffect
        {
            get { return (TransitionEffect)this.model.TransitionEffect; }
            set
            {
                if (this.model.TransitionEffect != (byte)value)
                {
                    this.model.TransitionEffect = (byte)value;
                    AppearanceManager.Current.TransitionEffect = value;
                    RaisePropertyChanged("TransitionEffect");
                }
            }
        }

        public FontSize FontSize
        {
            get { return (FontSize)this.model.FontSize; }
            set
            {
                if (this.model.FontSize != (byte)value)
                {
                    this.model.FontSize = (byte)value;
                    AppearanceManager.Current.FontSize = value;
                    RaisePropertyChanged("FontSize");
                }
            }
        }

        public int PageSize
        {
            get { return this.model.PageSize; }
            set
            {
                if (value > 9 && this.model.PageSize != value)
                {
                    this.model.PageSize = value;
                    RaisePropertyChanged("PageSize");
                }
            }
        }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Date Pattern is required.")]
        [MaxLength(30, ErrorMessage = "Date Pattern is too long.")]
        public string DatePattern
        {
            get { return this.model.DatePattern; }
            set
            {
                if (this.model.DatePattern != value)
                {
                    ValidateProperty(value, "DatePattern");
                    this.DatePatterns.AddIfNotExist(value);
                    this.model.DatePattern = value;
                    Application.Current.Resources["DatePattern"] = value;
                    RaisePropertyChanged("DatePattern");
                    SetDateTimePattern();
                }
            }
        }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Time Pattern is required.")]
        [MaxLength(20, ErrorMessage = "Time Pattern is too long.")]
        public string TimePattern
        {
            get { return this.model.TimePattern; }
            set
            {
                if (this.model.TimePattern != value)
                {
                    ValidateProperty(value, "TimePattern");
                    this.TimePatterns.AddIfNotExist(value);
                    this.model.TimePattern = value;
                    Application.Current.Resources["TimePattern"] = value;
                    RaisePropertyChanged("TimePattern");
                    SetDateTimePattern();
                }
            }
        }

        //[Required(AllowEmptyStrings = false, ErrorMessage = "Date Time Pattern is required.")]
        //[MaxLength(50, ErrorMessage = "Date Time Pattern is too long.")]
        public string DateTimePattern
        {
            get { return this.dateTimePattern; }
            private set
            {
                if (this.dateTimePattern != value)
                {
                    //ValidateProperty(value, "DateTimePattern");
                    this.dateTimePattern = value;
                    Application.Current.Resources["DateTimePattern"] = value;
                    RaisePropertyChanged("DateTimePattern");
                }
            }
        }

        public DayOfWeek FirstDayOfWeek
        {
            get { return (DayOfWeek)this.model.FirstDayOfWeek; }
            set
            {
                if (this.model.FirstDayOfWeek != (byte)value)
                {
                    this.model.FirstDayOfWeek = (byte)value;
                    DateTimeFormatInfo.CurrentInfo.FirstDayOfWeek = value;
                    RaisePropertyChanged("FirstDayOfWeek");
                }
            }
        }

        public int CurrencyPositivePattern
        {
            get { return this.model.CurrencyPositivePattern; }
            set
            {
                if (value >= 0 && value <= 3 && this.model.CurrencyPositivePattern != value)
                {
                    this.model.CurrencyPositivePattern = value;
                    //if (value != this.CurrencyPositivePatterns.SelectedIndex)
                    this.CurrencyPositivePatterns.SelectedIndex = value;
                    NumberFormatInfo.CurrentInfo.CurrencyPositivePattern = value;
                    RaisePropertyChanged("CurrencyPositivePattern");
                }
            }
        }

        public int CurrencyNegativePattern
        {
            get { return this.model.CurrencyNegativePattern; }
            set
            {
                if (value >= 0 && value <= 15 && this.model.CurrencyNegativePattern != value)
                {
                    this.model.CurrencyNegativePattern = value;
                    //if (value != this.CurrencyNegativePatterns.SelectedIndex)
                    this.CurrencyNegativePatterns.SelectedIndex = value;
                    NumberFormatInfo.CurrentInfo.CurrencyNegativePattern = value;
                    RaisePropertyChanged("CurrencyNegativePattern");
                }
            }
        }

        public bool IsDefault
        {
            get { return IsDummy(this.model.UserId); }
        }

        #endregion

        #region Ctor, Dtor

        public UserOptionsViewModel()
        {
            Initialize();
            LoadDefault();
        }

        #endregion

        #region Methods

        private void Initialize()
        {
            this.ViewName = "User Options";
            if (Thread.CurrentThread.CurrentCulture.IsReadOnly)
            {
                var ci = CultureInfo.CreateSpecificCulture(CultureInfo.CurrentCulture.Name);

                Thread.CurrentThread.CurrentCulture = ci;
                Thread.CurrentThread.CurrentUICulture = ci;
            }

            this.AccentColors = new ObservableCollection<Color>();
            this.AccentColors.AddRange(
                new Color[]
                {
                    Color.FromRgb(0xa4, 0xc4, 0x00),   // lime
                    Color.FromRgb(0x60, 0xa9, 0x17),   // green
                    Color.FromRgb(0x00, 0x8a, 0x00),   // emerald
                    Color.FromRgb(0x00, 0xab, 0xa9),   // teal
                    Color.FromRgb(0x1b, 0xa1, 0xe2),   // cyan
                    Color.FromRgb(0x00, 0x50, 0xef),   // cobalt
                    Color.FromRgb(0x6a, 0x00, 0xff),   // indigo
                    Color.FromRgb(0xaa, 0x00, 0xff),   // violet
                    Color.FromRgb(0xf4, 0x72, 0xd0),   // pink
                    Color.FromRgb(0xd8, 0x00, 0x73),   // magenta
                    Color.FromRgb(0xa2, 0x00, 0x25),   // crimson
                    Color.FromRgb(0xe5, 0x14, 0x00),   // red
                    Color.FromRgb(0xfa, 0x68, 0x00),   // orange
                    Color.FromRgb(0xf0, 0xa3, 0x0a),   // amber
                    Color.FromRgb(0xe3, 0xc8, 0x00),   // yellow
                    Color.FromRgb(0x82, 0x5a, 0x2c),   // brown
                    Color.FromRgb(0x6d, 0x87, 0x64),   // olive
                    Color.FromRgb(0x64, 0x76, 0x87),   // steel
                    Color.FromRgb(0x76, 0x60, 0x8a),   // mauve
                    Color.FromRgb(0x87, 0x79, 0x4e),   // taupe
                    Color.FromRgb(0x00, 0x82, 0xad),
                });

            this.Themes = new LinkCollection();
            // add the default themes
            this.Themes.Add(new Link { DisplayName = "Dark", Source = AppearanceManager.DarkThemeSource });
            this.Themes.Add(new Link { DisplayName = "Light", Source = AppearanceManager.LightThemeSource });
            // add additional themes
            this.Themes.Add(new Link { DisplayName = "Hello Kitty", Source = new Uri("/SMS;Component/Assets/ModernUI.HelloKitty.xaml", UriKind.Relative) });
            this.Themes.Add(new Link { DisplayName = "Love", Source = new Uri("/SMS;Component/Assets/ModernUI.Love.xaml", UriKind.Relative) });
            this.Themes.Add(new Link { DisplayName = "Snowflakes", Source = new Uri("/SMS;Component/Assets/ModernUI.Snowflakes.xaml", UriKind.Relative) });
            this.Themes.Add(new Link { DisplayName = "Flying Cat", Source = new Uri("/SMS;Component/Assets/ModernUI.FlyingCat.xaml", UriKind.Relative) });

            this.TransitionEffects = new ObservableCollection<TransitionEffect>();
            this.TransitionEffects.AddRange(ReflectionHelper.ExtractEnumValues<TransitionEffect>().Where(m => m != TransitionEffect.Custom));

            this.FontSizes = new ObservableCollection<FontSize>();
            this.FontSizes.AddRange(ReflectionHelper.ExtractEnumValues<FontSize>());

            this.DatePatterns = new ObservableCollection<string>();
            this.DatePatterns.AddRange(SR.GetDateFormats());

            this.TimePatterns = new ObservableCollection<string>();
            this.TimePatterns.AddRange(SR.GetTimeFormats());

            this.DateTimePatterns = new EntityCollection<string>();
            this.DateTimePatterns.AddRange(SR.GetDateTimeFormats(), false);
            this.DateTimePatterns.SelectionChanged += (s, e) =>
            {
                if (this.DateTimePatterns.SelectedIndex >= 0 &&
                    this.model != null)
                    this.model.DateTimeIndex = (byte)this.DateTimePatterns.SelectedIndex;
                SetDateTimePattern();
            };

            this.FirstDayOfWeeks = new ObservableCollection<DayOfWeek>();
            this.FirstDayOfWeeks.AddRange(ReflectionHelper.ExtractEnumValues<DayOfWeek>());

            this.CurrencyPositivePatterns = new EntityCollection<string>();
            this.CurrencyPositivePatterns.AddRange(
                new string[] {
                    "$n", "n$", "$ n", "n $"
                }, false);
            this.CurrencyPositivePatterns.SelectionChanged += (s, e) =>
            {
                this.CurrencyPositivePattern = this.CurrencyPositivePatterns.SelectedIndex;
            };

            this.CurrencyNegativePatterns = new EntityCollection<string>();
            this.CurrencyNegativePatterns.AddRange(
                new string[]{
                    "($n)", "-$n", "$-n", "$n-", "(n$)", 
                    "-n$", "n-$", "n$-", "-n $", "-$ n",
                    "n $-", "$ n-", "$ -n", "n- $", "($ n)",
                    "(n $)" 
                }, false);
            this.CurrencyNegativePatterns.SelectionChanged += (s, e) =>
            {
                this.CurrencyNegativePattern = this.CurrencyNegativePatterns.SelectedIndex;
            };

            this.defaultAccentColor = AppearanceManager.Current.AccentColor;
            this.defaultTheme = GetCurrentTheme();

            this.PropertyChanged += (s, e) =>
            {
                this.CanSave = !this.HasErrors;
            };
            AppearanceManager.Current.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == "ThemeSource" || e.PropertyName == "AccentColor")
                    SyncThemeAndColor();
            };
            this.SaveCommand = new RelayCommand(o => OnSave(), o => this.CanSave);
            this.ResetCommand = new RelayCommand(o => Reset());
            this.IsLoaded = true;
        }

        private void SyncThemeAndColor()
        {
            // synchronizes the selected viewmodel theme with the actual theme used by the appearance manager.
            this.Theme = GetCurrentTheme();

            // and make sure accent color is up-to-date
            this.AccentColor = AppearanceManager.Current.AccentColor;
        }

        private Link GetCurrentTheme()
        {
            return this.Themes.FirstOrDefault(l => string.Equals(l.Source.ToString(), AppearanceManager.Current.ThemeSource.ToString(), StringComparison.InvariantCultureIgnoreCase));
        }

        private int GetThemeId(Link value)
        {
            if (value.DisplayName == "Dark")
                return 1;
            if (value.DisplayName == "Light")
                return 2;
            if (value.DisplayName == "Hello Kitty")
                return 101;
            if (value.DisplayName == "Love")
                return 102;
            if (value.DisplayName == "Snowflakes")
                return 103;
            if (value.DisplayName == "Flying Cat")
                return 104;

            return -1;
        }

        private Link GetTheme(int index)
        {
            string name = null;

            if (index == 1)
                name = "Dark";
            else if (index == 2)
                name = "Light";
            else if (index == 101)
                name = "Hello Kitty";
            else if (index == 102)
                name = "Love";
            else if (index == 103)
                name = "Snowflakes";
            else if (index == 104)
                name = "Flying Cat";

            Link value = null;
            if (name != null)
                value = this.Themes.FirstOrDefault(m => m.DisplayName == name);

            if (value == null)
                value = this.Themes.First();

            return value;
        }

        private void SetDateTimePattern()
        {
            int index = this.DateTimePatterns.SelectedIndex;
            var dtfi = DateTimeFormatInfo.CurrentInfo;

            if (index < 0)
                return;
            else if (index == 0)
                this.DateTimePattern = string.Format("{0} {1}", dtfi.ShortDatePattern, dtfi.ShortTimePattern);
            else if (index == 1)
                this.DateTimePattern = string.Format("{0} {1}", dtfi.ShortDatePattern, dtfi.LongTimePattern);
            else if (index == 2)
                this.DateTimePattern = string.Format("{0} {1}", dtfi.LongDatePattern, dtfi.ShortTimePattern);
            else if (index == 3)
                this.DateTimePattern = string.Format("{0} {1}", dtfi.LongDatePattern, dtfi.LongTimePattern);
            else if (index == 4)
                this.DateTimePattern = dtfi.RFC1123Pattern;
            else if (index == 5)
                this.DateTimePattern = dtfi.SortableDateTimePattern;
            else if (index == 6)
                this.DateTimePattern = dtfi.UniversalSortableDateTimePattern;
            else
                this.DateTimePattern = string.Format("{0} {1}", this.DatePattern, this.TimePattern);
        }

        private void InternalReset()
        {
            var dtfi = DateTimeFormatInfo.CurrentInfo;

            this.PropertyChangedEnabled = false;
            this.AccentColor = this.defaultAccentColor;
            this.Theme = this.defaultTheme;
            this.FontSize = FontSize.Large;
            this.TransitionEffect = TransitionEffect.ModernUI;
            this.PageSize = 20;
            //this.DatePattern = dtfi.LongDatePattern;
            this.DatePattern = "d/M/yyyy";
            this.TimePattern = dtfi.ShortTimePattern;
            this.DateTimePatterns.SelectedIndex = 1;
            this.FirstDayOfWeek = dtfi.FirstDayOfWeek;
            this.CurrencyPositivePattern = 3;
            this.CurrencyNegativePattern = 8;
            if (this.HasErrors)
                this.ValidateErrors();
            this.PropertyChangedEnabled = true;
            RaisePropertyChanged(propertyName: null);
        }

        public void Load(UserOption m)
        {
            if (m == null)
            {
                Load(m.UserId);
                return;
            }

            try
            {
                this.PropertyChangedEnabled = false;
                if (!this.CurrencyPositivePatterns.IsInRange(m.CurrencyPositivePattern))
                    m.CurrencyPositivePattern = this.CurrencyPositivePatterns.GetValidIndex(m.CurrencyPositivePattern);
                if (!this.CurrencyNegativePatterns.IsInRange(m.CurrencyNegativePattern))
                    m.CurrencyNegativePattern = this.CurrencyNegativePatterns.GetValidIndex(m.CurrencyNegativePattern);

                this.model = m;
                this.AccentColor = Color.FromArgb(m.AccentColorA, m.AccentColorR, m.AccentColorG, m.AccentColorB);
                //this.AccentColors.AddIfNotExist(this.AccentColor);
                this.Theme = GetTheme(this.model.ThemeIndex);
                if (!this.FontSizes.Contains(this.FontSize))
                    this.FontSize = FontSize.Large;
                if (!this.TransitionEffects.Contains(this.TransitionEffect))
                    this.TransitionEffect = TransitionEffect.ModernUI;
                if (this.PageSize < 10)
                    this.PageSize = 20;
                this.DatePatterns.AddIfNotExist(this.DatePattern);
                this.TimePatterns.AddIfNotExist(this.TimePattern);
                this.DateTimePatterns.SelectedIndex = this.model.DateTimeIndex;
                if (this.HasErrors)
                    this.ValidateErrors();
                this.CanSave = false;
                this.PropertyChangedEnabled = true;
                RaisePropertyChanged(propertyName: null);
            }
            catch (Exception ex)
            {
                DialogManager.ShowErrorAndLog("Load Options", ex);
            }
        }

        public void Load(Guid userid)
        {
            try
            {
                this.model = new UserOption { UserId = userid };
                InternalReset();
                this.CanSave = false;
            }
            catch (Exception ex)
            {
                DialogManager.ShowErrorAndLog("Load Options", ex);
            }
        }

        public void LoadDefault()
        {
            try
            {
                this.model = new UserOption();
                InternalReset();
                this.CanSave = false;
            }
            catch (Exception ex)
            {
                DialogManager.ShowErrorAndLog("Load Default Options", ex);
            }
        }

        public void Reset()
        {
            try
            {
                InternalReset();
            }
            catch (Exception ex)
            {
                DialogManager.ShowErrorAndLog("Reset Options", ex);
            }
        }

        public void OnSave()
        {
            if (this.model.UserId == Guid.Empty)
                return;
            if (this.HasErrors)
            {
                DialogManager.ShowError("Validate Data", SR.GetString("ValidateData1", "application options"));
                return;
            }

            try
            {
                using (var uow = ApplicationManager.Resolve<ISMSUnitOfWork>())
                {
                    var uo = uow.FirstOrDefault<UserOption>(m => m.UserId == this.model.UserId);
                    if (uo == null)
                        uow.Add<UserOption>(this.model);
                    else
                    {
                        uo.AccentColorA = this.model.AccentColorA;
                        uo.AccentColorR = this.model.AccentColorR;
                        uo.AccentColorG = this.model.AccentColorG;
                        uo.AccentColorB = this.model.AccentColorB;
                        uo.ThemeIndex = this.model.ThemeIndex;
                        uo.FontSize = this.model.FontSize;
                        uo.TransitionEffect = this.model.TransitionEffect;
                        uo.PageSize = this.model.PageSize;
                        uo.DatePattern = this.model.DatePattern;
                        uo.TimePattern = this.model.TimePattern;
                        uo.DateTimeIndex = this.model.DateTimeIndex;
                        uo.FirstDayOfWeek = this.model.FirstDayOfWeek;
                        uo.CurrencyPositivePattern = this.model.CurrencyPositivePattern;
                        uo.CurrencyNegativePattern = this.model.CurrencyNegativePattern;
                    }
                    uow.SaveChanges();
                    this.CanSave = false;
                }
            }
            catch (Exception ex)
            {
                DialogManager.ShowErrorAndLog("Save Options", ex);
            }
        }

        #endregion
    }
}
