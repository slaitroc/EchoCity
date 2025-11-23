using System;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    protected string _INTERACTABLE_LOG_TAG = "INTERACTABLE";
    protected abstract string _TYPE_LOG_TAG { get; }
    protected abstract string _LOG_TAG { get; }
    protected string _LOG_TAG_FULL => $"{_TYPE_LOG_TAG}-{_INTERACTABLE_LOG_TAG}:::{_LOG_TAG}";
    protected string _LOG_COLOR = "#ff5733ff";

    public abstract void Interact();
}
