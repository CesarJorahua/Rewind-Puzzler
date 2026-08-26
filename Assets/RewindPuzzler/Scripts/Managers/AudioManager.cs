using UnityEngine;
using RewindPuzzler.Core.EventBus;
using System;
using DG.Tweening;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    private AudioSource _bgmAudio;

    private EventBinding<ReachEndMaze> _playerEndMazeBinding;
    private EventBinding<ResetMaze> _resetMazeBiding;

    private void Start()
    {
        _bgmAudio = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        _playerEndMazeBinding =  new EventBinding<ReachEndMaze>(OnPlayerEndMaze);
        _resetMazeBiding =  new EventBinding<ResetMaze>(OnResetMaze);
        EventBus<ReachEndMaze>.Register(_playerEndMazeBinding);
        EventBus<ResetMaze>.Register(_resetMazeBiding);
    }

    private void OnPlayerEndMaze()
    {
        _bgmAudio.DOFade(0,1f);
    }

    private void OnResetMaze()
    {
        _bgmAudio.DOFade(1f,1f);        
    }

    private void OnDisable()
    {
        EventBus<ReachEndMaze>.Deregister(_playerEndMazeBinding);
        EventBus<ResetMaze>.Deregister(_resetMazeBiding);
    }
}
