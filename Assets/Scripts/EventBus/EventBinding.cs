using System;

namespace RewindPuzzler.Core.EventBus
{
    public interface IEventBinding<T> {
        public Action<T> OnEvent { get; set; }
        public Action OnEventNoArgs { get; set; }
        public Func<T,bool> Filter{get;}
    }

    public class EventBinding<T> : IEventBinding<T> where T : IEvent {
        Action<T> _onEvent = _ => { };
        Action _onEventNoArgs = () => { };


        Action<T> IEventBinding<T>.OnEvent {
            get => _onEvent;
            set => _onEvent = value;
        }

        Action IEventBinding<T>.OnEventNoArgs {
            get => _onEventNoArgs;
            set => _onEventNoArgs = value;
        }

        Func<T,bool> _filter;
        public Func<T, bool> Filter => _filter;
        public EventBinding(Action<T> onEvent, Func<T, bool> filter = null) {
            this._onEvent = onEvent;
            this._filter = filter;
            }
        public EventBinding(Action onEventNoArgs, Func<T, bool> filter = null) {
            this._onEventNoArgs = onEventNoArgs;
            this._filter = filter;
            }

        public void Add(Action onEvent) => _onEventNoArgs += onEvent;
        public void Remove(Action onEvent) => _onEventNoArgs -= onEvent;
    
        public void Add(Action<T> onEvent) => this._onEvent += onEvent;
        public void Remove(Action<T> onEvent) => this._onEvent -= onEvent;
    }
}