using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Utils
{
    internal class TaskHelper
    {
        Task _task;
        CancellationTokenSource source;
        private TaskHelper() { }

        private void Run(Action<CancellationToken> action, bool force)
        {
            if (_task != null && !_task.IsCompleted && !force) return;
            source?.Cancel();
            _task?.Dispose();
            source = new CancellationTokenSource();
            var token = source.Token;
            _task = Task.Run(() =>
            {
                action.Invoke(token);
            }, token);
        }

        static readonly Dictionary<string, TaskHelper> cache = new();
        public static void Run(string id, Action<CancellationToken> action)
        {
            Run(id, action, false);
        }

        public static void Run(string id, Action<CancellationToken> action, bool force)
        {
            if (!cache.TryGetValue(id, out var task))
            {
                task = new TaskHelper();
                cache.Add(id, task);
            }
            task.Run(action, force);
        }
    }
}
