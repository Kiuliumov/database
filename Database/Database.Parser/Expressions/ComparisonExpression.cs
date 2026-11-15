using Database.Parser.Expressions.Interfaces;

namespace Database.Parser.Expressions;

public class ComparisonExpression : IExpression
{
    public string ColumnName { get; }
    public string Operator { get; }
    public string Value { get; }

    public ComparisonExpression(
        string columnName,
        string comparisonOperator,
        string value)
    {
        ColumnName = columnName;
        Operator = comparisonOperator;
        Value = value;
    }
}
