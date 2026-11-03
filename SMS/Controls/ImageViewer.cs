using CodeflexSystems.SmartUI.Windows.Controls;
using CodeflexSystems.SmartUI.Windows.Input;
using CodeflexSystems.SMS.Core;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace CodeflexSystems.SMS.Controls
{
    /// <summary>
    /// Image viewer control.
    /// </summary>
    [TemplatePart(Name = PART_ThumbnailImage, Type = typeof(Image))]
    [TemplatePart(Name = PART_ChangeImageButton, Type = typeof(ModernButton))]
    [TemplatePart(Name = PART_ResetImageButton, Type = typeof(ModernButton))]
    [TemplatePart(Name = PART_ViewImageButton, Type = typeof(ModernButton))]
    [TemplatePart(Name = PART_SaveImageButton, Type = typeof(ModernButton))]
    public class ImageViewer : Control
    {
        #region Fields

        private const string PART_ThumbnailImage = "PART_ThumbnailImage";
        private const string PART_ChangeImageButton = "PART_ChangeImageButton";
        private const string PART_ResetImageButton = "PART_ResetImageButton";
        private const string PART_ViewImageButton = "PART_ViewImageButton";
        private const string PART_SaveImageButton = "PART_SaveImageButton";

        private ImageSourceConverter converter;
        private OpenFileDialog imageOpenFileDialog;
        private SaveFileDialog imageSaveFileDialog;
        private ModernButton changeImageButton, resetImageButton, viewImageButton, saveImageButton;
        private Image thumbnailImage;
        private string[] supportedFormats;
        private bool flag, isLoaded;

        #endregion

        #region Properties

        public static readonly DependencyProperty SourceProperty;
        public static readonly DependencyProperty DefaultImageListProperty;
        public static readonly DependencyProperty DefaultImageValueProperty;
        public static readonly DependencyProperty EnlargeHeightProperty;
        public static readonly DependencyProperty EnlargeWidthProperty;
        public static readonly DependencyProperty EnlargeResizeModeProperty;

        public object Source
        {
            get { return GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        public ImageDictionary DefaultImageList
        {
            get { return GetValue(DefaultImageListProperty) as ImageDictionary; }
            set { SetValue(DefaultImageListProperty, value); }
        }

        public string DefaultImageValue
        {
            get { return GetValue(DefaultImageValueProperty) as string; }
            set { SetValue(DefaultImageValueProperty, value); }
        }

        public double EnlargeHeight
        {
            get { return (double)GetValue(EnlargeHeightProperty); }
            set { SetValue(EnlargeHeightProperty, value); }
        }

        public double EnlargeWidth
        {
            get { return (double)GetValue(EnlargeWidthProperty); }
            set { SetValue(EnlargeWidthProperty, value); }
        }

        public ResizeMode EnlargeResizeMode
        {
            get { return (ResizeMode)GetValue(EnlargeResizeModeProperty); }
            set { SetValue(EnlargeResizeModeProperty, value); }
        }

        public string OpenFileFilter
        {
            get { return this.imageOpenFileDialog.Filter; }
            set { this.imageOpenFileDialog.Filter = value; }
        }

        public string SaveFileFilter
        {
            get { return this.imageSaveFileDialog.Filter; }
            set
            {
                this.imageSaveFileDialog.Filter = value;
                GenerateSupportedFormats();
            }
        }

        public bool IsDefaultImageLoaded { get; private set; }
        public string ImageFileSizeError { get; set; }
        public long MaxImageFileSize { get; set; }

        #endregion

        #region Ctor, Dtor

        static ImageViewer()
        {
            var typeofThis = typeof(ImageViewer);
            DefaultStyleKeyProperty.OverrideMetadata(typeofThis, new FrameworkPropertyMetadata(typeofThis));
            SourceProperty = DependencyProperty.Register("Source", typeof(object), typeofThis,
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender,
                new PropertyChangedCallback(OnSourceChanged)));
            DefaultImageListProperty = DependencyProperty.Register("DefaultImageList", typeof(ImageDictionary), typeofThis);
            DefaultImageValueProperty = DependencyProperty.Register("DefaultImageValue", typeof(string), typeofThis,
                new PropertyMetadata(null, new PropertyChangedCallback(OnDefaultImageValueChanged)));
            EnlargeHeightProperty = DependencyProperty.Register("EnlargeHeight", typeof(double), typeofThis, new PropertyMetadata(300.0));
            EnlargeWidthProperty = DependencyProperty.Register("EnlargeWidth", typeof(double), typeofThis, new PropertyMetadata(300.0));
            EnlargeResizeModeProperty = DependencyProperty.Register("EnlargeResizeMode", typeof(ResizeMode), typeofThis, new PropertyMetadata(ResizeMode.NoResize));
        }

        public ImageViewer()
        {
            this.Loaded += (s, e) =>
            {
                if (this.isLoaded)
                    return;

                InitializeControls();
                EnableButtons();
                SetImage();
                this.isLoaded = true;
            };
        }

        #endregion

        #region Methods

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            bool flag1, flag2, flag3, flag4, flag5;

            this.thumbnailImage = GetTemplateChild(PART_ThumbnailImage) as Image;
            flag5 = this.thumbnailImage != null;

            this.changeImageButton = GetTemplateChild(PART_ChangeImageButton) as ModernButton;
            if (flag1 = this.changeImageButton != null)
                this.changeImageButton.Command = new RelayCommand(o => ChangeImage());

            this.resetImageButton = GetTemplateChild(PART_ResetImageButton) as ModernButton;
            if (flag2 = this.resetImageButton != null)
                this.resetImageButton.Command = new RelayCommand(o => ResetImage());

            this.viewImageButton = GetTemplateChild(PART_ViewImageButton) as ModernButton;
            if (flag3 = this.viewImageButton != null)
                this.viewImageButton.Command = new RelayCommand(o => ViewImage());

            this.saveImageButton = GetTemplateChild(PART_SaveImageButton) as ModernButton;
            if (flag4 = this.saveImageButton != null)
                this.saveImageButton.Command = new RelayCommand(o => SaveImage());

            this.flag = flag1 && flag2 && flag3 && flag4 && flag5;
        }

        private static void OnSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as ImageViewer;
            if (control != null)
                control.SetImage();
        }

        private static void OnDefaultImageValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as ImageViewer;
            if (control != null && e.NewValue != null)
                control.SetDefaultImage();
        }

        private void InitializeControls()
        {
            try
            {
                this.converter = new ImageSourceConverter();
                this.imageOpenFileDialog = new OpenFileDialog();
                this.imageSaveFileDialog = new SaveFileDialog();
                this.imageSaveFileDialog.OverwritePrompt = false;

                this.OpenFileFilter = ApplicationManager.ImageOpenFileFilter;
                this.SaveFileFilter = ApplicationManager.ImageSaveFileFilter;
                this.ImageFileSizeError = ApplicationManager.ImageFileSizeError;
                this.MaxImageFileSize = ApplicationManager.Current.MaxImageFileSize;
            }
            catch (Exception ex)
            {
                DialogManager.ShowErrorAndLog("Initialize", ex);
            }
        }

        private void EnableButtons()
        {
            if (!this.flag)
                return;

            bool flag = this.Source != null;

            this.resetImageButton.IsEnabled = flag;
            this.viewImageButton.IsEnabled = flag;
            this.saveImageButton.IsEnabled = flag;
        }

        private void SetDefaultImage()
        {
            if (!this.flag)
                return;

            try
            {
                if (this.Source != null)
                    return;

                if (this.DefaultImageList == null || string.IsNullOrEmpty(this.DefaultImageValue))
                    this.thumbnailImage.Source = null;
                else
                {
                    if (this.DefaultImageList.ContainsKey(this.DefaultImageValue))
                    {
                        this.thumbnailImage.Source = this.DefaultImageList[this.DefaultImageValue].Source;
                        this.IsDefaultImageLoaded = true;
                    }
                }
                EnableButtons();
            }
            catch (Exception ex)
            {
                DialogManager.ShowErrorAndLog("Set Default Image", ex);
            }
        }

        private void SetImage()
        {
            if (!this.flag)
                return;

            try
            {
                if (this.Source == null)
                    this.SetDefaultImage();
                else
                {
                    if (this.Source is string)
                    {
                        BitmapImage bi = new BitmapImage(new Uri(this.Source as string, UriKind.RelativeOrAbsolute));
                        this.thumbnailImage.Source = bi;
                    }
                    else if (this.Source is ImageSource)
                        this.thumbnailImage.Source = this.Source as ImageSource;
                    else
                        this.thumbnailImage.Source = converter.ConvertFrom(this.Source) as ImageSource;
                    this.IsDefaultImageLoaded = false;
                    EnableButtons();
                }
            }
            catch (Exception ex)
            {
                DialogManager.ShowErrorAndLog("Set Image", ex);
            }
        }

        private void GenerateSupportedFormats()
        {
            var query = from s in this.imageSaveFileDialog.Filter.Split(new char[] { '|' })
                        where s.StartsWith("*")
                        select s.Contains(';') ? s.Substring(1, s.IndexOf(';') - 1) : s.Substring(1);
            this.supportedFormats = query.ToArray();
        }

        private void ChangeImage()
        {
            try
            {
                if (this.imageOpenFileDialog.ShowDialog() == true)
                {
                    var fi = new FileInfo(this.imageOpenFileDialog.FileName);

                    if (this.MaxImageFileSize > 0 && fi.Length > this.MaxImageFileSize)
                    {
                        var message = this.ImageFileSizeError ?? "Image file size is too big.";
                        DialogManager.ShowError("File Size", message);
                    }
                    else
                    {
                        byte[] bs = new byte[fi.Length];

                        using (Stream s = this.imageOpenFileDialog.OpenFile())
                        {
                            s.Read(bs, 0, (int)fi.Length);
                            s.Close();
                        }
                        this.Source = bs;
                    }
                }
            }
            catch (Exception ex)
            {
                DialogManager.ShowErrorAndLog("Change Image", ex);
            }
        }

        private void ResetImage()
        {
            if (this.Source == null)
                return;

            try
            {
                this.Source = null;
            }
            catch (Exception ex)
            {
                DialogManager.ShowErrorAndLog("Reset Image", ex);
            }
        }

        private void ViewImage()
        {
            if (this.Source == null)
                return;

            try
            {
                NavigationManager.ShowEnlargeImage(this.Source, this);
            }
            catch (Exception ex)
            {
                DialogManager.ShowErrorAndLog("View Image", ex);
            }
        }

        private void SaveImage()
        {
            if (this.Source == null)
                return;

            try
            {
                BitmapSource bs = this.thumbnailImage.Source as BitmapSource;
                if (bs == null)
                    return;
                if (this.imageSaveFileDialog.ShowDialog() == false)
                    return;

                FileInfo fi = new FileInfo(this.imageSaveFileDialog.FileName);
                if (!this.supportedFormats.Any(t => fi.Extension.Equals(t)))
                {
                    DialogManager.ShowError("Format Error", CodeflexSystems.SMS.Core.SR.GetString("SaveFileError2", fi.Extension));
                    return;
                }
                if (fi.Exists)
                {
                    if (DialogManager.AskConfirmation("Confirmation",
                        CodeflexSystems.SMS.Core.SR.GetString("SaveFileAsk1", this.imageSaveFileDialog.FileName),
                        CodeflexSystems.SMS.Core.SR.GetString("SaveFileAsk2"), DialogButton.OKCancel) == DialogResult.Cancel)
                        return;
                }

                bool canSave = false;
                BitmapEncoder encoder;

                if (fi.Extension == ".jpg")
                    encoder = new JpegBitmapEncoder();
                else if (fi.Extension == ".bmp")
                    encoder = new BmpBitmapEncoder();
                else if (fi.Extension == ".gif")
                    encoder = new GifBitmapEncoder();
                else if (fi.Extension == ".png")
                    encoder = new PngBitmapEncoder();
                else if (fi.Extension == ".tif")
                    encoder = new TiffBitmapEncoder();
                else
                    encoder = new WmpBitmapEncoder();

                using (Stream s = this.imageSaveFileDialog.OpenFile())
                {
                    encoder.Frames.Add(BitmapFrame.Create(bs));
                    encoder.Save(s);
                    s.Close();
                    canSave = true;
                }

                if (canSave)
                    DialogManager.Show("Save Successfully", string.Format("Saved to {0}", this.imageSaveFileDialog.FileName));
                else
                    DialogManager.ShowError("Save Failed", CodeflexSystems.SMS.Core.SR.GetString("SaveFileError1"));
            }
            catch (Exception ex)
            {
                DialogManager.ShowErrorAndLog("Save Image", ex);
            }
        }

        #endregion
    }
}
