using UnityEngine;
using RewindPuzzler.Core.EventBus;
using System;
using DG.Tweening;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    private AudioSource BGMAudio;

    private EventBinding<ReachEndMaze> playerEndMazeBinding;
    private EventBinding<ResetMaze> resetMazeBiding;

    private void Start()
    {
        BGMAudio = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        playerEndMazeBinding =  new EventBinding<ReachEndMaze>(OnPlayerEndMaze);
        resetMazeBiding =  new EventBinding<ResetMaze>(OnResetMaze);
        EventBus<ReachEndMaze>.Register(playerEndMazeBinding);
        EventBus<ResetMaze>.Register(resetMazeBiding);
    }

    private void OnPlayerEndMaze()
    {
        BGMAudio.DOFade(0,1f);
    }

    private void OnResetMaze()
    {
        BGMAudio.DOFade(1f,1f);        
    }

    private void OnDisable()
    {
        EventBus<ReachEndMaze>.Deregister(playerEndMazeBinding);
        EventBus<ResetMaze>.Deregister(resetMazeBiding);
    }
}
