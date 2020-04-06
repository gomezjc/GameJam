using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ButtonSound : MonoBehaviour
{
    public Color hoverColor;
    public Color originalTextcolor;
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
        SoundManager.instance.PlaySound("buttonMenu");
        foreach (var text in textCollection)
        {
            text.color = Color.white;
        }
    }

    public void onHoverText()
    {
        _rectTransform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
        SoundManager.instance.PlaySound("buttonIntro");
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
    
    public void onHoverDialog()
    {
        _rectTransform.localScale = new Vector3(1.1f, 1f, 1f);
        SoundManager.instance.PlaySound("buttonIntro");
    }
    
    public void unHoverDialog()
    {
        _rectTransform.localScale = new Vector3(1f, 1f, 1f);
    }
}