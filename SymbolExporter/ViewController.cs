using System;
using System.IO;

using AppKit;
using CoreGraphics;
using Foundation;

namespace SymbolExporter
{
    public partial class ViewController : NSViewController
    {
        private const int _side = 200;

        private NSImageView _imageView;
        private NSTextField _weightTextField;
        private NSTextField _widthTextField;
        private NSTextField _heightTextField;
        private NSTextField _symbolNameTextField;
        private NSTextField _symbolExportPlaceTextField;
        private NSButton _lockButton;
        private NSSlider _weightSlider;

        private readonly SymbolViewModel _symbol;

        private NSOpenPanel _openPanel;

        public ViewController(IntPtr handle) : base(handle)
        {
            var documents = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            _symbol = new SymbolViewModel("circle", 50, 50, 0, documents, true);
            _symbol.PropertyChanged += _symbol_PropertyChanged;
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // Do any additional setup after loading the view.
            SetupView();
        }

        public override void ViewDidAppear()
        {
            base.ViewDidAppear();   
        }

        private void SetupView()
        {
            _weightTextField = new NSTextField();
            _weightTextField.Editable = false;
            _weightTextField.Bordered = false;
            _weightTextField.StringValue = $"Weight: {_symbol.Weight}";
            _weightTextField.BackgroundColor = NSColor.Clear;
            _weightTextField.TranslatesAutoresizingMaskIntoConstraints = false;

            View.AddSubview(_weightTextField);

            var configuration = NSImageSymbolConfiguration.Create(_side, _symbol.Weight);
            _imageView = new NSImageView();
            _imageView.SymbolConfiguration = configuration;

            _imageView.TranslatesAutoresizingMaskIntoConstraints = false;
            _imageView.Image = NSImage.GetSystemSymbol(_symbol.Name, null); ;
            _imageView.ContentTintColor = NSColor.Black;
            View.AddSubview(_imageView);

            var cs = new[] {
                _imageView.TopAnchor.ConstraintEqualToAnchor(View.TopAnchor, 20),
                _imageView.LeftAnchor.ConstraintEqualToAnchor(View.LeftAnchor, 20),
                _imageView.WidthAnchor.ConstraintEqualToConstant(_side),
                _imageView.HeightAnchor.ConstraintEqualToConstant(_side),
            };
            NSLayoutConstraint.ActivateConstraints(cs);


            var nameTitle = new NSTextField();
            nameTitle.StringValue = "Name:";
            nameTitle.Editable = false;
            nameTitle.Bordered = false;
            nameTitle.Alignment = NSTextAlignment.Right;
            nameTitle.BackgroundColor = NSColor.Clear;
            nameTitle.TranslatesAutoresizingMaskIntoConstraints = false;
            View.AddSubview(nameTitle);

            _symbolNameTextField = new NSTextField();
            _symbolNameTextField.PlaceholderString = "Symbol Name";
            //_symbolNameTextTield.Editable = false;
            //_symbolNameTextTield.Bordered = false;
            _symbolNameTextField.StringValue = _symbol.Name;
            _symbolNameTextField.Alignment = NSTextAlignment.Center;
            _symbolNameTextField.Changed += _symbolNameTextField_Changed;
            _symbolNameTextField.EditingEnded += _symbolNameTextTield_EditingEnded;
            _symbolNameTextField.TranslatesAutoresizingMaskIntoConstraints = false;

            View.AddSubview(_symbolNameTextField);

            cs = new[] {
                nameTitle.LeadingAnchor.ConstraintEqualToAnchor(_imageView.TrailingAnchor, 20),
                nameTitle.TopAnchor.ConstraintEqualToAnchor(View.TopAnchor, 20),

                _symbolNameTextField.CenterYAnchor.ConstraintEqualToAnchor(nameTitle.CenterYAnchor),
                _symbolNameTextField.LeadingAnchor.ConstraintEqualToAnchor(nameTitle.TrailingAnchor),
                _symbolNameTextField.TrailingAnchor.ConstraintEqualToAnchor(View.TrailingAnchor, -20),
                //_symbolNameTextTield.WidthAnchor.ConstraintEqualToConstant(250),
            };

            NSLayoutConstraint.ActivateConstraints(cs);

            _weightSlider = new NSSlider();
            _weightSlider.MinValue = -4;
            _weightSlider.MaxValue = 4;
            _weightSlider.Title = "Weight";
            _weightSlider.IntValue = 0;
            _weightSlider.Activated += Slider_Activated;
            _weightSlider.TranslatesAutoresizingMaskIntoConstraints = false;
            View.AddSubview(_weightSlider);

            cs = new[]
            {
                _weightTextField.LeadingAnchor.ConstraintEqualToAnchor(nameTitle.LeadingAnchor),
                //_weightTextField.TopAnchor.ConstraintEqualToAnchor(_symbolNameTextField.BottomAnchor, 10),
                _weightTextField.WidthAnchor.ConstraintEqualToConstant(100),
                _weightTextField.CenterYAnchor.ConstraintEqualToAnchor(_weightSlider.CenterYAnchor),

                _weightSlider.TopAnchor.ConstraintEqualToAnchor(_symbolNameTextField.BottomAnchor, 10),
                _weightSlider.LeadingAnchor.ConstraintEqualToAnchor(_weightTextField.TrailingAnchor, 10),
                _weightSlider.TrailingAnchor.ConstraintEqualToAnchor(_symbolNameTextField.TrailingAnchor),
                //_weightSlider.HeightAnchor.ConstraintEqualToConstant(60),
            };
            NSLayoutConstraint.ActivateConstraints(cs);

            NSButton exportButton = new NSButton();
            exportButton.BezelStyle = NSBezelStyle.RegularSquare;
            exportButton.Title = "Export";
            exportButton.TranslatesAutoresizingMaskIntoConstraints = false;
            exportButton.Activated += ExportButton_Activated;

            View.AddSubview(exportButton);

            cs = new[] {
                exportButton.BottomAnchor.ConstraintEqualToAnchor(View.BottomAnchor, -20),
                exportButton.RightAnchor.ConstraintEqualToAnchor(View.RightAnchor, -20),
                exportButton.WidthAnchor.ConstraintEqualToConstant(50),
                exportButton.HeightAnchor.ConstraintEqualToConstant(50),
            };

            NSLayoutConstraint.ActivateConstraints(cs);

            NSButton chooseButton = new NSButton();
            chooseButton.BezelStyle = NSBezelStyle.RegularSquare;
            chooseButton.Title = "Choose...";
            chooseButton.TranslatesAutoresizingMaskIntoConstraints = false;
            chooseButton.Activated += Button1_Activated;
            View.AddSubview(chooseButton);

            //cs = new[] {
            //    chooseButton.BottomAnchor.ConstraintEqualToAnchor(View.BottomAnchor, -20),
            //    chooseButton.LeftAnchor.ConstraintEqualToAnchor(nameTitle.LeadingAnchor, -20),
            //chooseButton.WidthAnchor.ConstraintEqualToConstant(50),
            //chooseButton.HeightAnchor.ConstraintEqualToConstant(50),
            //};

            //NSLayoutConstraint.ActivateConstraints(cs);


            var exportPathTitle = new NSTextField();
            exportPathTitle.StringValue = "Where:";
            exportPathTitle.Editable = false;
            exportPathTitle.Bordered = false;
            exportPathTitle.Alignment = NSTextAlignment.Right;
            exportPathTitle.BackgroundColor = NSColor.Clear;
            exportPathTitle.TranslatesAutoresizingMaskIntoConstraints = false;
            View.AddSubview(exportPathTitle);

            _symbolExportPlaceTextField = new NSTextField();
            _symbolExportPlaceTextField.PlaceholderString = "Export Path";
            _symbolExportPlaceTextField.Editable = false;
            _symbolExportPlaceTextField.TextColor = NSColor.SystemGrayColor;
            _symbolExportPlaceTextField.Bordered = false;
            _symbolExportPlaceTextField.StringValue = _symbol.Path;
            //_symbolExportPlaceTextField.StringValue = _symbol.Name;
            _symbolExportPlaceTextField.Alignment = NSTextAlignment.Left;
            _symbolExportPlaceTextField.EditingEnded += _symbolNameTextTield_EditingEnded;
            _symbolExportPlaceTextField.TranslatesAutoresizingMaskIntoConstraints = false;

            View.AddSubview(_symbolExportPlaceTextField);

            cs = new[] {
                exportPathTitle.LeadingAnchor.ConstraintEqualToAnchor(nameTitle.LeadingAnchor),
                exportPathTitle.TopAnchor.ConstraintEqualToAnchor(_weightTextField.BottomAnchor, 10),

                _symbolExportPlaceTextField.LeadingAnchor.ConstraintEqualToAnchor(exportPathTitle.TrailingAnchor),
                _symbolExportPlaceTextField.TrailingAnchor.ConstraintEqualToAnchor(chooseButton.LeadingAnchor, -10),
                //_symbolExportPlaceTextField.TopAnchor.ConstraintEqualToAnchor(_symbolNameTextField.BottomAnchor, 10),
                _symbolExportPlaceTextField.CenterYAnchor.ConstraintEqualToAnchor(exportPathTitle.CenterYAnchor),

                chooseButton.CenterYAnchor.ConstraintEqualToAnchor(_symbolExportPlaceTextField.CenterYAnchor),
                chooseButton.TrailingAnchor.ConstraintEqualToAnchor(_symbolNameTextField.TrailingAnchor),
                chooseButton.WidthAnchor.ConstraintEqualToConstant(80),
            };
            NSLayoutConstraint.ActivateConstraints(cs);

            _widthTextField = new NSTextField();
            _widthTextField.PlaceholderString = "Width";
            _widthTextField.StringValue = _symbol.Width.ToString();
            _widthTextField.Alignment = NSTextAlignment.Center;
            _widthTextField.EditingEnded += _widthTextField_EditingEnded;
            _widthTextField.TranslatesAutoresizingMaskIntoConstraints = false;
            View.AddSubview(_widthTextField);

            _heightTextField = new NSTextField();
            _heightTextField.PlaceholderString = "Height";
            _heightTextField.StringValue = _symbol.Height.ToString();
            _heightTextField.Alignment = NSTextAlignment.Center;
            _heightTextField.EditingEnded += _heightTextField_EditingEnded;
            _heightTextField.TranslatesAutoresizingMaskIntoConstraints = false;
            View.AddSubview(_heightTextField);

            var widthTitle = new NSTextField();
            widthTitle.StringValue = "Width:";
            widthTitle.Editable = false;
            widthTitle.Bordered = false;
            widthTitle.Alignment = NSTextAlignment.Right;
            widthTitle.BackgroundColor = NSColor.Clear;
            widthTitle.TranslatesAutoresizingMaskIntoConstraints = false;
            View.AddSubview(widthTitle);

            var heightTitle = new NSTextField();
            heightTitle.StringValue = "Height:";
            heightTitle.Editable = false;
            heightTitle.BackgroundColor = NSColor.Clear;
            heightTitle.Bordered = false;
            heightTitle.Alignment = NSTextAlignment.Right;
            heightTitle.TranslatesAutoresizingMaskIntoConstraints = false;
            View.AddSubview(heightTitle);

            _lockButton = new NSButton();
            _lockButton.Image = NSImage.GetSystemSymbol(_symbol.LockRatio ? "lock" : "lock.open", null);
            _lockButton.TranslatesAutoresizingMaskIntoConstraints = false;
            _lockButton.Activated += LockButton_Activated;
            _lockButton.Bordered = false;

            View.AddSubview(_lockButton);

            var formatTitle = new NSTextField();
            formatTitle.StringValue = "Format:";
            formatTitle.Editable = false;
            formatTitle.Bordered = false;
            formatTitle.Alignment = NSTextAlignment.Right;
            formatTitle.BackgroundColor = NSColor.Clear;
            formatTitle.TranslatesAutoresizingMaskIntoConstraints = false;
            View.AddSubview(formatTitle);

            var comboBox = new NSComboBox();
            comboBox.TranslatesAutoresizingMaskIntoConstraints = false;
            comboBox.Editable = false;
            comboBox.SelectionChanged += ComboBox_SelectionChanged;
            View.AddSubview(comboBox);

            var names = Enum.GetNames(typeof(SymbolViewModel.ExportFormat));
            foreach (var name in names)
                comboBox.Add(new NSString(name));

            comboBox.SelectItem(0);

            var switchTitle = new NSTextField();
            switchTitle.StringValue = "Options:";
            switchTitle.Editable = false;
            switchTitle.Bordered = false;
            switchTitle.Alignment = NSTextAlignment.Right;
            switchTitle.BackgroundColor = NSColor.Clear;
            switchTitle.TranslatesAutoresizingMaskIntoConstraints = false;
            View.AddSubview(switchTitle);


            var switchButton = new NSButton();
            switchButton.Title = "Generate as two scale factor.(@2x)";
            switchButton.SetButtonType(NSButtonType.Switch);
            switchButton.TranslatesAutoresizingMaskIntoConstraints = false;
            switchButton.Activated += SwitchButton_Activated;
            //switchButton.AllowsMixedState = true;
            switchButton.State = _symbol.GenerateAsTwoScaleFactor ? NSCellStateValue.On : NSCellStateValue.Off;
            View.AddSubview(switchButton);

            var switchButton2 = new NSButton();
            switchButton2.Title = "Generate tree scale factor at the same time.(@3x)";
            switchButton2.SetButtonType(NSButtonType.Switch);
            switchButton2.TranslatesAutoresizingMaskIntoConstraints = false;
            switchButton2.State = _symbol.GenerateThreeScaleFactor ? NSCellStateValue.On : NSCellStateValue.Off;
            switchButton2.Activated += SwitchButton2_Activated;

            View.AddSubview(switchButton2);

            cs = new[] {
                nameTitle.WidthAnchor.ConstraintEqualToAnchor(widthTitle.WidthAnchor),
                exportPathTitle.WidthAnchor.ConstraintEqualToAnchor(widthTitle.WidthAnchor),
                widthTitle.LeadingAnchor.ConstraintEqualToAnchor(nameTitle.LeadingAnchor),
                widthTitle.TopAnchor.ConstraintEqualToAnchor(chooseButton.BottomAnchor, 10),

                _widthTextField.LeadingAnchor.ConstraintEqualToAnchor(widthTitle.TrailingAnchor),
                _widthTextField.CenterYAnchor.ConstraintEqualToAnchor(widthTitle.CenterYAnchor),
                _widthTextField.WidthAnchor.ConstraintEqualToConstant(100),

                heightTitle.LeadingAnchor.ConstraintEqualToAnchor(nameTitle.LeadingAnchor),
                heightTitle.TopAnchor.ConstraintEqualToAnchor(widthTitle.BottomAnchor, 10),
                heightTitle.WidthAnchor.ConstraintEqualToAnchor(widthTitle.WidthAnchor),

                _heightTextField.LeadingAnchor.ConstraintEqualToAnchor(heightTitle.TrailingAnchor),
                _heightTextField.CenterYAnchor.ConstraintEqualToAnchor(heightTitle.CenterYAnchor),
                _heightTextField.WidthAnchor.ConstraintEqualToConstant(100),

                _lockButton.WidthAnchor.ConstraintEqualToConstant(30),
                _lockButton.HeightAnchor.ConstraintEqualToConstant(30),
                _lockButton.LeadingAnchor.ConstraintEqualToAnchor(_widthTextField.TrailingAnchor, 10),
                _lockButton.CenterYAnchor.ConstraintEqualToAnchor(widthTitle.BottomAnchor, 5),
                _lockButton.TrailingAnchor.ConstraintLessThanOrEqualToAnchor(_symbolNameTextField.TrailingAnchor),

                formatTitle.LeadingAnchor.ConstraintEqualToAnchor(nameTitle.LeadingAnchor),
                formatTitle.TopAnchor.ConstraintEqualToAnchor(heightTitle.BottomAnchor, 10),
                formatTitle.WidthAnchor.ConstraintEqualToAnchor(heightTitle.WidthAnchor),

                comboBox.WidthAnchor.ConstraintEqualToConstant(100),
                comboBox.LeadingAnchor.ConstraintEqualToAnchor(formatTitle.TrailingAnchor),
                comboBox.CenterYAnchor.ConstraintEqualToAnchor(formatTitle.CenterYAnchor),

                switchTitle.LeadingAnchor.ConstraintEqualToAnchor(nameTitle.LeadingAnchor),
                switchTitle.TopAnchor.ConstraintEqualToAnchor(formatTitle.BottomAnchor, 10),
                //switchTitle.CenterYAnchor.ConstraintEqualToAnchor(formatTitle.CenterYAnchor),
                switchTitle.WidthAnchor.ConstraintEqualToAnchor(widthTitle.WidthAnchor),
                switchButton.LeadingAnchor.ConstraintEqualToAnchor(switchTitle.TrailingAnchor),
                switchButton.CenterYAnchor.ConstraintEqualToAnchor(switchTitle.CenterYAnchor),

                switchButton2.LeadingAnchor.ConstraintEqualToAnchor(switchTitle.TrailingAnchor),
                switchButton2.TopAnchor.ConstraintEqualToAnchor(switchButton.BottomAnchor, 10),
                switchButton2.TrailingAnchor.ConstraintLessThanOrEqualToAnchor(_symbolNameTextField.TrailingAnchor),

                //comboBox.TopAnchor.ConstraintEqualToAnchor(heightTitle.BottomAnchor, 10),
                exportButton.TopAnchor.ConstraintGreaterThanOrEqualToAnchor(switchButton2.BottomAnchor, 10)
            };

            NSLayoutConstraint.ActivateConstraints(cs);
        }

        private void _symbol_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SymbolViewModel.Weight))
            {
                var configuration = NSImageSymbolConfiguration.Create(_side, _symbol.Weight);
                _imageView.SymbolConfiguration = configuration;
                _weightTextField.StringValue = $"Weight: {_symbol.Weight}";
            }
            else if (e.PropertyName == nameof(SymbolViewModel.Width))
            {
                _widthTextField.StringValue = _symbol.Width.ToString();
            }
            else if (e.PropertyName == nameof(SymbolViewModel.Height))
            {
                _heightTextField.StringValue = _symbol.Height.ToString();
            }
            else if (e.PropertyName == nameof(SymbolViewModel.Path))
            {
                _symbolExportPlaceTextField.StringValue = _symbol.Path;
            }
            else if (e.PropertyName == nameof(SymbolViewModel.Name))
            {
                LoadImage();
            }
        }

        private void _symbolNameTextField_Changed(object sender, EventArgs e)
        {
            if (!(sender is NSNotification notificationCenter) || !(notificationCenter.Object is NSTextField textField))
                return;

            if (string.IsNullOrWhiteSpace(textField.StringValue))
                return;

            if (NSImage.GetSystemSymbol(textField.StringValue, null) is NSImage image)
            {
                _imageView.Image = image;
                _symbol.Name = textField.StringValue;
            }
        }

        private void SwitchButton2_Activated(object sender, EventArgs e)
        {
            if (!(sender is NSButton button))
                return;
            var state = button.State;
            if (state == NSCellStateValue.On)
            {
                _symbol.GenerateThreeScaleFactor = true;
            }
            else if (state == NSCellStateValue.Off)
            {
                _symbol.GenerateThreeScaleFactor = false;
            }
        }

        private void SwitchButton_Activated(object sender, EventArgs e)
        {
            if (!(sender is NSButton button))
                return;

            var state = button.State;
            if (state == NSCellStateValue.On)
            {
                _symbol.GenerateAsTwoScaleFactor = true;
            }
            else if (state == NSCellStateValue.Off)
            {
                _symbol.GenerateAsTwoScaleFactor = false;
            }
        }

        private void ComboBox_SelectionChanged(object sender, EventArgs e)
        {
            if (!(sender is NSNotification notificationCenter) || !(notificationCenter.Object is NSComboBox comboBox))
                return;

            int index = (int)comboBox.SelectedIndex;
            //Enum.TryParse<SymbolViewModel.ExportFormat>()
            _symbol.Format = (SymbolViewModel.ExportFormat)index;
        }

        private void LockButton_Activated(object sender, EventArgs e)
        {
            _symbol.LockRatio = !_symbol.LockRatio;
            _lockButton.Image = NSImage.GetSystemSymbol(_symbol.LockRatio ? "lock" : "lock.open", null);

            if (_symbol.LockRatio && _symbol.Width != _symbol.Height)
                _symbol.Height = _symbol.Width;
        }

        private void _widthTextField_EditingEnded(object sender, EventArgs e)
        {
            if (!(sender is NSNotification notificationCenter) || !(notificationCenter.Object is NSTextField textField))
                return;

            if (string.IsNullOrWhiteSpace(textField.StringValue) || !nfloat.TryParse(textField.StringValue, out nfloat value))
            {
                textField.StringValue = _symbol.Width.ToString();
                return;
            }
            else
            {
                _symbol.Width = value;

                if (_symbol.LockRatio)
                    _symbol.Height = value;
            }
        }

        private void _heightTextField_EditingEnded(object sender, EventArgs e)
        {
            if (!(sender is NSNotification notificationCenter) || !(notificationCenter.Object is NSTextField textField))
                return;

            if (string.IsNullOrWhiteSpace(textField.StringValue) || !nfloat.TryParse(textField.StringValue, out nfloat value))
            {
                textField.StringValue = _symbol.Height.ToString();
                return;
            }
            else
            {
                _symbol.Height = value;

                if (_symbol.LockRatio)
                    _symbol.Width = value;
            }
        }

        private void Button1_Activated(object sender, EventArgs e)
        {
            _openPanel ??= new NSOpenPanel {
                CanChooseFiles = false,
                CanChooseDirectories = true,
                AllowsMultipleSelection = false,
            };

            _openPanel.Begin(NSSavePanelComplete);
        }

        private void NSSavePanelComplete(nint result)
        {
            if (result == 1)
            {
                var dic = _openPanel.Directory;
                _symbol.Path = dic;
            }
        }

        private void _symbolNameTextTield_EditingEnded(object sender, EventArgs e)
        {
            if (!(sender is NSNotification notificationCenter) ||
                !(notificationCenter.Object is NSTextField textField) ||
                textField.StringValue.Equals(_symbol.Name))
                return;

            if (string.IsNullOrWhiteSpace(textField.StringValue))
            {
                textField.StringValue = _symbol.Name;
                return;
            }

            _symbol.Name = textField.StringValue;
        }

        private void Slider_Activated(object sender, EventArgs e)
        {
            var slider = sender as NSSlider;
            var value = slider.IntValue * 0.25f;
            _symbol.Weight = value;
        }

        private void LoadImage()
        {
            _imageView.Image = NSImage.GetSystemSymbol(_symbol.Name, null);
        }

        private void ExportButton_Activated(object sender, EventArgs e)
        {
            _widthTextField.BecomeFirstResponder();
            _heightTextField.BecomeFirstResponder();
            _symbolNameTextField.BecomeFirstResponder();

            if (!Directory.Exists(_symbol.Path))
                return;
            var image = NSImage.GetSystemSymbol(_symbol.Name, null);
            if (image == null)
                return;

            var sourceDirectory = Path.Combine(_symbol.Path, _symbol.Name);
            if (!Directory.Exists(sourceDirectory))
                Directory.CreateDirectory(sourceDirectory);

            var fileName = _symbol.Name;

            if (_symbol.GenerateAsTwoScaleFactor)
                fileName += "@2x";

            NSBitmapImageFileType fileType;
            if (_symbol.Format == SymbolViewModel.ExportFormat.PNG)
            {
                fileName += ".png";
                fileType = NSBitmapImageFileType.Png;
            }
            else
            {
                fileName += ".jpg";
                fileType = NSBitmapImageFileType.Jpeg;
            }

            var source = Path.Combine(sourceDirectory, fileName);
            if (File.Exists(source))
            {
                NSAlert alert = new NSAlert();
                alert.AlertStyle = NSAlertStyle.Warning;
                alert.MessageText = $"“{fileName}” already exists. Do you want to replace it?";
                alert.InformativeText = "A file or folder with the same name already exists in the folder forward.fill. Replacing it will overwrite its current contents.";

                var replace = alert.AddButton("Replace");
                var cancel = alert.AddButton("Cancel");
                replace.KeyEquivalent = "\r";
                cancel.KeyEquivalent = "";

                if (alert.RunModal() != (int)NSAlertButtonReturn.First)
                    return;
            }
            var imageView = GetSFImageView(image, _symbol.Width, _symbol.Height, _symbol.Weight);
            imageView.SaveViewToPng(source, fileType);

            //image.SaveImageFile(source, _symbol.Width, _symbol.Height, fileType);

            if (_symbol.GenerateAsTwoScaleFactor && _symbol.GenerateThreeScaleFactor)
            {
                fileName = $"{_symbol.Name}@3x";

                if (_symbol.Format == SymbolViewModel.ExportFormat.PNG)
                    fileName += ".png";
                else
                    fileName += ".jpg";

                source = Path.Combine(sourceDirectory, fileName);

                if (File.Exists(source))
                {
                    NSAlert alert = new NSAlert();
                    alert.AlertStyle = NSAlertStyle.Warning;
                    alert.MessageText = $"“{fileName}” already exists. Do you want to replace it?";
                    alert.InformativeText = "A file or folder with the same name already exists in the folder forward.fill. Replacing it will overwrite its current contents.";

                    var replace = alert.AddButton("Replace");
                    var cancel = alert.AddButton("Cancel");
                    replace.KeyEquivalent = "\r";
                    cancel.KeyEquivalent = "";

                    if (alert.RunModal() != (int)NSAlertButtonReturn.First)
                        return;
                }

                imageView = GetSFImageView(image, _symbol.Width * 1.5f, _symbol.Height * 1.5f, _symbol.Weight);
                imageView.SaveViewToPng(source, fileType);
                //image.SaveImageFile(source, _symbol.Width * 1.5f, _symbol.Height * 1.5f, fileType);
            }
        }

        private NSImageView GetSFImageView(NSImage image, nfloat width, nfloat height, nfloat weight)
        {
            var min = width > height ? height : width;
            var configuration = NSImageSymbolConfiguration.Create(min, weight);

            var imageView = new NSImageView(new CGRect(0, 0, width, height));
            imageView.SymbolConfiguration = configuration;
            imageView.ContentTintColor = NSColor.Black;
            imageView.Image = image;
            return imageView;
        }

        //private NSImage Resize(NSImage inputImage, nfloat factor)
        //{
        //    var image = new NSImage(new CGSize(inputImage.Size.Width * factor, inputImage.Size.Height * factor));
        //    image.LockFocus();
        //     NSAffineTransform transform = new NSAffineTransform();
        //    transform.Scale(factor);
        //    transform.Concat();
        //    inputImage.Draw(CGPoint.Empty, CGRect.Empty, NSCompositingOperation.Copy, 1);
        //    image.UnlockFocus();

        //    image.DangerousAutorelease();

        //    return image;
        //}

        public override NSObject RepresentedObject
        {
            get
            {
                return base.RepresentedObject;
            }
            set
            {
                base.RepresentedObject = value;
                // Update the view, if already loaded.
            }
        }
    }
}
