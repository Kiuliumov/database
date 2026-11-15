using Database.Parser.Commands.Enums;
using Database.Parser.Commands.Interfaces;
using Database.Parser.Enums;
using Database.Shared.Models;
using System.Windows.Input;

namespace Database.Parser.Commands;

public class CreateTableCommand : ICommand
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