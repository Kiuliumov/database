
using Database.Parser.Commands.Enums;
using Database.Parser.Commands.Interfaces;
using Database.Parser.Expressions.Interfaces;

namespace Database.Parser.Commands;

public class DeleteCommand : ISqlCommand
{
    public SqlCommandType CommandType => SqlCommandType.Delete;

    public string TableName { get; }
    public int? RowNumber { get; }
    public IExpression? Where { get; }

    public DeleteCommand(
        string tableName,
        int? rowNumber = null,
        IExpression? where = null)
    {
        TableName = tableName;
        RowNumber = rowNumber;
        Where = where;
    }
}
