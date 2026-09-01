using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRGrabInteractable))]
[RequireComponent(typeof(Rigidbody))]
public class SnapToPosition : MonoBehaviour
{
    public Transform snapPoint;

    XRGrabInteractable grab;
    Rigidbody rb;

    void Awake()
    {
        grab = GetComponent<XRGrabInteractable>();
        rb = GetComponent<Rigidbody>();
    }

    void OnEnable()
    {
        grab.selectExited.AddListener(OnRelease);
    }

    void OnDisable()
    {
        grab.selectExited.RemoveListener(OnRelease);
    }

    void OnRelease(SelectExitEventArgs args)
    {
        StartCoroutine(SnapRoutine());
    }

    IEnumerator SnapRoutine()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(0.15f);

        rb.isKinematic = true;

        rb.position = snapPoint.position;
        rb.rotation = snapPoint.rotation;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        rb.isKinematic = false;
    }
}