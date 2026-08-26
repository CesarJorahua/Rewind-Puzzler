using System.Collections;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class TypewriteEffect : MonoBehaviour
{
    [Header("Effect Settings")]
    [SerializeField]
    private float speed = 20;

    [SerializeField]
    private float punctuationDelay = 0.5f;

    private TMP_Text _textBox;
    private int _currentVisibleCharIndex;
    private Coroutine _typewriteCoroutine;

    private WaitForSeconds _simpleDelay;
    private WaitForSeconds _punctuationDelay;

    private bool _isInitialized;

    private void Initialize()
    {
        _isInitialized = true;
        _textBox = GetComponent<TMP_Text>();
        _simpleDelay = new WaitForSeconds(1 / speed);
        _punctuationDelay = new WaitForSeconds(punctuationDelay);
    }

    public void SetText(string text)
    {
        if(!_isInitialized)
            Initialize();

        if (_typewriteCoroutine != null)
        {
            StopCoroutine(_typewriteCoroutine);
        }

        _textBox.text = text;
        _textBox.maxVisibleCharacters = 0;
        _currentVisibleCharIndex = 0;        
    }

    public void StartEffect()
    {
        _typewriteCoroutine = StartCoroutine(TypeWritter());
    }

    private IEnumerator TypeWritter()
    {
        TMP_TextInfo textInfo = _textBox.textInfo;
        while (_currentVisibleCharIndex < textInfo.characterCount + 1)
        {
            char character = textInfo.characterInfo[_currentVisibleCharIndex].character;
            _textBox.maxVisibleCharacters++;
            if (
                character == '?'
                || character == '.'
                || character == ','
                || character == ':'
                || character == ';'
                || character == '!'
                || character == '-'
            )
            {
                yield return _punctuationDelay;
            }
            else
                yield return _simpleDelay;
        }
    }
}
