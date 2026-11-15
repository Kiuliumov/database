using Database.Parser.Commands.Enums;
using Database.Parser.Commands.Interfaces;

namespace Database.Parser.Commands;

public class CreateIndexCommand : ISqlCommand
{
    public SqlCommandType CommandType => SqlCommandType.CreateIndex;

    public string IndexName { get; }
    public string TableName { get; }
    public string ColumnName { get; }

    public CreateIndexCommand(
        string indexName,
        string tableName,
        string columnName)
    {
        IndexName = indexName;
        TableName = tableName;
        ColumnName = columnName;
    }
}
