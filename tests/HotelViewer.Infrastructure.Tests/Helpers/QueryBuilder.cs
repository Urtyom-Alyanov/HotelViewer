using System.Linq.Expressions;
using FluentAssertions;
using HotelViewer.Domain.Entity;
using HotelViewer.Domain.Helper;
using LanguageExt;
using static LanguageExt.Prelude;

namespace HotelViewer.Infrastructure.Helpers;

public class QueryBuilder {
  private static Option<string> MockPropertyMap(Expression<Func<User, object>> selector) {
    var name = PropertyExt.GetPropertyName(selector).IfNone("");
    return name switch {
      nameof(User.Username) => Some("USERNAME_COL"),
      nameof(User.Role) => Some("ROLE_COL"),
      _ => None
    };
  }

  private readonly QueryBuilder<User> _builder = new("SELECT * FROM Tests", MockPropertyMap);

  [Fact]
  public void Build_WithSimpleEquality_ReturnsCorrectSql() {
    // Arrange
    var filter = new Filter<User>(
      new FilterCriterion<User>(e => e.Username, "John")
    );

    // Act
    var result = _builder.Build(Some(filter), None, None, None);

    // Assert
    result.Sql.Should().Be("SELECT * FROM Tests WHERE USERNAME_COL = ?");
    result.Parameters.Should().HaveCount(1).And.Contain("John");
  }

  [Fact]
  public void Build_WithInOperator_GeneratesMultiplePlaceholders() {
    // Arrange
    var ids = new List<Username> { new("John"), new("Julia"), new("Doe") };
    var filter = new Filter<User>(
      new FilterCriterion<User>(e => e.Username, ids, FilterOp.In)
    );

    // Act
    var result = _builder.Build(Some(filter), None, None, None);

    // Assert
    result.Sql.Should().Contain("USERNAME_COL IN (?, ?, ?)");
    result.Parameters.Should().HaveCount(3).And.ContainInOrder("John", "Julia", "Doe");
  }

  [Fact]
  public void Build_WithMultipleCriteria_OrdersParametersCorrectly() {
    // Arrange
    var filter = new Filter<User>()
      .And(e => e.Username, new Username("JULI"))
      .And(e => e.Role, (int)UserRole.Admin, FilterOp.Like);

    // Act
    var result = _builder.Build(Some(filter), None, None, None);

    // Assert
    result.Sql.Should().Be("SELECT * FROM Tests WHERE USERNAME_COL = ? AND ROLE_COL LIKE ?");
    result.Parameters.Should().HaveCount(2);
    var @params = result.Parameters.ToArray();
    @params[0].Should().Be("JULI");
    @params[1].Should().Be("%2%");
  }

  [Fact]
  public void Build_WithSort_AppendsOrderBy() {
    // Arrange
    var sort = new Sort<User>(e => e.Username, Ascending: false);

    // Act
    var result = _builder.Build(None, Some(sort), None, None);

    // Assert
    result.Sql.Should().EndWith("ORDER BY USERNAME_COL DESC");
  }
}
