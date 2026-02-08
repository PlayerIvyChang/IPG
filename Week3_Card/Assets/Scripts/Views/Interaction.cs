using UnityEngine;

public class Interaction : Singleton<Interaction>
{
    public bool IsDragging { get; set; } = false;

    public bool CanInteract()
    {
        if (!ActionSystem.Instance.IsPerforming) return true;
        else return false;
    }
    public bool CanHover()
    {
        if (IsDragging) return false;
        else return true;
    }
}
