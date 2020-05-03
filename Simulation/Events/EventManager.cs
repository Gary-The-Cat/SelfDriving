using System;
using System.Collections.Generic;
using System.Linq;

namespace CarSimulation.Events
{
    public static class EventManager
    {
        public static Dictionary<Type, List<Event>> lookup { get; set; } = new Dictionary<Type, List<Event>>();

        public static void Listen<T>(Action<T> action)
        {
            var type = typeof(T);
            if (!lookup.ContainsKey(type))
            {
                lookup.Add(type, new List<Event>());
            }

            lookup[type].Add(new Event<T> { Callback = action });
        }

        public static void Remove<T>(Action<T> action)
        {
            var type = typeof(T);
            var e = lookup[type].First(ev => ((Event<T>)ev).Callback == action);
            lookup[type].Remove(e);
        }

        public static void Trigger<T>(T item = default(T))
        {
            if(lookup.TryGetValue(typeof(T), out var actions))
            {
                foreach(var action in actions)
                {
                    ((Event<T>)action).Callback(item);
                }
            }
        }

        public static void Enqueue<T>(T item)
        {
            if (lookup.TryGetValue(typeof(T), out var actions))
            {
                foreach (var action in actions)
                {
                    ((Event<T>)action).Callback(item);
                }
            }
        }
    }
}
