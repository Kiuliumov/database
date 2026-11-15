
using Database.Parser.Commands.Enums;
using Database.Parser.Commands.Interfaces;
using Database.Parser.Expressions.Interfaces;

namespace Database.Parser.Commands;

public class DeleteCommand : ISqlCommand
{
    public SqlCommandType CommandType => SqlCommandType.Delete;

    public string TableName { get; }
    public int[]? RowNumbers { get; }
    public IExpression? Where { get; }

    public DeleteCommand(
        string tableName,
        int[]? rowNumbers = null,
        IExpression? where = null)
    {
        TableName = tableName;
        RowNumbers = rowNumbers;
        Where = where;
    }
}
