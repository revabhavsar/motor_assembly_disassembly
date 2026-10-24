using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRGrabInteractable))]
public class PartRemover : MonoBehaviour
{
    public DisassemblyManager.PartType partType;

    private XRGrabInteractable grab;
    private bool removed = false;

    private void Awake()
    {
        grab = GetComponent<XRGrabInteractable>();
    }

    private void OnEnable()
    {
        grab.selectExited.AddListener(OnReleased);
    }

    private void OnDisable()
    {
        grab.selectExited.RemoveListener(OnReleased);
    }

    private void OnReleased(SelectExitEventArgs args)
    {
        if (removed)
            return;

        if (DisassemblyManager.Instance == null)
            return;

        // Check correct sequence
        if (!DisassemblyManager.Instance.CanRemovePart(partType))
        {
            Debug.Log(
                "LOCKED: " +
                partType +
                " cannot be removed yet!"
            );

            return;
        }

        removed = true;

        Debug.Log(partType + " REMOVED");

        DisassemblyManager.Instance.PartRemoved(
            partType
        );
    }
}