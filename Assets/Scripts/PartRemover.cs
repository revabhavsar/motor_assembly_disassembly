using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRGrabInteractable))]
public class PartRemover : MonoBehaviour
{
    public DisassemblyManager.PartType partType;

    private XRGrabInteractable grabInteractable;
    private bool notified = false;

    private void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
    }

    private void OnEnable()
    {
        grabInteractable.selectExited.AddListener(OnReleased);
    }

    private void OnDisable()
    {
        grabInteractable.selectExited.RemoveListener(OnReleased);
    }

    private void OnReleased(SelectExitEventArgs args)
    {
        if (notified)
            return;

        notified = true;

        if (DisassemblyManager.Instance != null)
        {
            DisassemblyManager.Instance.PartRemoved(partType);
        }

        Debug.Log(partType + " Removed");
    }
}