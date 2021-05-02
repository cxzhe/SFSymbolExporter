using System;
using System.Linq;
using AppKit;
using Foundation;

namespace SymbolExporter
{
    public class FormatDataSource : NSComboBoxDataSource
    {
        public NSString[] _items = new NSString[] { new NSString("PNG"), new NSString("JPG"), };
        public override nint ItemCount(NSComboBox comboBox)
        {
            return _items.Length;
        }

        public override nint IndexOfItem(NSComboBox comboBox, string value)
        {
            for (int i = 0; i < _items.Length; i++)
            {


            }
            return 1;
        }
        public override string CompletedString(NSComboBox comboBox, string uncompletedString)
        {
            return "123";
        }

        public override NSObject ObjectValueForItem(NSComboBox comboBox, nint index)
        {
            return _items[index];
        }
    }

    public static class NSComboBoxDataSourceExtenstion
    {

    }
}
