using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour
{
    public Image BackGround;
    public TextMeshProUGUI HeaderField;
    public TextMeshProUGUI ContentField;
    public LayoutElement LayoutElement;
    public ContentSizeFitter ContentSizeFitter;
    public VerticalLayoutGroup VerticalLayoutGroup;
    public int CharacterWrapLimit;
    public RectTransform RectTransform;

    public void Start()
    {
        RectTransform.localScale = new Vector3(1, 1, 1);
    }

    public void SetText(string content = "", string header = "")
    {
        if (header == null) { header = ""; };
        if (content == null) { content = ""; };

        UpdateContentBox(header.Length, content.Length);
        UpdatePivot();

        HeaderField.gameObject.SetActive(!string.IsNullOrEmpty(header));
        HeaderField.text = header;
        ContentField.text = content;
    }

    public void Update()
    {
        transform.position = Input.mousePosition;
        ContentSizeFitter.enabled = true;
    }

    public void UpdatePivot()
    {
        var position = Input.mousePosition;

        var pivotX = position.x / Screen.width;
        var pivotY = position.y / Screen.height;

        // standaard normale tooltip, tenzij aan uiteinde van scherm
        if (pivotX < 0.9)
        {
            pivotX = 0;
        }
        if (pivotY > 0.1)
        {
            pivotY = 1;
        }

        RectTransform.pivot = new Vector2(pivotX, pivotY);
        transform.position = position;
    }

    public void UpdateContentBox(int headerLength, int contentLength)
    {
        ContentSizeFitter.enabled = true;

        var needsExpension = (headerLength > CharacterWrapLimit || contentLength > CharacterWrapLimit) ? true : false;
        if(LayoutElement.enabled != needsExpension)
        {
            ContentSizeFitter.enabled = false; // enabled bij volgende tik -> forceert verandering (Vraag me niet waarom... )
            LayoutElement.enabled = needsExpension;

            if(needsExpension)
            {
                VerticalLayoutGroup.padding.top = 30;
                VerticalLayoutGroup.padding.bottom = 30;
            }
            else
            {
                VerticalLayoutGroup.padding.top = 10;
                VerticalLayoutGroup.padding.bottom = 10;
            }
        }
    }
}