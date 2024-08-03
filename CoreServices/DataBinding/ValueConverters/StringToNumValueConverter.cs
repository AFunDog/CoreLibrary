using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoreServices.DataBinding.Contracts;

namespace CoreServices.DataBinding.ValueConverters
{
    public sealed class StringToNumValueConverter : IValueConverter
    {
        public object Convert(object sourceValue, Type targetType, object? parameter)
        {
            return int.Parse((string)sourceValue);
        }
    }
}
