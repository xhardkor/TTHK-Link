using System.Globalization;
using TTHK_Link.Models;

namespace TTHK_Link.Converters;

public class SectionHeaderConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        // parameter = предыдущий item (мы его передадим), показываем хедер если секция изменилась
        if (value is not FlyoutMenuItem current) return false;

        if (parameter is not FlyoutMenuItem prev) return true;
        return !string.Equals(current.Section, prev.Section, StringComparison.Ordinal);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotSupportedException();
}