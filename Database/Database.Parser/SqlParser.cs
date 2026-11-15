using Database.Parser.Commands;
using Database.Parser.Commands.Interfaces;
using Database.Parser.Expressions;
using Database.Parser.Expressions.Interfaces;
using Database.Parser.Lexing;
using Database.Parser.Lexing.Enums;
using Database.Shared.Enums;
using Database.Shared.Models;

namespace Database.Parser;

public class SqlParser
{
    private Token[] _tokens = new Token[0];
    private int _position;

    public ISqlCommand Parse(string sql)
    {
        var tokenizer = new Tokenizer();

        _tokens = tokenizer.Tokenize(sql);
        _position = 0;

        ISqlCommand command = ParseCommand();
        MatchSymbol(";");
        Expect(TokenType.EndOfInput);
        return command;
    }

    private ISqlCommand ParseCommand()
    {
        if (MatchKeyword("CREATE"))
            return ParseCreate();
        if (MatchKeyword("DROP"))
            return ParseDrop();
        if (MatchKeyword("TABLEINFO"))
            return new TableInfoCommand(ReadIdentifier());
        if (MatchKeyword("INSERT"))
            return ParseInsert();
        if (MatchKeyword("GET"))
            return ParseGetRow();
        if (MatchKeyword("DELETE"))
            return ParseDelete();
        if (MatchKeyword("SELECT"))
            return ParseSelect();

        throw Error("Unknown SQL command");
    }

    private ISqlCommand ParseCreate()
    {
        if (MatchKeyword("TABLE"))
            return ParseCreateTable();

        if (MatchKeyword("INDEX"))
            return ParseCreateIndex();

        throw Error("Expected TABLE or INDEX");
    }

    private ISqlCommand ParseDrop()
    {
        if (MatchKeyword("TABLE"))
            return new DropTableCommand(ReadIdentifier());

        if (MatchKeyword("INDEX"))
            return new DropIndexCommand(ReadIdentifier());

        throw Error("Expected TABLE or INDEX");
    }

    private ISqlCommand ParseCreateTable()
    {
        string tableName = ReadIdentifier();
        ExpectSymbol("(");

        var columns = new ColumnDefinition[_tokens.Length];

        int count = 0;

        do
        {
            columns[count++] = ReadColumnDefinition();
        }
        while (MatchSymbol(","));

        ExpectSymbol(")");

        return new CreateTableCommand(tableName, Copy(columns, count));
    }

    private ColumnDefinition ReadColumnDefinition()
    {
        string name = ReadIdentifier();
        ExpectSymbol(":");
        DataType type = ReadDataType();
        string? defaultValue = null;

        if (MatchKeyword("DEFAULT"))
            defaultValue = ReadValue();

        return new ColumnDefinition(name, type, defaultValue);
    }

    private ISqlCommand ParseCreateIndex()
    {
        string indexName = ReadIdentifier();

        ExpectKeyword("ON");

        string tableName = ReadIdentifier();

        ExpectSymbol("(");
        string columnName = ReadIdentifier();
        ExpectSymbol(")");

        return new CreateIndexCommand(indexName, tableName, columnName);
    }

    private ISqlCommand ParseInsert()
    {
        ExpectKeyword("INTO");

        string tableName = ReadIdentifier();

        string[] columns = new string[0];

        if (MatchSymbol("("))
        {
            columns = ReadIdentifierList();
            ExpectSymbol(")");
        }

        ExpectKeyword("VALUES");
        ExpectSymbol("(");

        string[] values = ReadValueList();

        ExpectSymbol(")");

        if (columns.Length > 0 &&
            columns.Length != values.Length)
        {
            throw Error("Column count does not match value count");
        }

        return new InsertCommand(tableName, columns, values);
    }

    private ISqlCommand ParseGetRow()
    {
        ExpectKeyword("ROW");

        int[] rowNumbers = ReadRowNumbers();

        ExpectKeyword("FROM");

        string tableName = ReadIdentifier();

        return new GetRowCommand(tableName, rowNumbers);
    }

    private ISqlCommand ParseDelete()
    {
        ExpectKeyword("FROM");

        string tableName = ReadIdentifier();

        if (MatchKeyword("ROW"))
        {
            int[] rowNumbers = ReadRowNumbers();

            return new DeleteCommand(tableName, rowNumbers: rowNumbers);
        }

        if (MatchKeyword("WHERE"))
        {
            IExpression condition = ParseOr();

            return new DeleteCommand(tableName, where: condition);
        }

        throw Error("Expected ROW or WHERE");
    }

    private ISqlCommand ParseSelect()
    {
        bool distinct = MatchKeyword("DISTINCT");

        string[] columns;

        if (MatchOperator("*"))
        {
            columns = new string[] { "*" };
        }
        else
        {
            columns = ReadIdentifierList();
        }

        ExpectKeyword("FROM");

        string tableName = ReadIdentifier();

        IExpression? where = null;
        string? orderByColumn = null;
        bool descending = false;

        if (MatchKeyword("WHERE"))
            where = ParseOr();

        if (MatchKeyword("ORDER"))
        {
            ExpectKeyword("BY");

            orderByColumn = ReadIdentifier();

            if (MatchKeyword("DESC"))
                descending = true;
            else
                MatchKeyword("ASC");
        }

        return new SelectCommand(tableName, columns, distinct, where, orderByColumn, descending);
    }

    private IExpression ParseOr()
    {
        IExpression left = ParseAnd();

        while (MatchKeyword("OR"))
        {
            IExpression right = ParseAnd();

            left = new LogicalExpression(left, "OR", right);
        }

        return left;
    }

    private IExpression ParseAnd()
    {
        IExpression left = ParseNot();

        while (MatchKeyword("AND"))
        {
            IExpression right = ParseNot();

            left = new LogicalExpression(left, "AND", right);
        }

        return left;
    }

    private IExpression ParseNot()
    {
        if (MatchKeyword("NOT"))
            return new NotExpression(ParseNot());

        return ParsePrimary();
    }

    private IExpression ParsePrimary()
    {
        if (MatchSymbol("("))
        {
            IExpression expression = ParseOr();
            ExpectSymbol(")");

            return expression;
        }

        string column = ReadIdentifier();
        string comparisonOperator = ReadComparisonOperator();
        string value = ReadValue();

        return new ComparisonExpression(column, comparisonOperator, value);
    }

    private string[] ReadIdentifierList()
    {
        string[] items = new string[_tokens.Length];
        int count = 0;

        do
        {
            items[count++] = ReadIdentifier();
        }
        while (MatchSymbol(","));

        return Copy(items, count);
    }

    private string[] ReadValueList()
    {
        string[] items = new string[_tokens.Length];
        int count = 0;

        do
        {
            items[count++] = ReadValue();
        }
        while (MatchSymbol(","));

        return Copy(items, count);
    }

    private DataType ReadDataType()
    {
        if (MatchKeyword("INT"))
            return DataType.Int;

        if (MatchKeyword("STRING"))
            return DataType.String;

        if (MatchKeyword("DATE"))
            return DataType.Date;

        throw Error("Invalid data type");
    }

    private string ReadValue()
    {
        bool negative = MatchOperator("-");

        Token token = Current;

        if (token.Type != TokenType.Number &&
            token.Type != TokenType.StringLiteral)
        {
            throw Error("Expected value");
        }

        if (negative && token.Type != TokenType.Number)
            throw Error("Minus requires a number");

        Advance();

        if (negative)
            return "-" + token.Value;

        return token.Value;
    }

    private int[] ReadRowNumbers()
    {
        var rowNumbers = new int[_tokens.Length];
        int count = 0;

        do
        {
            rowNumbers[count++] = ReadInteger();
        }
        while (MatchSymbol(","));

        return Copy(rowNumbers, count);
    }

    private int ReadInteger()
    {
        Token token = Expect(TokenType.Number);

        int result = 0;

        for (int i = 0; i < token.Value.Length; i++)
        {
            int digit = token.Value[i] - '0';

            if (result > (int.MaxValue - digit) / 10)
                throw Error("Integer is too large");

            result = result * 10 + digit;
        }

        return result;
    }

    private string ReadIdentifier()
    {
        Token token = Expect(TokenType.Identifier);
        return token.Value;
    }

    private string ReadComparisonOperator()
    {
        Token token = Expect(TokenType.Operator);

        if (token.Value == "-" || token.Value == "*")
            throw Error("Invalid comparison operator");

        return token.Value;
    }

    private Token Current => _tokens[_position];

    private void Advance()
    {
        if (_position < _tokens.Length - 1)
            _position++;
    }

    private bool MatchKeyword(string keyword)
    {
        if (Current.Type != TokenType.Identifier)
            return false;

        if (!EqualsIgnoreCase(Current.Value, keyword))
            return false;

        Advance();
        return true;
    }

    private void ExpectKeyword(string keyword)
    {
        if (!MatchKeyword(keyword))
            throw Error($"Expected keyword '{keyword}'");
    }

    private bool MatchSymbol(string symbol)
    {
        return MatchToken(TokenType.Symbol, symbol);
    }

    private void ExpectSymbol(string symbol)
    {
        if (!MatchSymbol(symbol))
            throw Error($"Expected symbol '{symbol}'");
    }

    private bool MatchOperator(string value)
    {
        return MatchToken(TokenType.Operator, value);
    }

    private bool MatchToken(TokenType type, string value)
    {
        if (Current.Type != type || Current.Value != value)
            return false;

        Advance();
        return true;
    }

    private Token Expect(TokenType type)
    {
        Token token = Current;

        if (token.Type != type)
            throw Error($"Expected {type}");

        if (type != TokenType.EndOfInput)
            Advance();

        return token;
    }

    private FormatException Error(string message)
    {
        return new FormatException($"{message} at position {Current.Position}");
    }

    private static T[] Copy<T>(T[] source, int count)
    {
        T[] result = new T[count];

        for (int i = 0; i < count; i++)
            result[i] = source[i];

        return result;
    }

    private static bool EqualsIgnoreCase(string first, string second)
    {
        if (first.Length != second.Length)
            return false;

        for (int i = 0; i < first.Length; i++)
        {
            if (ToLowerAscii(first[i]) != ToLowerAscii(second[i]))
                return false;
        }

        return true;
    }

    private static char ToLowerAscii(char c)
    {
        if (c >= 'A' && c <= 'Z')
            return (char)(c + ('a' - 'A'));

        return c;
    }
}
