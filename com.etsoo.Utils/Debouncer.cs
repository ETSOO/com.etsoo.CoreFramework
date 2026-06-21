using System.Collections.Concurrent;

namespace com.etsoo.Utils
{
    /// <summary>
    /// Debouncer
    /// 防抖
    /// </summary>
    public sealed class Debouncer<TKey> : IDisposable, IAsyncDisposable where TKey : notnull
    {
        private readonly CancellationTokenSource _cts = new();

        private readonly Func<TKey, CancellationToken, Task> _action;
        private readonly TimeSpan _delay;
        private readonly int _scanInterval;
        private readonly bool _overnightCheck;

        private readonly ConcurrentDictionary<TKey, DateTime> _pending = new();

        /// <summary>
        /// Constructor
        /// 构造函数
        /// </summary>
        /// <param name="action">Action to call</param>
        /// <param name="delay">Delay time for debouncing</param>
        /// <param name="scanInterval">Scan interval in milliseconds, default is 1000</param>
        /// <param name="overnightCheck">Whether to check overnight, default is false</param>
        public Debouncer(Func<TKey, CancellationToken, Task> action,
            TimeSpan delay,
            int scanInterval = 1000,
            bool overnightCheck = true)
        {
            _action = action;
            _delay = delay;
            _scanInterval = scanInterval;
            _overnightCheck = overnightCheck;

            _ = RunAsync(_cts.Token);
        }

        public void Debounce(TKey key)
        {
            var now = DateTime.UtcNow;
            var timestamp = now.Add(_delay);

            // Add or update the timestamp for the key
            _pending[key] = timestamp;

            // When the timestamp is an overnight time, execute the action immediately
            if (_overnightCheck && timestamp.Date > now.Date)
            {
                var token = _cts.Token;
                if (_pending.TryRemove(key, out _) && !token.IsCancellationRequested)
                {
                    _action(key, token).GetAwaiter().GetResult();
                }
            }
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

        public async ValueTask DisposeAsync()
        {
            await _cts.CancelAsync();
            _cts.Dispose();

            _pending.Clear();
        }
    }
}
