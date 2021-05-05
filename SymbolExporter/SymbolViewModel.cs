using System;
using System.ComponentModel;


namespace SymbolExporter
{
    public class SymbolViewModel : INotifyPropertyChanged
    {
        private string _name;
        private nfloat _width;
        private nfloat _height;
        private nfloat _weight;
        private string _path;
        private ExportFormat _exportFormat = ExportFormat.PNG;
        private bool _lockRatio;
        private bool _generateAsTwoScaleFactor = true;
        private bool _generateThreeScaleFactor = true;

        public event PropertyChangedEventHandler PropertyChanged;

        public SymbolViewModel()
        {

        }

        public SymbolViewModel(string name, nfloat width, nfloat height, nfloat weight, string path, bool lockRatio)
        {
            _name = name;
            _width = width;
            _height = height;
            _weight = weight;
            _path = path;
            _lockRatio = lockRatio;
        }

        public string Name
        {
            get => _name;
            set{
                if (value != this._name)
                {
                    _name = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Name)));
                }
            }
        }

        public nfloat Width
        {
            get => _width;
            set
            {
                if (value != this._width)
                {
                    _width = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Width)));
                }
            }
        }
        public nfloat Height
        {
            get => _height;
            set
            {
                if (value != this._height)
                {
                    _height = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Height)));
                }
            }
        }
        public nfloat Weight
        {
            get => _weight;
            set
            {
                if (value != this._weight)
                {
                    _weight = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Weight)));
                }
            }
        }

        public string Path
        {
            get => _path;
            set
            {
                if (value != _path)
                {
                    _path = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Path)));
                }
            }
        }

        public ExportFormat Format
        {
            get => _exportFormat;
            set
            {
                if (value != _exportFormat)
                {
                    _exportFormat = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Format)));
                }
            }
        }

        public bool LockRatio
        {
            get => _lockRatio;
            set
            {
                if (value != _lockRatio)
                {
                    _lockRatio = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LockRatio)));
                }
            }
        }

        public bool GenerateAsTwoScaleFactor
        {
            get => _generateAsTwoScaleFactor;
            set
            {
                if (value != _generateAsTwoScaleFactor)
                {
                    _generateAsTwoScaleFactor = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(GenerateAsTwoScaleFactor)));
                }
            }
        }

        public bool GenerateThreeScaleFactor
        {
            get => _generateThreeScaleFactor;
            set
            {
                if (value != _generateThreeScaleFactor)
                {
                    _generateThreeScaleFactor = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(GenerateThreeScaleFactor)));
                }
            }
        }

        public enum ExportFormat
        {
            PNG = 0,
            JPG,
        }
    }
}
