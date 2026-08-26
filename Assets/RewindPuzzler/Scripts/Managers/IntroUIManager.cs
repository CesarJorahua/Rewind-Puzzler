using System.Collections.Generic;
using RewindPuzzler.Core.EventBus;
using UnityEngine;
using UnityEngine.Serialization;

public class IntroUIManager : MonoBehaviour
{
    [SerializeField] List<GameObject> playerCharacterList;
    [SerializeField] Transform introUIParent;
    [SerializeField] InputClientManager inputClientManager;

    private int _currentPlayerListIndex;
    private GameObject _currentPlayerSelected;
    private int _characterCount;

    private EventBinding<CharacterSelect> _characterSelectbinding;

    void Start()
    {
        _characterCount = playerCharacterList.Count-1;
        SetCharacter();
    }

    private void OnEnable()
    {
        _characterSelectbinding = new (SetCharacter);
        EventBus<CharacterSelect>.Register(_characterSelectbinding);
    }

    private void OnDisable()
    {
        EventBus<CharacterSelect>.Deregister(_characterSelectbinding);        
    }

    public void NextCharacter()
    {
        if (_currentPlayerListIndex<_characterCount)
        {
            _currentPlayerListIndex++;
        }
        else
        {
            _currentPlayerListIndex = 0;
        }
        SetCharacter();
    }

    public void PrevCharacter()
    {
        if (_currentPlayerListIndex>0)
        {
            _currentPlayerListIndex--;
        }
        else
        {
            _currentPlayerListIndex = _characterCount;
        }
        SetCharacter();   
    }

    public void SetCharacter()
    {
        if(_currentPlayerSelected)
            Destroy(_currentPlayerSelected);
        _currentPlayerSelected = Instantiate(playerCharacterList[_currentPlayerListIndex]);
        ConfigurePlayerPosition(_currentPlayerSelected.transform);
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
        MovementReciever component = _currentPlayerSelected.AddComponent<MovementReciever>();
        component.SetObstacleMask(LayerMask.GetMask("Obstacle"));
        introUIParent.parent.gameObject.SetActive(false);
        _currentPlayerSelected.transform.SetParent(null);
        _currentPlayerSelected.transform.localScale = Vector3.one;
        inputClientManager.MovementReciever = component;
        component.EnableMovement();

        inputClientManager.Reset();
    }

}
