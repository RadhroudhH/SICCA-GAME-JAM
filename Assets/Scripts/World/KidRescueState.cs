using UnityEngine;
using System;

public class KidRescueState : MonoBehaviour
{
    public bool IsRescued { get; private set; }
    public event Action OnKidRescued;

    public void SetRescued()
    {
        if (IsRescued) return;

        IsRescued = true;
        OnKidRescued?.Invoke();
        Debug.Log("Kid has been rescued!");
    }
}
