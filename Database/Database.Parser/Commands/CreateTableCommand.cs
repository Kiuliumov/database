using Database.Parser.Commands.Enums;
using Database.Parser.Commands.Interfaces;
using Database.Shared.Models;

namespace Database.Parser.Commands;

public class CreateTableCommand : ISqlCommand
{
    public SqlCommandType CommandType => SqlCommandType.CreateTable;

    public string TableName { get; }
    public ColumnDefinition[] Columns { get; }

    public CreateTableCommand(
        string tableName,
        ColumnDefinition[] columns)
    {
        TableName = tableName;
        Columns = columns;
    }
}