using Database.Parser.Commands.Enums;
using Database.Parser.Commands.Interfaces;

namespace Database.Parser.Commands;

public class DropIndexCommand : ISqlCommand
{
    public SqlCommandType CommandType => SqlCommandType.DropIndex;

    public string IndexName { get; }

    public DropIndexCommand(string indexName)
    {
        IndexName = indexName;
    }
}
