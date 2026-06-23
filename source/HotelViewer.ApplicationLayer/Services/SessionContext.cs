using HotelViewer.Domain.Entity;
using LanguageExt;
using static LanguageExt.Prelude;

namespace HotelViewer.ApplicationLayer.Services;

public class SessionContext {
  public Option<User> CurrentUser { get; private set; } = None;

  public void SetUser(User user) => CurrentUser = Some(user);
  public void Clear() => CurrentUser = None;

  public bool IsInRole(UserRole role) =>
    CurrentUser.Match(u => u.Role >= role, () => false);
}
