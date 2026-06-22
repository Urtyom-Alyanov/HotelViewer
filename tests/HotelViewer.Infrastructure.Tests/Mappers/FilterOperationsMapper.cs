using FluentAssertions;
using HotelViewer.Domain.Entity;
using HotelViewer.Domain.Helper;
using HotelViewer.Domain.Value;
using LanguageExt;
using static LanguageExt.Prelude;

namespace HotelViewer.Infrastructure.Mappers;

public class FilterOperationsMapper {
  [Fact]
  public void UnwrapValueObject_ShouldExtractValueProperty()
  {
    // Arrange
    var id = new HotelId(42);

    // Act
    var result = FilterOperationMapper.UnwrapValueObject(id);

    // Assert
    result.Should().Be(42);
    result.Should().Be(id.Value);
  }

  [Fact]
  public void UnwrapValueObject_ShouldExtractValuePropertyFromFunction()
  {
    // Arrange
    var id = new RoomNumber(1, 2);

    // Act
    var result = FilterOperationMapper.UnwrapValueObject(id);

    // Assert
    result.Should().Be(102);
    result.Should().Be(id.ToDbValue());
  }

  [Fact]
  public void MapPlaceholders_ShouldReturnMultipleQuestions_ForInOperator()
  {
    // Act
    var result = FilterOperationMapper.MapPlaceholders(FilterOp.In, Some(3u));

    // Assert
    result.Should().Be("(?, ?, ?)");
  }

  [Fact]
  public void MapValue_ShouldReturnListOfUnwrappedObjects_ForInOperator()
  {
    // Arrange
    var ids = new List<HotelId> { new(1), new(2) };

    // Act
    var result = FilterOperationMapper.MapValue(FilterOp.In, ids);

    // Assert
    result.Should().BeAssignableTo<IEnumerable<object>>();
    var list = (IEnumerable<object>)result;
    list.Should().ContainInOrder(1, 2);
  }

  [Fact]
  public void MapValue_ShouldAddPercents_ForLikeOperator()
  {
    // Act
    var result = FilterOperationMapper.MapValue(FilterOp.Like, "admin");

    // Assert
    result.Should().Be("%admin%");
  }
}
