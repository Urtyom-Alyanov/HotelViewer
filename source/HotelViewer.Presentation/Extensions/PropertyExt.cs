using System.Linq.Expressions;

namespace HotelViewer.Presentation.Extensions;

public static class PropertyExt {
  public static string GetPath<TEntity, TValue>(Expression<Func<TEntity, TValue>> selector) {
    var expression = selector.Body;

    if (expression is UnaryExpression unary && unary.NodeType == ExpressionType.Convert)
      expression = unary.Operand;

    var parts = new List<string>();
    while (expression is MemberExpression member) {
      parts.Add(member.Member.Name);
      expression = member.Expression;
    }

    parts.Reverse();
    return string.Join(".", parts);
  }
}
