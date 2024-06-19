using CoreServices.DataBinding.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreServices.DataBinding.ValueConverters
{
    public class StringToNumValueConverter : IValueConverter
    {
        public object Convert(object sourceValue, Type targetType, object? parameter)
        {
            return int.Parse((string)sourceValue);
        }
    }
}
