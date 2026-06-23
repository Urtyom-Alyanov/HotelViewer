using HotelViewer.ApplicationLayer.Errors;
using HotelViewer.Domain.Entity;
using LanguageExt;
using static LanguageExt.Prelude;

namespace HotelViewer.ApplicationLayer.Services;

public abstract class BaseService(SessionContext sessionContext) {
  protected Either<ApplicationError, Unit> EnsureRole(UserRole requiredRole, string action) {
    return sessionContext.IsInRole(requiredRole)
      ? Right(unit)
      : Left<ApplicationError, Unit>(new AccessDenied(action));
  }
}
