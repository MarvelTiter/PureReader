using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Utils
{
    internal class TaskHelper
    {
        Task _task;

        private TaskHelper() { }

        public void Run(Action action)
        {
            if (_task != null && !_task.IsCompleted) return;
            _task?.Dispose();
            _task = Task.Run(action);
        }

        static readonly Dictionary<string, TaskHelper> cache = new();
        public static void Run(string id, Action action)
        {
            if (!cache.TryGetValue(id, out var task))
            {
                task = new TaskHelper();
                cache.Add(id, task);
            }
            task.Run(action);
        }
    }
}
