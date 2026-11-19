namespace Database.Shared.Models;

public class TableDefinition
{
    public string TableName { get; set; }
    public List<ColumnDefinition> Columns { get; set; }

    public TableDefinition(string tableName, List<ColumnDefinition> columns)
    {
        TableName = tableName;
        Columns = columns;
    }
}