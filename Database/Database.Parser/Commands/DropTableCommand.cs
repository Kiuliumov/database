using Database.Parser.Commands.Enums;
using Database.Parser.Commands.Interfaces;

namespace Database.Parser.Commands;

public class DropTableCommand : ISqlCommand
{
    public SqlCommandType CommandType => SqlCommandType.DropTable;

    public string TableName { get; }

    public DropTableCommand(string tableName)
    {
        TableName = tableName;
    }
}
