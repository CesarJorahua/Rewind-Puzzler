using System.Collections.Generic;
using UnityEngine;

namespace RewindPuzzler.Core.EventBus
{
    public static class EventBus<T> where T : IEvent {
        static readonly List<IEventBinding<T>> _buffer = new();
        static bool _dirty = true;
        static readonly HashSet<IEventBinding<T>> Bindings = new HashSet<IEventBinding<T>>();

        public static void Register(EventBinding<T> b)
        {
            Bindings.Add(b);   
            _dirty = true;
        }

        public static void Deregister(EventBinding<T> b)
        {
            Bindings.Remove(b); 
            _dirty = true;
        }

        public static void Raise(T @event)
        {
            if (_dirty) { _buffer.Clear(); _buffer.AddRange(Bindings); _dirty = false; }
            for (int i = 0; i < _buffer.Count; i++)
            {
                var binding = _buffer[i];
                if (!Bindings.Contains(binding)) continue;
                if(binding.Filter != null && !binding.Filter(@event)) continue;
                // Both default to no-op lambdas, so invoking both supports bindings made
                // with either the Action<T> or the parameterless constructor.
                binding.OnEvent.Invoke(@event);
                binding.OnEventNoArgs.Invoke();
            }
        }

        public static void Clear() {
            Debug.Log($"Clearing {typeof(T).Name} bindings");
            Bindings.Clear();
        }
    }
}
