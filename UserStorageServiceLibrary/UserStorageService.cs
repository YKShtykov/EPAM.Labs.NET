using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace UserStorageServiceLibrary
{
  /// <summary>
  /// User storage service stores users in inner storage. It realize add, find and delete methodes
  /// </summary>
  [Serializable]
  public class UserStorageService : IDisposable
  {
    readonly IStorage storage;
    readonly IIdGenedator generator;
    /// <summary>
    /// If additional validation is needed, this func will be useful
    /// </summary>
    public Func<User, bool> AdditionalUserValidation;
    public event EventHandler<UserEventArgs> StateChanged = delegate { };
    public event EventHandler<UserNotFoundEventArgs> UserNotFound = delegate { };

    public UserStorageService(IStorage storage = null,
                              IIdGenedator generator = null)
    {
      this.storage = storage ?? new DefaultStorage();
      this.generator = generator ?? new DefaultIdGenerator();
    }

    /// <summary>
    /// Adds user in storage
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    public int Add(User user)
    {
      if (ReferenceEquals(user, null)) throw new ArgumentNullException();

      if (!ReferenceEquals(AdditionalUserValidation, null))
        if (!AdditionalUserValidation(user)) throw new UserNotValidException();
      user.personalId = generator.GenerateId();

      storage.Add(user);
      OnStateChanged("Add", user);

      return user.personalId;
    }

    /// <summary>
    /// Delete user from storage
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    public bool Remove(User user)
    {
      if (ReferenceEquals(user, null)) throw new ArgumentNullException();
      bool result = storage.Delete(user);
      if (result) OnStateChanged("Remove", user);
      return result;
    }

    /// <summary>
    /// Delete user from storage
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public bool Remove(int userId)
    {
      bool result = storage.Delete(userId);
      if (result) OnStateChanged("Remove", new User() { personalId = userId });
      return result;
    }


    /// <summary>
    /// Find user by personal id
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public User FindById(int userId)
    {
      User result = storage.FindById(userId);
      if (ReferenceEquals(result, null)) OnUserNotFound(null, userId);
      result = storage.FindById(userId);
      return result;
    }

    /// <summary>
    /// find all users with that first name
    /// </summary>
    /// <param name="firstName"></param>
    /// <returns></returns>
    public IEnumerable<User> FindByFirstName(string firstName)
    {
      if (ReferenceEquals(firstName, null)) throw new ArgumentNullException();

      IEnumerable<User> users = storage.FindByPredicate(u => u.FirstName == firstName);
      if (users.Count() == 0)
      {
        OnUserNotFound("FirstName", firstName);
      }

      return storage.FindByPredicate(u => u.FirstName == firstName);
    }
    /// <summary>
    /// find all users with that last name
    /// </summary>
    /// <param name="lastName"></param>
    /// <returns></returns>
    public IEnumerable<User> FindByLastname(string lastName)
    {
      if (ReferenceEquals(lastName, null)) throw new ArgumentNullException();

      IEnumerable<User> users = storage.FindByPredicate(u => u.LastName == lastName);
      if (users.Count() == 0)
      {
        OnUserNotFound("LastName", lastName);
      }

      return storage.FindByPredicate(u => u.LastName == lastName);
    }
    /// <summary>
    /// find all users with that birth date
    /// </summary>
    /// <param name="date"></param>
    /// <returns></returns>
    public IEnumerable<User> FindByBirthDate(DateTime date)
    {
      if (ReferenceEquals(date, null)) throw new ArgumentNullException();

      IEnumerable<User> users = storage.FindByPredicate(u => u.BirthDate == date);
      if (users.Count() == 0)
      {
        OnUserNotFound("BirthDate", date);
      }

      return storage.FindByPredicate(u => u.BirthDate == date);
    }
    /// <summary>
    /// find all users with that gender
    /// </summary>
    /// <param name="gender"></param>
    /// <returns></returns>
    public IEnumerable<User> FindByGender(Gender gender)
    {
      if (ReferenceEquals(gender, null)) throw new ArgumentNullException();

      IEnumerable<User> users = storage.FindByPredicate(u => u.Gender == gender);
      if (users.Count() == 0)
      {
        OnUserNotFound("Gender", gender);
      }

      return storage.FindByPredicate(u => u.Gender == gender);
    }

    public IEnumerable<User> GetAll()
    {
      return storage.FindByPredicate(u => true);
    }    

    private void OnStateChanged(string operation, User user)
    {
      var evArgs = new UserEventArgs(operation, user);
      StateChanged(this, evArgs);
    }

    private void OnUserNotFound(string field, object value)
    {
      var evArgs = new UserNotFoundEventArgs(field, value);
      UserNotFound(this, evArgs);
    }

    public void Dispose()
    {
      XmlSerializer formatter = new XmlSerializer(typeof(User));
      using (FileStream fs = new FileStream("users.xml", FileMode.OpenOrCreate))
      {
        formatter.Serialize(fs, generator.CurrentId);

        List<User> allusers = storage.FindByPredicate(u => true).ToList();
        foreach (var user in allusers)
        {
          formatter.Serialize(fs, user);
        }

        Console.WriteLine("user storage service state was serialized");
      }
    }
  }
}
