using Database.Parser.Commands.Enums;
using Database.Parser.Commands.Interfaces;

namespace Database.Parser.Commands;

public class GetRowCommand : ISqlCommand
{
    public SqlCommandType CommandType => SqlCommandType.GetRow;

    public string TableName { get; }
    public int RowNumber { get; }

    public GetRowCommand(string tableName, int rowNumber)
    {
        TableName = tableName;
        RowNumber = rowNumber;
    }
}
