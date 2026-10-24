using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRGrabInteractable))]
public class ObjectHighlight : MonoBehaviour
{
    public Color hoverColor = Color.orange;
    public Color grabColor = Color.green;

    private XRGrabInteractable interactable;
    private Renderer objectRenderer;

    private Color originalColor;

    private void Awake()
    {
        interactable = GetComponent<XRGrabInteractable>();
        objectRenderer = GetComponent<Renderer>();

        if (objectRenderer != null)
        {
            originalColor = objectRenderer.material.color;
        }
    }

    private void OnEnable()
    {
        interactable.hoverEntered.AddListener(OnHoverEntered);
        interactable.hoverExited.AddListener(OnHoverExited);

        interactable.selectEntered.AddListener(OnSelectEntered);
        interactable.selectExited.AddListener(OnSelectExited);
    }

    private void OnDisable()
    {
        interactable.hoverEntered.RemoveListener(OnHoverEntered);
        interactable.hoverExited.RemoveListener(OnHoverExited);

        interactable.selectEntered.RemoveListener(OnSelectEntered);
        interactable.selectExited.RemoveListener(OnSelectExited);
    }

    private void OnHoverEntered(HoverEnterEventArgs args)
    {
        objectRenderer.material.color = hoverColor;
    }

    private void OnHoverExited(HoverExitEventArgs args)
    {
        if (!interactable.isSelected)
            objectRenderer.material.color = originalColor;
    }

    private void OnSelectEntered(SelectEnterEventArgs args)
    {
        objectRenderer.material.color = grabColor;
    }

    private void OnSelectExited(SelectExitEventArgs args)
    {
        objectRenderer.material.color = originalColor;
    }
}