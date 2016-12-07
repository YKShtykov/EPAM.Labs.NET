using UserStorageServiceLibrary;

namespace WcfServiceLibrary1
{
  /// <summary>
  /// Converter for user and user context conversion
  /// </summary>
  public class ContextConverter
  {
    /// <summary>
    /// Convert user to user context
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    public static UserContext UserToContext(User user)
    {
      if (ReferenceEquals(user, null)) return null;
      return new UserContext()
      {
        FirstName = user.FirstName,
        LastName = user.LastName,
        DateOfBirth = user.BirthDate,
        Gender = user.Gender,
        PersonalId = user.personalId
      };
    }
    /// <summary>
    /// Convert user context to user
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    public static User ContextToUser(UserContext user)
    {
      if (ReferenceEquals(user, null)) return null;
      return new User()
      {
        FirstName = user.FirstName,
        LastName = user.LastName,
        BirthDate = user.DateOfBirth,
        Gender = user.Gender,
        personalId = user.PersonalId
      };
    }
  }
}
