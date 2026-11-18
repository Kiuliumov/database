using Database.Shared.Enums;

namespace Database.Shared.Models;

public class ColumnDefinition
{
    public string Name { get; }
    public DataType Type { get; }
    public string? DefaultValue { get; }
    public int? MaxLength { get; }

    public ColumnDefinition(
        string name,
        DataType type,
        string? defaultValue = null,
        int? maxLength = null)
    {
        if (type != DataType.String && maxLength.HasValue)
            throw new ArgumentException(
                "MaxLength can only be set for string data types.");

        if (maxLength.HasValue &&
            (maxLength.Value < 3 || maxLength.Value > 255))
            throw new ArgumentException(
                "MaxLength must be between 3 and 255.");

        if (type == DataType.String && !maxLength.HasValue)
            maxLength = 255; // Default max length for strings if not specified

        Name = name;
        Type = type;
        DefaultValue = defaultValue;
        MaxLength = maxLength;
    }
}