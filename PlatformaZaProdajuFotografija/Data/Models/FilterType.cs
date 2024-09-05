namespace Data.Models;

public enum FilterType
{
    Name,
    Photographer,
    Tag,
    Owner,
    All
}

public static class FilterTypeExtensions
{
    public static string ToFilterTypeString(this FilterType filterType)
    {
        return filterType switch
        {
            FilterType.Name => "Name",
            FilterType.Photographer => "Photographer",
            FilterType.Tag => "Tag",
            FilterType.Owner => "Owner",
            FilterType.All => "All",
            _ => "All"
        };
    }

    public static FilterType ParseFilterType(string filterType)
    {
        return filterType switch
        {
            "Name" => FilterType.Name,
            "Photographer" => FilterType.Photographer,
            "Tag" => FilterType.Tag,
            "Owner" => FilterType.Owner,
            "All" => FilterType.All,
            _ => FilterType.All
        };
    }
}