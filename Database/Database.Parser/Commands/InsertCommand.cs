using Database.Parser.Commands.Enums;
using Database.Parser.Commands.Interfaces;

namespace Database.Parser.Commands;

public class InsertCommand : ISqlCommand
{
    public SqlCommandType CommandType => SqlCommandType.Insert;
    public string TableName { get; }
    public string[] Columns { get; }
    public string[] Values { get; }

    public InsertCommand(string tableName, string[] columns, string[] values)
    {
        TableName = tableName;
        Columns = columns;
        Values = values;
    }
}
