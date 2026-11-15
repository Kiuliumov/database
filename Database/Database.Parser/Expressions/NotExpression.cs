using Database.Parser.Expressions.Interfaces;

namespace Database.Parser.Expressions;

public class NotExpression : IExpression
{
    public IExpression Expression { get; }

    public NotExpression(IExpression expression)
    {
        Expression = expression;
    }
}
