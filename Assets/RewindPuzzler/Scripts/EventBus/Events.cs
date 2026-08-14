using UnityEngine;

namespace RewindPuzzler.Core.EventBus
{
    public interface IEvent { }
    public struct PlaySfxEvent : IEvent
    {
        public string Type;
        public Vector3 Position;
    }

    public struct GameStarted: IEvent {}
    public struct ReachEndMaze: IEvent {}
    public struct ResetMaze: IEvent {}
    public struct CharacterSelect: IEvent {}
}