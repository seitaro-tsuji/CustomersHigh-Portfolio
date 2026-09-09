using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class SpecialActionButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public event Action Pressed;
    public event Action Held;
    public event Action Released;

    private bool isHolding;
    public bool IsHolding => isHolding;

    private Button _button;
    private bool _isActive;

    private void Awake()
    {
        _button = GetComponent<Button>();
        _isActive = true;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if(_isActive)
        {
            isHolding = true;
            Pressed?.Invoke();
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (_isActive)
        {
            isHolding = false;
            Released?.Invoke();
        }
    }

    private void Update()
    {
        if (isHolding)
        {
            Held?.Invoke();
        }
    }

    public Button GetButton()
    {
        return _button;
    }

    public void Disable()
    {
        TextMeshProUGUI text = GetComponentInChildren<TextMeshProUGUI>();
        Color color = text.color;

        _isActive = false;
        _button.interactable = false;
        color.a = 0.5f;
        text.color = color;
    }
}
