using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreServices.DataBinding.Contracts
{
    public interface IValueConverter
    {
        object Convert(object sourceValue,Type targetType,object parameter);
    }
}
