using System;
using System.Linq;
using NUnit.Framework;
using UserStorageServiceLibrary;
using System.Collections.Generic;

namespace UserStorageServiceLibraryTests
{
  [TestFixture]
  public class UserStorageServiceTests
  {
    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Add_Null_ArgumentNullException()
    {
      var service = new UserStorageService();

      service.Add(null);
    }

    [Test]
    public void Add_ValidUser_IdReturned()
    {
      var service = new UserStorageService();
      int id = service.Add(new User());
      Assert.True(id == 0);
    }

    [Test]
    public void Remove_ValidUser_TrueReturned()
    {
      var service = new UserStorageService();
      int id = service.Add(new User());
      Assert.True(service.Remove(id));
    }

    [Test]
    public void Remove_UnexistingUser_FalseReturned()
    {
      var service = new UserStorageService();
      int id = service.Add(new User());
      Assert.False(service.Remove(100));
    }

    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Remove_Null_ArgumentNullException()
    {
      var service = new UserStorageService();

      Assert.False(service.Remove(null));
    }

    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void FindByFirstname_Null_ArgumentNullException()
    {
      var service = new UserStorageService();

      service.FindByFirstName(null);
    }

    [Test]
    public void FindByFirstname_ExistingFirstname_UsersReturned()
    {
      var service = new UserStorageService();
      User a = new User() { FirstName = "Yakov" };
      User b = new User() { FirstName = "Yakov" };
      service.Add(a);
      service.Add(b);
      var result = service.FindByFirstName("Yakov").ToList();
      List<User> expected = new List<User>();
      expected.Add(a);
      expected.Add(b);
      CollectionAssert.AreEqual(result,expected);
    }


    [Test]
    public void Find_ExistingUser_UserReturned()
    {
      var service = new UserStorageService();
      User user = new User();
      int id = service.Add(user);
      User actual = service.FindById(0);
      Assert.True(user.Equals(actual));
    }
  }
}
