using System.Collections.Generic;
using DG.Tweening;
using RewindPuzzler.Core.EventBus;
using TMPro;
using UnityEngine;

[System.Serializable]
public class EndMessageList
{
    public List<EndMessageData> endPhrases;
}

public class EndScreenManager : MonoBehaviour
{
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text bodyText;
    [SerializeField] private AudioSource bgAudio;
    [SerializeField] private GameObject EndScreenGameObject;

    private EndMessageList endMessageData;
    private EventBinding<ReachEndMaze> endMazeBinding;
    private EventBinding<ResetMaze> resetMazeBinding;
    private string titleRNG;
    private string bodyRNG;

    private EndMessageData randomPhraseTitle;
    private EndMessageData randomPhraseBody;


    private void OnEnable()
    {
        endMazeBinding =  new EventBinding<ReachEndMaze>(OnPlayerEndMaze);
        resetMazeBinding =  new EventBinding<ResetMaze>(OnResetMaze);
        EventBus<ReachEndMaze>.Register(endMazeBinding);
        EventBus<ResetMaze>.Register(resetMazeBinding);
    }

    private void OnDisable()
    {
        EventBus<ReachEndMaze>.Deregister(endMazeBinding);
        EventBus<ResetMaze>.Deregister(resetMazeBinding);
    }

    private void Start()
    {
        GenerateRNGMessage();
    }

    private void GenerateRNGMessage()
    {
        string json = Resources.Load<TextAsset>("end_messages").text;
        endMessageData =  JsonUtility.FromJson<EndMessageList>(json);
        randomPhraseTitle = endMessageData.endPhrases[Random.Range(0, endMessageData.endPhrases.Count)];
        randomPhraseBody = endMessageData.endPhrases[Random.Range(0, endMessageData.endPhrases.Count)];
        bodyRNG = randomPhraseBody.body;
        titleRNG = randomPhraseTitle.title;
    }

    private void OnPlayerEndMaze()
    {
        // Ensure the EndScreenGameObject is active immediately
        EndScreenGameObject.SetActive(true);

        // Set text first
        titleText.text = titleRNG;
        bodyText.GetComponent<TypewriteEffect>().SetText(bodyRNG);

        // Delay starting the effect slightly to ensure initialization
        StartCoroutine(DelayStartEffect());
    }

    private System.Collections.IEnumerator DelayStartEffect()
    {
        // Wait one frame to ensure all systems are initialized
        yield return null;
        bodyText.GetComponent<TypewriteEffect>().StartEffect();
    }

    private void OnResetMaze()
    {
        GenerateRNGMessage();
        EndScreenGameObject.SetActive(false);
    }

    public void OnCharacterSelectButton()
    {
        EventBus<CharacterSelect>.Raise(new CharacterSelect());
    }

}
