namespace DigitalDoor.Reporting.Entities.Helpers;

internal class ExpressionsTools
{
    public static MemberExpression GetMemberInfo(Expression method)
    {
        LambdaExpression lambda = method as LambdaExpression;
        if(lambda == null)
            throw new ArgumentNullException("method");

        MemberExpression memberExpr = null;

        if(lambda.Body.NodeType == ExpressionType.Convert)
        {
            memberExpr =
                ((UnaryExpression)lambda.Body).Operand as MemberExpression;
        }
        else if(lambda.Body.NodeType == ExpressionType.MemberAccess)
        {
            memberExpr = lambda.Body as MemberExpression;
        }

        if(memberExpr == null)
            throw new ArgumentException("method");

        return memberExpr;
    }
}
