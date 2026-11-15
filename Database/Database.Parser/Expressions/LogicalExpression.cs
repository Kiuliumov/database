using Database.Parser.Expressions.Interfaces;

namespace Database.Parser.Expressions;

public class LogicalExpression : IExpression
{
    public IExpression Left { get; }
    public string Operator { get; }
    public IExpression Right { get; }

    public LogicalExpression(IExpression left, string logicalOperator, IExpression right)
    {
        Left = left;
        Operator = logicalOperator;
        Right = right;
    }
}
