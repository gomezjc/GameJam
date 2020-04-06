using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ButtonSound : MonoBehaviour
{
    public Color hoverColor;
    public Color originalTextcolor;
    public AudioSource source;
    public AudioClip hoverButton;
    
    private Image image;
    private RectTransform _rectTransform;
    public TextMeshProUGUI[] textCollection;

    private void Start()
    {
        textCollection = GetComponentsInChildren<TextMeshProUGUI>();
        _rectTransform = GetComponent<RectTransform>();
        image = GetComponent<Image>();
    }

    public void onHover()
    {
        _rectTransform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
        image.color = hoverColor;
        source.PlayOneShot(hoverButton);
        foreach (var text in textCollection)
        {
            text.color = Color.white;
        }
    }

    public void onHoverText()
    {
        _rectTransform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
        source.PlayOneShot(hoverButton);
    }
    
    public void unHoverText()
    {
        _rectTransform.localScale = new Vector3(1f, 1f, 1f);
    }

    public void unHover()
    {
        _rectTransform.localScale = new Vector3(1f, 1f, 1f);
        image.color = Color.white;
        foreach (var text in textCollection)
        {
            text.color = originalTextcolor;
        }
    }
}