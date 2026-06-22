using Xunit;
using FluentAssertions;
using HotelViewer.Infrastructure;
using HotelViewer.Infrastructure.Mappers;
using HotelViewer.Domain.Repository;

namespace HotelViewer.Infrastructure.Mappers;

public class DataAccessErrorToRepositoryErrorMapperTests
{
    [Fact]
    public void Map_DatabaseConnectionError_ShouldReturnInfrastructureFaultWithDetail()
    {
        // Arrange
        var innerException = new Exception("Access denied or file locked");
        var error = new DatabaseConnectionError(innerException);

        // Act
        var result = DataAccessErrorToRepositoryErrorMapper.Map(error);

        // Assert
        result.Should().BeOfType<InfrastructureFault>();
        result.Message.Should().Contain("Связь с базой данных потеряна");
        result.Message.Should().Contain("Access denied or file locked");
    }

    [Fact]
    public void Map_QueryExecutionError_ShouldReturnInfrastructureFaultWithQueryContext()
    {
        // Arrange
        var sql = "SELECT * FROM NonExistentTable";
        var innerException = new Exception("Table not found");
        var error = new QueryExecutionError(sql, innerException);

        // Act
        var result = DataAccessErrorToRepositoryErrorMapper.Map(error);

        // Assert
        result.Should().BeOfType<InfrastructureFault>();
        result.Message.Should().Contain("Неверный SQL запрс");
        result.Message.Should().Contain("Table not found");
    }

    [Fact]
    public void Map_DriverNotInstalled_ShouldReturnInfrastructureFaultWithOriginalMessage()
    {
        // Arrange
        var error = new DriverNotInstalled();

        // Act
        var result = DataAccessErrorToRepositoryErrorMapper.Map(error);

        // Assert
        result.Should().BeOfType<InfrastructureFault>();
        result.Message.Should().Be("Сбой инфраструктуры данных: " + error.Message);
        result.Message.Should().Contain("Драйвер базы данных не установлен");
    }

    [Fact]
    public void Map_FileNotFoundError_ShouldTranslateCorrectly()
    {
        // Arrange
        var path = @"C:\db.accdb";
        var error = new FileNotFoundError(path);

        // Act
        var result = DataAccessErrorToRepositoryErrorMapper.Map(error);

        // Assert
        result.Should().BeOfType<InfrastructureFault>();
        result.Message.Should().Contain(path);
    }
}
