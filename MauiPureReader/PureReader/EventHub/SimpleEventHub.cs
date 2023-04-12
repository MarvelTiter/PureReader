using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureReader.EventHub
{
    internal static class SimpleEventHub
    {
        class EventHub
        {
            public string Token { get; set; }
        }
        class EventHub<TMessage> : EventHub
        {
            public Action<TMessage> Action { get; set; }
        }

        static readonly Dictionary<Type, IList<EventHub>> caches = new Dictionary<Type, IList<EventHub>>();
        internal static void UnRegister<TViewModel>(this TViewModel model)
        {
            caches.Remove(typeof(TViewModel));
        }
        internal static void Register<TViewModel, TMessage>(this TViewModel model, string token, Action<TMessage> action)
        {
            if (caches.TryGetValue(typeof(TViewModel), out var events))
            {
                var hub = events.FirstOrDefault(e => e.Token == token);
                if (hub == null)
                {
                    hub = new EventHub<TMessage>
                    {
                        Token = token,
                        Action = action,
                    };
                    events.Add(hub);
                }
                else
                {
                    (hub as EventHub<TMessage>).Action = action;
                }
            }
            else
            {
                events = new List<EventHub>
                {
                    new EventHub<TMessage>
                    {
                        Token = token,
                        Action = action,
                    }
                };
                caches.Add(typeof(TViewModel), events);
            }
        }

        internal static void Send<TTarget, TMessage>(string token,TMessage msg)
        {
            if (caches.TryGetValue(typeof(TTarget), out var events))
            {
                (events.FirstOrDefault(e => e.Token == token) as EventHub<TMessage>)?.Action.Invoke(msg);
            }
        }
    }
}
