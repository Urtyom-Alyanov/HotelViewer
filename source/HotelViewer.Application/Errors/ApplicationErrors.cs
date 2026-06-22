using HotelViewer.Domain.Repository;

namespace HotelViewer.Application.Errors;

public abstract record ApplicationError(string Message);
public record AccessDenied(string Action) : ApplicationError($"У вас недостаточно прав для выполнения действия: {Action}");
public record RepositoryFailure(RepositoryError Error) : ApplicationError(Error.Message);
public record ValidationError(string Detail) : ApplicationError($"Ошибка валидации: {Detail}");
