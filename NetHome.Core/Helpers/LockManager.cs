using System.Collections.Concurrent;

namespace NetHome.Core.Helpers
{
    public class LockManager
    {
        private static readonly ConcurrentDictionary<int, object> deviceKeyring = new();
        public static object GetDeviceLock(int deviceId) => deviceKeyring.GetOrAdd(deviceId, o => new object());
    }
}
