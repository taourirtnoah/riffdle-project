namespace Riffdle.Models.ViewModels;

public class AutocompleteDropdownViewModel
{
    public string Label { get; set; } = string.Empty;

    public string InputName { get; set; } = string.Empty;

    public string InputId { get; set; } = string.Empty;

    public string HiddenName { get; set; } = string.Empty;

    public string HiddenId { get; set; } = string.Empty;

    public string? SelectedText { get; set; }

    public int? SelectedId { get; set; }

    public string SearchUrl { get; set; } = string.Empty;

    public string Placeholder { get; set; } = string.Empty;
}
