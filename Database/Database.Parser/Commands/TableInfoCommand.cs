using Database.Parser.Commands.Enums;
using Database.Parser.Commands.Interfaces;

namespace Database.Parser.Commands;

public class TableInfoCommand : ISqlCommand
{
    public SqlCommandType CommandType => SqlCommandType.TableInfo;

    public string TableName { get; }

    public TableInfoCommand(string tableName)
    {
        TableName = tableName;
    }
}
