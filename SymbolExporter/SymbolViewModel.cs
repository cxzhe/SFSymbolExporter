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

        public event PropertyChangedEventHandler PropertyChanged;

        public SymbolViewModel()
        {

        }

        public SymbolViewModel(string name, nfloat width, nfloat height, nfloat weight)
        {
            _name = name;
            _width = width;
            _height = height;
            _weight = weight;
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
    }
}
