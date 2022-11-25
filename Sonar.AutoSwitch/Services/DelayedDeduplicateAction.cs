using System;
using System.Threading;
using System.Threading.Tasks;

namespace Sonar.AutoSwitch.Services;

public class DelayedDeduplicateAction
{
    private readonly SemaphoreSlim _semaphoreSlim = new(1, 1);
    private CancellationTokenSource _cancellationToken;

    public void QueueAction(Func<Task> action, int delayInMs = 2000)
    {
        _semaphoreSlim.Wait();
        try
        {
            _cancellationToken?.Cancel();
            _cancellationToken?.Dispose();
            _cancellationToken = new CancellationTokenSource();
            Task.Delay(delayInMs, _cancellationToken.Token).ContinueWith(async t =>
            {
                if (!t.IsCompletedSuccessfully || t.IsCanceled) return;
                await _semaphoreSlim.WaitAsync();
                await action().ContinueWith(_ => _).ConfigureAwait(false);
                _semaphoreSlim.Release();
            });
        }
        finally
        {
            _semaphoreSlim.Release();
        }
    }
}