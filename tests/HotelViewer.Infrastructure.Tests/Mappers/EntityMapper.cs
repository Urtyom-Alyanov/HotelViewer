using FluentAssertions;
using LanguageExt;

namespace HotelViewer.Infrastructure.Mappers;

public class EntityMapper {
  [Fact]
  public void MapPropertyIntoDbColumn_ShouldReturnCorrectColumn() {
    var result = UserMapper.MapPropertyIntoDbColumn(e => e.Username);

    result.Should().Be("ИмяПользователя");
  }

  [Fact]
  public void MapDbColumnIntoProperty_ShouldReturnCorrectColumn() {
    var result = UserMapper.MapDbColumnIntoProperty("ИмяПользователя");

    result.Should().Be("Username");
  }

  [Fact]
  public void MapDbColumnIntoProperty_ShouldNoneOnIncorrect() {
    var result = UserMapper.MapDbColumnIntoProperty("oijpojpojpojiPONJPOI");

    result.Should().Be(Option<string>.None);
  }
}
