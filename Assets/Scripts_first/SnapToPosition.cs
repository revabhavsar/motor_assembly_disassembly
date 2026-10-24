using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRGrabInteractable))]
[RequireComponent(typeof(Rigidbody))]
public class SnapToPosition : MonoBehaviour
{
    public Transform snapPoint;

    private XRGrabInteractable grab;
    private Rigidbody rb;

    private void Awake()
    {
        grab = GetComponent<XRGrabInteractable>();
        rb = GetComponent<Rigidbody>();

        // IMPORTANT:
        // Parts must not be affected by physics.
        rb.isKinematic = true;
        rb.useGravity = false;
    }

    private void OnEnable()
    {
        grab.selectExited.AddListener(OnRelease);
    }

    private void OnDisable()
    {
        grab.selectExited.RemoveListener(OnRelease);
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        if (snapPoint != null)
            StartCoroutine(SnapRoutine());
    }

    private IEnumerator SnapRoutine()
    {
        yield return new WaitForEndOfFrame();

        rb.isKinematic = true;
        rb.useGravity = false;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        rb.position = snapPoint.position;
        rb.rotation = snapPoint.rotation;
    }
}