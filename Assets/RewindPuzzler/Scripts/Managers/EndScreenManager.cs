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
    [SerializeField] private GameObject endScreenGameObject;

    private EndMessageList _endMessageData;
    private EventBinding<ReachEndMaze> _endMazeBinding;
    private EventBinding<ResetMaze> _resetMazeBinding;
    private string _titleRng;
    private string _bodyRng;

    private EndMessageData _randomPhraseTitle;
    private EndMessageData _randomPhraseBody;


    private void OnEnable()
    {
        _endMazeBinding =  new EventBinding<ReachEndMaze>(OnPlayerEndMaze);
        _resetMazeBinding =  new EventBinding<ResetMaze>(OnResetMaze);
        EventBus<ReachEndMaze>.Register(_endMazeBinding);
        EventBus<ResetMaze>.Register(_resetMazeBinding);
    }

    private void OnDisable()
    {
        EventBus<ReachEndMaze>.Deregister(_endMazeBinding);
        EventBus<ResetMaze>.Deregister(_resetMazeBinding);
    }

    private void Start()
    {
        GenerateRngMessage();
    }

    private void GenerateRngMessage()
    {
        string json = Resources.Load<TextAsset>("end_messages").text;
        _endMessageData =  JsonUtility.FromJson<EndMessageList>(json);
        _randomPhraseTitle = _endMessageData.endPhrases[Random.Range(0, _endMessageData.endPhrases.Count)];
        _randomPhraseBody = _endMessageData.endPhrases[Random.Range(0, _endMessageData.endPhrases.Count)];
        _bodyRng = _randomPhraseBody.body;
        _titleRng = _randomPhraseTitle.title;
    }

    private void OnPlayerEndMaze()
    {
        // Ensure the EndScreenGameObject is active immediately
        endScreenGameObject.SetActive(true);

        // Set text first
        titleText.text = _titleRng;
        bodyText.GetComponent<TypewriteEffect>().SetText(_bodyRng);

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
        GenerateRngMessage();
        endScreenGameObject.SetActive(false);
    }

    public void OnCharacterSelectButton()
    {
        EventBus<CharacterSelect>.Raise(new CharacterSelect());
    }

}
