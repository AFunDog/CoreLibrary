using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoreServices.DataBinding.Contracts;

namespace CoreServices.DataBinding.ValueConverters
{
    public sealed class CommonValueConverter(Func<object, Type, object?, object> converter) : IValueConverter
    {
        private Func<object, Type, object?, object> _converter = converter;

        public object Convert(object sourceValue, Type targetType, object? parameter)
        {
            return _converter(sourceValue, targetType, parameter);
        }
    }
}
