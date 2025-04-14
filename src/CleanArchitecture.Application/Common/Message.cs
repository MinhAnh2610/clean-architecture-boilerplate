namespace CleanArchitecture.Application.Common;

public static class Message
{
  public static string SUCCESSFUL_RETRIEVED(string objectName)
  {
    return $"Retrieved {objectName} successfully.";
  }

  public static string SUCCESSFUL_CREATED(string objectName)
  {
    return $"Created {objectName} successfully.";
  }

  public static string SUCCESSFUL_DELETED(string objectName)
  {
    return $"Deleted {objectName} successfully.";
  }

  public static string SUCCESSFUL_UPDATED(string objectName)
  {
    return $"Updated {objectName} successfully.";
  }
}
