using Database.Parser.Lexing;

class Program
{
    static void Main(string[] args)
    {
        Tokenizer tokenizer = new Tokenizer();

        Token[] tokens = tokenizer.Tokenize(
            "INSERT INTO Students(Id, Name) VALUES (1, 'Иван')"
        );

        foreach (Token token in tokens)
        {
            Console.WriteLine(
                $"{token.Type}: {token.Value}"
            );
        }
    }
}