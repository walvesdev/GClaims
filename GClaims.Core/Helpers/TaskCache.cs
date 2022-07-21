namespace GClaims.Core.Helpers;

public static class TaskCache
{
    static TaskCache()
    {
        TrueResult = Task.FromResult(true);
        FalseResult = Task.FromResult(false);
    }

    public static Task<bool> TrueResult { get; }

    public static Task<bool> FalseResult { get; }
}