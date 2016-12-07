using System;

namespace UserStorageServiceLibrary
{
  /// <summary>
  /// Default implementation of ID generator interface for user storage;
  /// </summary>
  [Serializable]
  class DefaultIdGenerator : IIdGenedator
  {
    public int CurrentId { get; private set; }

    public int GenerateId()
    {
      return CurrentId++;
    }
  }
}
