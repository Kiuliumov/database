namespace Database.Parser.Utilities;

internal static class CharUtils
{
    internal static bool IsWhitespace(char c) =>
        c is ' ' or '\t' or '\n' or '\r';

    internal static bool IsIdentifierStart(char c) =>
        IsLetter(c) || c == '_';

    internal static bool IsIdentifierPart(char c) =>
        IsIdentifierStart(c) || IsDigit(c);

    internal static bool IsQuote(char c) =>
        c is '\'' or '"';

    internal static bool IsSymbol(char c) =>
        c is '(' or ')' or ',' or ':' or ';';

    internal static bool IsOperatorStart(char c) =>
        c is '=' or '<' or '>' or '!' or '-' or '*';

    internal static bool IsTwoCharacterOperator(char first, char second) =>
        (first == '<' && second is '=' or '>') ||
        (first == '>' && second == '=') ||
        (first == '!' && second == '=');

    internal static bool IsLetter(char c) =>
        c is >= 'a' and <= 'z' or >= 'A' and <= 'Z';

    internal static bool IsDigit(char c) =>
        c is >= '0' and <= '9';
}
