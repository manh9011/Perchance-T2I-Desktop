namespace Perchance
{
    public static class Global
    {
        public static readonly SemaphoreSlim Semaphore = new(3);
        public static CancellationTokenSource Cancellation = new();
    }
}
