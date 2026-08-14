using System.Collections.Generic;
using RewindPuzzler.Core.EventBus;
using UnityEngine;
using UnityEngine.Serialization;

public class IntroUIManager : MonoBehaviour
{
    [SerializeField] List<GameObject> playerCharacterList;
    [SerializeField] Transform introUIParent;
    [SerializeField] InputClientManager inputClientManager;

    private int currentPlayerListIndex;
    private GameObject currentPlayerSelected;
    private int characterCount;

    private EventBinding<CharacterSelect> characterSelectbinding;

    void Start()
    {
        characterCount = playerCharacterList.Count-1;
        SetCharacter();
    }

    private void OnEnable()
    {
        characterSelectbinding = new (SetCharacter);
        EventBus<CharacterSelect>.Register(characterSelectbinding);
    }

    private void OnDisable()
    {
        EventBus<CharacterSelect>.Deregister(characterSelectbinding);        
    }

    public void NextCharacter()
    {
        if (currentPlayerListIndex<characterCount)
        {
            currentPlayerListIndex++;
        }
        else
        {
            currentPlayerListIndex = 0;
        }
        SetCharacter();
    }

    public void PrevCharacter()
    {
        if (currentPlayerListIndex>0)
        {
            currentPlayerListIndex--;
        }
        else
        {
            currentPlayerListIndex = characterCount;
        }
        SetCharacter();   
    }

    public void SetCharacter()
    {
        if(currentPlayerSelected)
            Destroy(currentPlayerSelected);
        currentPlayerSelected = Instantiate(playerCharacterList[currentPlayerListIndex]);
        ConfigurePlayerPosition(currentPlayerSelected.transform);
    }

    private void ConfigurePlayerPosition(Transform playerTransform)
    {
        playerTransform.SetParent(introUIParent);
        playerTransform.localScale = Vector3.one * 90f;
        playerTransform.localPosition = Vector3.down * 100f;
    }

    public void PlayButton()
    {
        EventBus<GameStarted>.Raise(new GameStarted());
        PlayButtonHit();
    }

    private void PlayButtonHit()
    {
        MovementReciever component = currentPlayerSelected.AddComponent<MovementReciever>();
        component.SetObstacleMask(LayerMask.GetMask("Obstacle"));
        introUIParent.parent.gameObject.SetActive(false);
        currentPlayerSelected.transform.SetParent(null);
        currentPlayerSelected.transform.localScale = Vector3.one;
        inputClientManager.MovementReciever = component;
        component.EnableMovement();

        inputClientManager.Reset();
    }

}
