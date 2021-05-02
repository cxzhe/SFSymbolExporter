using System;
using System.IO;

using AppKit;
using CoreGraphics;
using Foundation;

namespace SymbolExporter
{
    public partial class ViewController : NSViewController
    {
        private NSImage _image;
        private NSImageView _imageView;
        private NSTextField _weightTextField;
        private NSTextField _widthTextField;
        private NSTextField _heightTextField;
        private NSTextField _symbolNameTextField;
        private NSTextField _symbolExportPlaceTextField;
        private NSButton _lockButton;
        private NSSlider _weightSlider;
        private bool _isLock = true;

        private readonly SymbolViewModel _symbol;

        private const int _side = 200;

        public ViewController(IntPtr handle) : base(handle)
        {
            var documents = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            _symbol = new SymbolViewModel("arrowshape.turn.up.backward.2", 50, 50, 0, documents);
            _symbol.PropertyChanged += _symbol_PropertyChanged;
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

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // Do any additional setup after loading the view.

            _weightTextField = new NSTextField();
            _weightTextField.Editable = false;
            _weightTextField.Bordered = false;
            _weightTextField.StringValue = $"Weight: {_symbol.Weight}";
            _weightTextField.BackgroundColor = NSColor.Clear;
            _weightTextField.TranslatesAutoresizingMaskIntoConstraints = false;

            View.AddSubview(_weightTextField);


            var configuration = NSImageSymbolConfiguration.Create(_side, _symbol.Weight);
            _image = NSImage.GetSystemSymbol(_symbol.Name, null);
            //image.Size = new CGSize(100, 100);

            _imageView = new NSImageView();
            _imageView.SymbolConfiguration = configuration;
            _imageView.TranslatesAutoresizingMaskIntoConstraints = false;
            _imageView.Image = _image;
            View.AddSubview(_imageView);

            var cs = new[] {
                _imageView.TopAnchor.ConstraintEqualToAnchor(View.TopAnchor, 20),
                _imageView.LeftAnchor.ConstraintEqualToAnchor(View.LeftAnchor, 20),
                _imageView.WidthAnchor.ConstraintEqualToConstant(_side),
                _imageView.HeightAnchor.ConstraintEqualToConstant(_side),
            };
            NSLayoutConstraint.ActivateConstraints(cs);

            _symbolNameTextField = new NSTextField();
            _symbolNameTextField.PlaceholderString = "Symbol Name";
            //_symbolNameTextTield.Editable = false;
            //_symbolNameTextTield.Bordered = false;
            _symbolNameTextField.StringValue = _symbol.Name;
            _symbolNameTextField.Alignment = NSTextAlignment.Center;
            _symbolNameTextField.EditingEnded += _symbolNameTextTield_EditingEnded;
            _symbolNameTextField.TranslatesAutoresizingMaskIntoConstraints = false;

            View.AddSubview(_symbolNameTextField);

            cs = new[] {
                _symbolNameTextField.TopAnchor.ConstraintEqualToAnchor(View.TopAnchor, 20),
                _symbolNameTextField.LeftAnchor.ConstraintEqualToAnchor(_imageView.RightAnchor, 20),
                _symbolNameTextField.RightAnchor.ConstraintEqualToAnchor(View.RightAnchor, -20),
                //_symbolNameTextTield.WidthAnchor.ConstraintEqualToConstant(250),
            };

            NSLayoutConstraint.ActivateConstraints(cs);

            _weightSlider = new NSSlider();
            _weightSlider.MinValue = -6;
            _weightSlider.MaxValue = 6;
            _weightSlider.Title = "Weight";
            _weightSlider.IntValue = 0;
            _weightSlider.Activated += Slider_Activated;
            _weightSlider.TranslatesAutoresizingMaskIntoConstraints = false;
            View.AddSubview(_weightSlider);

            cs = new[]
            {
                _weightSlider.TopAnchor.ConstraintEqualToAnchor(_symbolNameTextField.BottomAnchor),
                _weightSlider.RightAnchor.ConstraintEqualToAnchor(_symbolNameTextField.RightAnchor),
                _weightSlider.LeftAnchor.ConstraintEqualToAnchor(_weightTextField.RightAnchor, 10),
                _weightSlider.HeightAnchor.ConstraintEqualToConstant(60),

                _weightTextField.LeftAnchor.ConstraintEqualToAnchor(_symbolNameTextField.LeftAnchor),
                _weightTextField.CenterYAnchor.ConstraintEqualToAnchor(_weightSlider.CenterYAnchor),
                _weightTextField.WidthAnchor.ConstraintEqualToConstant(100),
            };
            NSLayoutConstraint.ActivateConstraints(cs);

            NSButton button = new NSButton();
            button.Title = "Export";
            button.TranslatesAutoresizingMaskIntoConstraints = false;
            button.Activated += ExportButton_Activated;
            View.AddSubview(button);

            cs = new[] {
                button.BottomAnchor.ConstraintEqualToAnchor(View.BottomAnchor, -20),
                button.RightAnchor.ConstraintEqualToAnchor(View.RightAnchor, -20),
                button.WidthAnchor.ConstraintEqualToConstant(50),
                button.HeightAnchor.ConstraintEqualToConstant(50),
            };

            NSLayoutConstraint.ActivateConstraints(cs);

            NSButton chooseButton = new NSButton();
            chooseButton.Title = "Choose...";
            chooseButton.TranslatesAutoresizingMaskIntoConstraints = false;
            chooseButton.Activated += Button1_Activated;
            View.AddSubview(chooseButton);

            //cs = new[] {
            //    chooseButton.BottomAnchor.ConstraintEqualToAnchor(View.BottomAnchor, -20),
            //    chooseButton.LeftAnchor.ConstraintEqualToAnchor(_symbolNameTextField.LeftAnchor, -20),
                //chooseButton.WidthAnchor.ConstraintEqualToConstant(50),
                //chooseButton.HeightAnchor.ConstraintEqualToConstant(50),
            //};

            //NSLayoutConstraint.ActivateConstraints(cs);

            _symbolExportPlaceTextField = new NSTextField();
            _symbolExportPlaceTextField.PlaceholderString = "Export Path";
            _symbolExportPlaceTextField.Editable = false;
            _symbolExportPlaceTextField.Bordered = false;
            _symbolExportPlaceTextField.StringValue = _symbol.Path;
            //_symbolExportPlaceTextField.StringValue = _symbol.Name;
            _symbolExportPlaceTextField.Alignment = NSTextAlignment.Left;
            _symbolExportPlaceTextField.EditingEnded += _symbolNameTextTield_EditingEnded;
            _symbolExportPlaceTextField.TranslatesAutoresizingMaskIntoConstraints = false;

            View.AddSubview(_symbolExportPlaceTextField);

            cs = new[] {
                _symbolExportPlaceTextField.LeftAnchor.ConstraintEqualToAnchor(_symbolNameTextField.LeftAnchor),
                _symbolExportPlaceTextField.RightAnchor.ConstraintEqualToAnchor(chooseButton.LeftAnchor, -10),
                _symbolExportPlaceTextField.TopAnchor.ConstraintEqualToAnchor(_weightSlider.BottomAnchor, -10),
                chooseButton.CenterYAnchor.ConstraintEqualToAnchor(_symbolExportPlaceTextField.CenterYAnchor),
                chooseButton.RightAnchor.ConstraintEqualToAnchor(_symbolNameTextField.RightAnchor),
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
            widthTitle.Alignment = NSTextAlignment.Center;
            widthTitle.BackgroundColor = NSColor.Clear;
            widthTitle.TranslatesAutoresizingMaskIntoConstraints = false;
            View.AddSubview(widthTitle);

            var heightTitle = new NSTextField();
            heightTitle.StringValue = "Height:";
            heightTitle.Editable = false;
            heightTitle.BackgroundColor = NSColor.Clear;
            heightTitle.Bordered = false;
            heightTitle.Alignment = NSTextAlignment.Center;
            heightTitle.TranslatesAutoresizingMaskIntoConstraints = false;
            View.AddSubview(heightTitle);

            _lockButton = new NSButton();
            _lockButton.Image = NSImage.GetSystemSymbol(_isLock ? "lock" : "lock.open", null);
            _lockButton.TranslatesAutoresizingMaskIntoConstraints = false;
            _lockButton.Activated += LockButton_Activated;
            _lockButton.Bordered = false;
            
            View.AddSubview(_lockButton);

            var formatTitle = new NSTextField();
            formatTitle.StringValue = "Format:";
            formatTitle.Editable = false;
            formatTitle.Bordered = false;
            formatTitle.Alignment = NSTextAlignment.Center;
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
            switchTitle.StringValue = "As @2x";
            switchTitle.Editable = false;
            switchTitle.Bordered = false;
            switchTitle.Alignment = NSTextAlignment.Center;
            switchTitle.BackgroundColor = NSColor.Clear;
            switchTitle.TranslatesAutoresizingMaskIntoConstraints = false;
            View.AddSubview(switchTitle);


            var switchBu = new NSToggleButton();
            switchBu.TranslatesAutoresizingMaskIntoConstraints = false;
            View.AddSubview(switchBu);


            cs = new[] {
                widthTitle.LeadingAnchor.ConstraintEqualToAnchor(_symbolNameTextField.LeadingAnchor),
                widthTitle.TopAnchor.ConstraintEqualToAnchor(chooseButton.BottomAnchor, 10),

                _widthTextField.LeadingAnchor.ConstraintEqualToAnchor(widthTitle.TrailingAnchor),
                _widthTextField.CenterYAnchor.ConstraintEqualToAnchor(widthTitle.CenterYAnchor),
                _widthTextField.WidthAnchor.ConstraintEqualToConstant(200),

                heightTitle.LeadingAnchor.ConstraintEqualToAnchor(_symbolNameTextField.LeadingAnchor),
                heightTitle.TopAnchor.ConstraintEqualToAnchor(widthTitle.BottomAnchor, 10),
                heightTitle.WidthAnchor.ConstraintEqualToAnchor(widthTitle.WidthAnchor),

                _heightTextField.LeadingAnchor.ConstraintEqualToAnchor(heightTitle.TrailingAnchor),
                _heightTextField.CenterYAnchor.ConstraintEqualToAnchor(heightTitle.CenterYAnchor),
                _heightTextField.WidthAnchor.ConstraintEqualToConstant(200),

                _lockButton.WidthAnchor.ConstraintEqualToConstant(30),
                _lockButton.HeightAnchor.ConstraintEqualToConstant(30),
                _lockButton.LeadingAnchor.ConstraintEqualToAnchor(_widthTextField.TrailingAnchor, 10),
                _lockButton.CenterYAnchor.ConstraintEqualToAnchor(widthTitle.BottomAnchor, 5),


                formatTitle.LeadingAnchor.ConstraintEqualToAnchor(_symbolNameTextField.LeadingAnchor),
                formatTitle.TopAnchor.ConstraintEqualToAnchor(heightTitle.BottomAnchor, 10),
                formatTitle.WidthAnchor.ConstraintEqualToAnchor(heightTitle.WidthAnchor),

                comboBox.WidthAnchor.ConstraintEqualToConstant(100),
                comboBox.LeadingAnchor.ConstraintEqualToAnchor(formatTitle.TrailingAnchor),
                comboBox.CenterYAnchor.ConstraintEqualToAnchor(formatTitle.CenterYAnchor),

                switchTitle.LeadingAnchor.ConstraintEqualToAnchor(comboBox.TrailingAnchor),
                switchTitle.CenterYAnchor.ConstraintEqualToAnchor(formatTitle.CenterYAnchor),

                //comboBox.TopAnchor.ConstraintEqualToAnchor(heightTitle.BottomAnchor, 10),
            };

            NSLayoutConstraint.ActivateConstraints(cs);
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
            _isLock = !_isLock;
            _lockButton.Image = NSImage.GetSystemSymbol(_isLock ? "lock" : "lock.open", null);
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

                if (_isLock)
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

                if (_isLock)
                    _symbol.Width = value;
            }
        }

        NSOpenPanel openPanel;

        private void Button1_Activated(object sender, EventArgs e)
        {
            openPanel ??= new NSOpenPanel {
                CanChooseFiles = false,
                CanChooseDirectories = true,
                AllowsMultipleSelection = false,
            };

            openPanel.Begin(NSSavePanelComplete11);
        }

        private void NSSavePanelComplete11(nint result)
        {
            if (result == 1)
            {
                var dic = openPanel.Directory;
                _symbol.Path = dic;
            }
        }

        private void _symbolNameTextTield_EditingEnded(object sender, EventArgs e)
        {
            if (!(sender is NSNotification notificationCenter) || !(notificationCenter.Object is NSTextField textField))
                return;

            if (string.IsNullOrWhiteSpace(textField.StringValue))
            {
                textField.StringValue = _symbol.Name;
                return;
            }

            _symbol.Name = textField.StringValue;
            //Console.WriteLine("_symbolNameTextTield_EditingEnded");
        }

        private void Slider_Activated(object sender, EventArgs e)
        {
            var slider = sender as NSSlider;
            var value = slider.IntValue * 0.25f;
            _symbol.Weight = value;

            //var configuration = NSImageSymbolConfiguration.Create(_side, _symbol.Weight);
            //_imageView.SymbolConfiguration = configuration;
            //_weightTextField.StringValue = $"Weight: {_symbol.Weight}";
        }

        private void LoadImage()
        {
            _image = NSImage.GetSystemSymbol(_symbol.Name, null);
            _imageView.Image = _image;

            //var configuration = NSImageSymbolConfiguration.Create(_side, _symbol.Weight);
            //var image = NSImage.GetSystemSymbol("folder", configuration.Description);
            //_imageView.SymbolConfiguration = configuration;
            //_weightTextField.StringValue = $"Weight: {_symbol.Weight}";
        }
        
        private void ExportButton_Activated(object sender, EventArgs e)
        {
            if (!Directory.Exists(_symbol.Path))
                return;

            var dp = Path.Combine(_symbol.Path, _symbol.Name);
            if (Directory.Exists(dp))
                return;

            Directory.CreateDirectory(dp);
            var image = ResizeSFImage(_imageView.Image, _symbol.Height);
            var bitmapImageRep = new NSBitmapImageRep(image.AsTiff());

            NSData data;
            string fileName;
            if (_symbol.Format == SymbolViewModel.ExportFormat.PNG)
            {
                data = bitmapImageRep.RepresentationUsingTypeProperties(NSBitmapImageFileType.Png);
                fileName = $"{_symbol.Name}.png";
            }
            else
            {
                data = bitmapImageRep.RepresentationUsingTypeProperties(NSBitmapImageFileType.Jpeg);
                fileName = $"{_symbol.Name}.jpg";
            }

            var path = Path.Combine(dp, fileName);
            data.Save(path, false);

        }

        private NSImage ResizeSFImage(NSImage inputImage, nfloat side)
        {
            var ratio = inputImage.Size.Width / inputImage.Size.Height;
            var r1 = side / inputImage.Size.Width;
            var r2 = side / inputImage.Size.Height;

            nfloat factor;
            CGPoint point;

            if (r1 < r2)
            {
                factor = r1;
                var margin = (side - inputImage.Size.Height * factor) / 2 / factor;
                point = new CGPoint(0, margin);
            }
            else
            {
                factor = r2;
                var margin = (side - inputImage.Size.Width * factor) / 2 / factor;
                point = new CGPoint(margin, 0);
            }

            var image = new NSImage(new CGSize(side, side));
            image.LockFocus();
            NSAffineTransform transform = new NSAffineTransform();
            transform.Scale(factor);
            transform.Concat();
            inputImage.Draw(point, CGRect.Empty, NSCompositingOperation.Copy, 1);
            image.UnlockFocus();

            image.DangerousAutorelease();

            return image;
        }

        private NSImage Resize(NSImage inputImage, nfloat factor)
        {
            var image = new NSImage(new CGSize(inputImage.Size.Width * factor, inputImage.Size.Height * factor));
            image.LockFocus();
             NSAffineTransform transform = new NSAffineTransform();
            transform.Scale(factor);
            transform.Concat();
            inputImage.Draw(CGPoint.Empty, CGRect.Empty, NSCompositingOperation.Copy, 1);
            image.UnlockFocus();
            
            image.DangerousAutorelease();

            return image;
        }

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
