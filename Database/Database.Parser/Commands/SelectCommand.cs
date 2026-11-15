
using Database.Parser.Commands.Enums;
using Database.Parser.Commands.Interfaces;
using Database.Parser.Expressions.Interfaces;

namespace Database.Parser.Commands;

public class SelectCommand : ISqlCommand
{
    public SqlCommandType CommandType => SqlCommandType.Select;

    public string TableName { get; }
    public string[] Columns { get; }
    public bool Distinct { get; }
    public IExpression? Where { get; }
    public string? OrderByColumn { get; }
    public bool OrderDescending { get; }

    public SelectCommand(
        string tableName,
        string[] columns,
        bool distinct = false,
        IExpression? where = null,
        string? orderByColumn = null,
        bool orderDescending = false)
    {
        TableName = tableName;
        Columns = columns;
        Distinct = distinct;
        Where = where;
        OrderByColumn = orderByColumn;
        OrderDescending = orderDescending;
    }
}
