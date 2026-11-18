using Database.Parser;
using Database.Parser.Commands;
using Database.Parser.Commands.Interfaces;

var parser = new SqlParser();

while (true)
{
    Console.Write("Database> ");

    string? sql = Console.ReadLine();

    if (sql == null || sql == "exit")
        break;

    try
    {
        ISqlCommand command = parser.Parse(sql);

        Console.WriteLine(
            $"Parsed: {command.CommandType}"
        );

        if (command is CreateTableCommand create)
        {
            Console.WriteLine(
                $"Table: {create.TableName}"
            );

            foreach (var column in create.Columns)
            {
                if (column.MaxLength.HasValue)
                {
                    Console.WriteLine(
                        $"  {column.Name}: {column.Type} (MaxLength: {column.MaxLength})"
                    );
                }
                else
                {
                    Console.WriteLine(
                        $"  {column.Name}: {column.Type}"
                    );
                }
                if (column.DefaultValue != null)
                {
                    Console.WriteLine(
                        $"    Default: {column.DefaultValue}"
                    );
                }
            }
        }
    }
    catch (FormatException ex)
    {
        Console.WriteLine(
            $"SQL Error: {ex.Message}"
        );
    }
}
