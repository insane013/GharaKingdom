using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class FloatingText : MonoBehaviour
{
    public enum FloatingTextType { damage, miss}

    [SerializeField] private Color _missColor;
    [SerializeField] private Color _damageColor;

    private Animator _animator;
    private TextMeshProUGUI _textComponent;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _textComponent = GetComponent<TextMeshProUGUI>();
    }
    public void SetText(string text, FloatingTextType type)
    {
        this.gameObject.SetActive(true);

        _textComponent.text = text;

        if (type == FloatingTextType.damage) _textComponent.color = _damageColor;
        if (type == FloatingTextType.miss) _textComponent.color = _missColor;

        Animate();
    }

    private void Animate()
    {
        _animator.SetTrigger("animate");
    }

    private void Fade()
    {
        Destroy(this.gameObject);
    }
}
