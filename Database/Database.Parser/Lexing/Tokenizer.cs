using Database.Parser.Lexing.Enums;

namespace Database.Parser.Lexing;

public class Tokenizer
{
    public Token[] Tokenize(string sql)
    {
        ArgumentNullException.ThrowIfNull(sql);

        // Each character can produce at most one token, plus the end marker.
        var tokens = new Token[sql.Length + 1];
        int count = 0;
        int position = 0;

        while (position < sql.Length)
        {
            char current = sql[position];

            // Skip whitespace between tokens.
            if (IsWhitespace(current))
            {
                position++;
                continue;
            }

            int start = position;
            TokenType type;

            // Read a name, allowing digits after the first character.
            if (IsIdentifierStart(current))
            {
                type = TokenType.Identifier;
                do
                {
                    position++;
                }
                while (position < sql.Length &&
                       IsIdentifierPart(sql[position]));
            }
            // Read a whole number.
            else if (IsDigit(current))
            {
                type = TokenType.Number;
                do
                {
                    position++;
                }
                while (position < sql.Length && IsDigit(sql[position]));
            }
            // Read a quoted string, including its surrounding quotes.
            else if (IsQuote(current))
            {
                type = TokenType.StringLiteral;
                position = ReadStringEnd(sql, start);
            }
            // Read a single punctuation symbol.
            else if (IsSymbol(current))
            {
                type = TokenType.Symbol;
                position++;
            }
            // Read a one- or two-character comparison operator.
            else if (IsOperatorStart(current))
            {
                type = TokenType.Operator;
                position = ReadOperatorEnd(sql, start);
            }
            else
            {
                // Reject characters outside the supported token types.
                throw new FormatException($"Unexpected character '{current}' at position {position}.");
            }

            // Keep the original text and starting position.
            tokens[count++] = new Token(type, Extract(sql, start, position), start);
        }

        // Mark the end so the parser knows there are no more tokens.
        tokens[count++] = new Token(TokenType.EndOfInput, "", position);
        return CopyTokens(tokens, count);
    }

    // Copy only the populated part of the token buffer.
    private static Token[] CopyTokens(Token[] tokens, int count)
    {
        var result = new Token[count];
        for (int i = 0; i < count; i++)
            result[i] = tokens[i];

        return result;
    }

    // Copy token text without built-in string processing methods.
    private static string Extract(string input, int start, int end)
    {
        var characters = new char[end - start];
        for (int i = start; i < end; i++)
            characters[i - start] = input[i];

        return new string(characters);
    }

    // Return the position immediately after the closing quote.
    private static int ReadStringEnd(string sql, int start)
    {
        char quote = sql[start];
        int position = start + 1;

        while (position < sql.Length)
        {
            if (sql[position] != quote)
            {
                position++;
                continue;
            }

            // A doubled quote is part of the string, not its closing delimiter.
            if (position + 1 < sql.Length && sql[position + 1] == quote)
            {
                position += 2;
                continue;
            }

            return position + 1;
        }

        throw new FormatException($"Unterminated string at position {start}.");
    }

    private static int ReadOperatorEnd(string sql, int start)
    {
        int next = start + 1;

        if (next < sql.Length && IsTwoCharacterOperator(sql[start], sql[next]))
            return next + 1;

        // An exclamation mark is only valid as part of !=.
        if (sql[start] == '!')
            throw new FormatException($"Invalid operator at position {start}.");

        return next;
    }

    // Character rules for the supported SQL tokens.
    private static bool IsWhitespace(char c)
    {
        return c is ' ' or '\t' or '\n' or '\r';
    }

    private static bool IsIdentifierStart(char c)
    {
        return IsLetter(c) || c == '_';
    }

    private static bool IsIdentifierPart(char c)
    {
        return IsIdentifierStart(c) || IsDigit(c);
    }

    private static bool IsQuote(char c)
    {
        return c is '\'' or '"';
    }

    private static bool IsSymbol(char c)
    {
        return c is '(' or ')' or ',' or ':' or ';';
    }

    private static bool IsOperatorStart(char c)
    {
        return c is '=' or '<' or '>' or '!';
    }

    private static bool IsTwoCharacterOperator(char first, char second)
    {
        return (first == '<' && (second == '=' || second == '>')) ||
               (first == '>' && second == '=') ||
               (first == '!' && second == '=');
    }

    private static bool IsLetter(char c)
    {
        return (c >= 'a' && c <= 'z') ||
               (c >= 'A' && c <= 'Z');
    }

    private static bool IsDigit(char c)
    {
        return c >= '0' && c <= '9';
    }
}
