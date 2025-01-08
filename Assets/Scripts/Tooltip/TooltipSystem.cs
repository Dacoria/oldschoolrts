using System;
using System.Collections;
using UnityEngine;

public class TooltipSystem : MonoBehaviour
{
    public static TooltipSystem Instance;
    public Tooltip Tooltip;

    private float waitTimeToShow = 0.5f;
    private float fadeInTime = 0.355f;
    private float fadeOutTime = 0.25f;

    public void Awake()
    {
        Instance = this;
    }    

    private Color colorBackground;
    private Color colorHeader;
    private Color colorContent;

    void Start()
    {
        Tooltip.gameObject.SetActive(true);
        colorBackground = Tooltip.BackGround.color;
        colorHeader = Tooltip.HeaderField.color;
        colorContent = Tooltip.ContentField.color;

        // start invisible
        Tooltip.BackGround.color = new Color(colorBackground.r, colorBackground.g, colorBackground.b, 0);
        Tooltip.HeaderField.color = new Color(colorHeader.r, colorHeader.g, colorHeader.b, 0);
        Tooltip.ContentField.color = new Color(colorContent.r, colorContent.g, colorContent.b, 0);
    }      

    private DateTime showTimeTooltip;

    public void Show(string content = "", string header = "", bool waitBeforeShowing = true, GameObject activeTooltipGo = null)
    {
        ActiveTooltipGo = activeTooltipGo;
        StopAllCoroutines();
        Instance.Tooltip.SetText(content, header);
        showTimeTooltip = DateTime.Now;        

        if (waitBeforeShowing)
        {
            Instance.StartCoroutine(Instance.ShowAfterXSeconds());
        }
        else
        {
            Instance.StartCoroutine(Instance.FadeIn());
        }
    }

    private GameObject ActiveTooltipGo;

    public bool UpdateText(GameObject activeTooltipGo, string content, string header = "")
    {
        // voorkomt dat 2 updates tegelijkertijd bezig zijn (wat kan via een event + rayhit)
        if(activeTooltipGo == ActiveTooltipGo)
        {
            Instance.Tooltip.SetText(content, header);
            return true;
        }
        return false;
    }

    public IEnumerator ShowAfterXSeconds()
    {
        yield return Wait4Seconds.Get(waitTimeToShow);
        Instance.StartCoroutine(Instance.FadeIn());
    }

    public void Hide(bool ignoreWaitBuffer = false)
    {
        // stel je klikt van 1 tooltip op een persoon naar de volgende --> dan moet de tooltip blijven (uitzondering: hover)
        if (ignoreWaitBuffer || (DateTime.Now - showTimeTooltip).TotalMilliseconds > 50)
        {
            StopAllCoroutines();
            Instance.StartCoroutine(Instance.FadeOut());
        }
    }

    private IEnumerator FadeOut()
    {
        for (float a = (Tooltip.BackGround.color.a * fadeOutTime); a >= 0; a -= Time.deltaTime)
        {
            Tooltip.BackGround.color = new Color(colorBackground.r, colorBackground.g, colorBackground.b, a / fadeOutTime);
            Tooltip.HeaderField.color = new Color(colorHeader.r, colorHeader.g, colorHeader.b, a / fadeOutTime);
            Tooltip.ContentField.color = new Color(colorContent.r, colorContent.g, colorContent.b, a / fadeOutTime);

            yield return null;
        }
    }

    private IEnumerator FadeIn()
    {
        var initialA = Tooltip.BackGround.color.a;

        for (float a = 0; a <= fadeInTime; a += Time.deltaTime)
        {
            Tooltip.BackGround.color = new Color(colorBackground.r, colorBackground.g, colorBackground.b, Math.Min(1, initialA + (a / fadeInTime)));
            Tooltip.HeaderField.color = new Color(colorHeader.r, colorHeader.g, colorHeader.b, Math.Min(1, initialA + (a / fadeInTime)));
            Tooltip.ContentField.color = new Color(colorContent.r, colorContent.g, colorContent.b, Math.Min(1, initialA + (a / fadeInTime)));
            yield return null;
        }
    }
}