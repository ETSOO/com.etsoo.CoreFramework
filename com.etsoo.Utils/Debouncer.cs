using System.Collections.Concurrent;

namespace com.etsoo.Utils
{
    /// <summary>
    /// Debouncer
    /// 防抖
    /// </summary>
    public sealed class Debouncer<TKey> : IDisposable where TKey : notnull
    {
        private readonly CancellationTokenSource _cts = new();

        private readonly Func<TKey, CancellationToken, Task> _action;
        private readonly int _scanInterval;

        private readonly ConcurrentDictionary<TKey, DateTime> _pending = new();

        /// <summary>
        /// Constructor
        /// 构造函数
        /// </summary>
        /// <param name="action">Action to call</param>
        /// <param name="scanInterval">Scan interval in milliseconds, default is 1000</param>
        public Debouncer(Func<TKey, CancellationToken, Task> action, int scanInterval = 1000)
        {
            _action = action;
            _scanInterval = scanInterval;

            _ = RunAsync(_cts.Token);
        }

        public void Debounce(TKey key, TimeSpan delay)
        {
            var timestamp = DateTime.UtcNow.Add(delay);

            // Add or update the timestamp for the key
            _pending[key] = timestamp;
        }

        private async Task RunAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                await Task.Delay(_scanInterval, token);

                await ScanAsync();
            }
        }

        public async Task ScanAsync()
        {
            var now = DateTime.UtcNow;

            var token = _cts.Token;

            foreach (var (key, timestamp) in _pending)
            {
                if (now > timestamp && _pending.TryRemove(key, out _) && !token.IsCancellationRequested)
                {
                    await _action(key, token);
                }
            }
        }

        public void Dispose()
        {
            _cts.Cancel();
            _cts.Dispose();

            _pending.Clear();
        }

        public async Task DisposeAsync()
        {
            await _cts.CancelAsync();
            _cts.Dispose();

            _pending.Clear();
        }
    }
}
