using Database.Shared.Enums;

namespace Database.Shared.Models;

public class ColumnDefinition
{
    public string Name { get; }
    public DataType Type { get; }
    public string? DefaultValue { get; }

    public ColumnDefinition(
        string name,
        DataType type,
        string? defaultValue = null)
    {
        Name = name;
        Type = type;
        DefaultValue = defaultValue;
    }
}
