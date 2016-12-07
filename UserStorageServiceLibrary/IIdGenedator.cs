namespace UserStorageServiceLibrary
{
  /// <summary>
  /// ID generator interface 
  /// </summary>
  public interface IIdGenedator
  {
    int CurrentId { get; }

    int GenerateId();
  }
}
