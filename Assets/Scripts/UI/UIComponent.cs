using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using UnityEngine.EventSystems;

[RequireComponent(typeof(CanvasGroup))]
public abstract class UIComponent : MonoBehaviour, ISelectHandler, IDeselectHandler {
    [Header("General")]
    protected CanvasGroup canvasGroup;
    public FXRequest highlightFX;
    public Vector3 highlightScale = Vector3.one;
    public Ease highlightEase = Ease.Linear;
    public float highlightDuration = 1;
    private Vector3 startingScale = Vector3.one;
    public Ease unHighlightEase = Ease.Linear;
    public float unHighlightDuration = 1;
    private Tween scaleTween;

    [Header("DoAction")]
    public bool isSelectable;
    protected Selectable selectable;
    public UnityEvent function;
    public FXRequest doActionFX;

    private void Awake() {
        startingScale = transform.localScale;

        Init();
        if (isSelectable) {
            selectable = gameObject.GetComponent<Selectable>();
            if (selectable == null) {
                selectable = gameObject.AddComponent<Selectable>();
            }
        }
    }

    //debug
    //private void Update() {
    //    if (scaleTween == null) {
    //        return;
    //    }
    //    Debug.Log(scaleTween.id, gameObject);
    //}

    public abstract void Configure();

    private void Init() {
        canvasGroup = GetComponent<CanvasGroup>();
        Configure();
    }

    //Called in the Editor only, triggered when something changes or when you hit Save
    //By having this on a setup function, we can edit and test new data in live-time 
    private void OnValidate() {
        Init();
    }

    public void SelectableHandling(bool toggleInteractivity) {
        if (!isSelectable) {
            return;
        }

        selectable.interactable = toggleInteractivity;
    }

    public void Highlight() {
        scaleTween.Kill();
        if (highlightDuration > 0) {
            scaleTween = transform.DOScale(highlightScale, highlightDuration).SetEase(highlightEase).SetUpdate(true);
        }
        else {
            transform.localScale = highlightScale;
        }

        highlightFX.Play(gameObject);
    }

    public void UnHighlight() {
        scaleTween.Kill();
        if (unHighlightDuration > 0) {
            scaleTween = transform.DOScale(startingScale, unHighlightDuration).SetEase(unHighlightEase).SetUpdate(true);
        }
        else {
            transform.localScale = startingScale;
        }
    }

    public virtual void DoAction() {
        if (!selectable) {
            return;
        }
        if (function == null) {
            return;
        }

        doActionFX.Play(gameObject);
        function.Invoke();
    }

    public void OnSelect(BaseEventData eventData) {
        Highlight();
    }

    public void OnDeselect(BaseEventData eventData) {
        UnHighlight();
    }
}