﻿using System;

namespace UserStorageServiceLibrary
{
  [Serializable]
  public class User : ICloneable, IEquatable<User>
  {

    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime BirthDate { get; set; }
    public int personalId { get; set; }

    public Gender Gender { get; set; }
    public object Clone()
    {
      return new User()
      {
        FirstName = FirstName,
        LastName = LastName,
        BirthDate = BirthDate,
        personalId = personalId,
        Gender = Gender
      };
    }

    public bool Equals(User other)
    {
      if (ReferenceEquals(other, null)) return false;

      return FirstName == other.FirstName &&
             LastName == other.LastName &&
             BirthDate == other.BirthDate &&
             personalId == other.personalId &&
             Gender == other.Gender;
    }

    public override int GetHashCode()
    {
      return FirstName?.GetHashCode() ?? 0 ^
             LastName?.GetHashCode() ?? 0 ^
             BirthDate.GetHashCode() ^
             personalId.GetHashCode() ^
             Gender.GetHashCode();
    }
  }
  public enum Gender
  {
    Male,
    Female,
    Undefined
  }
}
