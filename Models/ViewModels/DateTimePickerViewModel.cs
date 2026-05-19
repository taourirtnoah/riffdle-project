using System.Globalization;

namespace Riffdle.Models.ViewModels;

public class DateTimePickerViewModel
{
    public string Label { get; set; } = string.Empty;

    public string InputName { get; set; } = string.Empty;

    public string InputId { get; set; } = string.Empty;

    public DateTime? Value { get; set; }

    public string DisplayFormat { get; set; } = CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern + " HH:mm";

    public string? Placeholder { get; set; }
}
