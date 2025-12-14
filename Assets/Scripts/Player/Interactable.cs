using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    [Header("UI Indicators")]
    [SerializeField] protected List<GameObject> indicatorUIs = new();
    [SerializeField] private bool hideAtStart = true;

    protected virtual void Start()
    {
        if (hideAtStart)
        {
            foreach (var ui in indicatorUIs)
                if (ui) ui.SetActive(false);
        }
    }

    public void ShowIndicators()
    {
        foreach (var ui in indicatorUIs)
            if (ui) ui.SetActive(true);
    }

    public void HideIndicators()
    {
        foreach (var ui in indicatorUIs)
            if (ui) ui.SetActive(false);
    }

    public virtual void Interact() { }
}
