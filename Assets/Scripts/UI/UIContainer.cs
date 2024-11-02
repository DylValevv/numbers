using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;
using System.Collections.Generic;
using UnityEditor;

[RequireComponent(typeof(CanvasGroup))]
public class UIContainer : MonoBehaviour {
    [Header("Layout Attributes")]
    protected CanvasGroup canvasGroup;
    public bool showOnStart = false;
    protected List<UIComponent> uiComponents = new List<UIComponent>();

    [Header("Fade Attributes")]
    public float fadeInDuration;
    public Ease fadeInEase;
    public float fadeOutDuration;
    public Ease fadeOutEase;
    [Header("Close Attributes")]
    public bool cancelCanClose;
    public UIContainer openContainerOnCancel;
    private Tween fadeTween;
    public UnityEvent functionOnClose;

    protected bool initialized;

    private void Start() {
        Setup();
    }

    public void Setup() {
        canvasGroup = GetComponent<CanvasGroup>();
        UIComponent[] uiComponentChildren = transform.GetComponentsInChildren<UIComponent>(transform);//there are three layout group objects where UIComponents can go, so this is preferred as opposed to a direct reference to a parent transform's children
        foreach (UIComponent uiComponent in uiComponentChildren) {
            uiComponents.Add(uiComponent);
            uiComponent.Configure();
        }
        if (uiComponents.Count == 0) {
            Debug.LogWarning("No UI Components in the 'uiComponents' list!", gameObject);
        }

        Show(showOnStart);

        initialized = true;
    }

    public virtual void ShowInstant(bool show) {
        canvasGroup.interactable = show;
        canvasGroup.blocksRaycasts = show;
        canvasGroup.alpha = show ? 1 : 0;
        foreach (UIComponent uiComponent in uiComponents) {
            uiComponent.SelectableHandling(show);
        }
    }

    public virtual void Show(bool show) {
        canvasGroup.interactable = show;
        canvasGroup.blocksRaycasts = show;
        fadeTween.Kill();

        if (show) {
            if (fadeInDuration > 0) {
                fadeTween = DOTween.To(() => canvasGroup.alpha, x => canvasGroup.alpha = x, 1, fadeInDuration).SetEase(fadeInEase).SetUpdate(true);
            }
            else {
                canvasGroup.alpha = show ? 1 : 0;
            }
        }
        else {
            if (fadeOutDuration > 0) {
                fadeTween = DOTween.To(() => canvasGroup.alpha, x => canvasGroup.alpha = x, 0, fadeOutDuration).SetEase(fadeOutEase).SetUpdate(true);
            }
            else {
                canvasGroup.alpha = show ? 1 : 0;
            }
        }

        foreach (UIComponent child in uiComponents) {
            UIComponent menuItem = child.GetComponent<UIComponent>();
            menuItem.SelectableHandling(show);
        }
    }

    //instantly goes to the assigned menu "behind"
    protected void Back() {
        if (cancelCanClose && IsShowing()) {
            ShowInstant(false);

            if (openContainerOnCancel) {
                openContainerOnCancel.ShowInstant(true);
            }

            functionOnClose.Invoke();
        }
    }

    public bool IsShowing() {
        if (canvasGroup == null) {
            return false;
        }
        return canvasGroup.interactable;
    }

    public void Pause(bool pause) {
        GameManager.Instance.PauseGame(pause);
    }
}