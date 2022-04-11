using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetHome.Core.Helpers
{
    public class LockManager
    {
        private static readonly ConcurrentDictionary<int, object> deviceKeyring = new();
        public static object GetDeviceLock(int deviceId) => deviceKeyring.GetOrAdd(deviceId, o => new object());
    }
}
