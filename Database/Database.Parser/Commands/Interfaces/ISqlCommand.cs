using Database.Parser.Commands.Enums;

namespace Database.Parser.Commands.Interfaces;

public interface ISqlCommand
{
    SqlCommandType CommandType { get; }
}
