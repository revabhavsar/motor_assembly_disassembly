using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRGrabInteractable))]
[RequireComponent(typeof(Rigidbody))]
public class AssemblyPart : MonoBehaviour
{
    [Header("Assembly")]
    public AssemblyManager.AssemblyPartType partType;

    [Header("Snap Point")]
    public Transform snapPoint;

    private XRGrabInteractable grab;
    private Rigidbody rb;

    private bool assembled = false;
    private bool snapping = false;

    private void Awake()
    {
        grab = GetComponent<XRGrabInteractable>();
        rb = GetComponent<Rigidbody>();

        rb.isKinematic = true;
        rb.useGravity = false;
    }

    private void OnEnable()
    {
        if (grab != null)
        {
            grab.selectExited.AddListener(OnReleased);
        }
    }

    private void OnDisable()
    {
        if (grab != null)
        {
            grab.selectExited.RemoveListener(OnReleased);
        }
    }

    private void OnReleased(SelectExitEventArgs args)
    {
        // Already assembled or currently snapping
        if (assembled || snapping)
        {
            return;
        }

        // Ignore cancelled release
        if (args.isCanceled)
        {
            return;
        }

        // -------------------------------------------------
        // CHECK ASSEMBLY MANAGER
        // -------------------------------------------------

        if (AssemblyManager.Instance == null)
        {
            Debug.LogError(
                gameObject.name +
                " : AssemblyManager.Instance is NULL!"
            );

            return;
        }

        // -------------------------------------------------
        // CHECK ASSEMBLY ORDER
        // -------------------------------------------------

        if (!AssemblyManager.Instance.CanAssemble(partType))
        {
            Debug.Log(
                gameObject.name +
                " is NOT the current assembly part."
            );

            return;
        }

        // -------------------------------------------------
        // CHECK SNAP POINT
        // -------------------------------------------------

        if (snapPoint == null)
        {
            Debug.LogError(
                gameObject.name +
                " : SNAP POINT IS MISSING!"
            );

            return;
        }

        Debug.Log(
            "SNAP STARTED -> " +
            gameObject.name +
            " -> " +
            snapPoint.name
        );

        // Wait until XR Grab has completely released the object
        StartCoroutine(SnapAfterRelease());
    }

    private IEnumerator SnapAfterRelease()
    {
        snapping = true;

        // -------------------------------------------------
        // WAIT FOR XR INTERACTION TOOLKIT
        // -------------------------------------------------

        // Give XR Interaction Toolkit one frame to finish
        // its grab/release operation.
        yield return null;

        // Wait until the current frame has completely finished.
        yield return new WaitForEndOfFrame();

        // -------------------------------------------------
        // STOP PHYSICS
        // -------------------------------------------------

        rb.isKinematic = true;
        rb.useGravity = false;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // -------------------------------------------------
        // SNAP POSITION AND ROTATION
        // -------------------------------------------------

        transform.position = snapPoint.position;
        transform.rotation = snapPoint.rotation;

        // -------------------------------------------------
        // SET PARENT
        // -------------------------------------------------

        // If the snap point has a parent, put the assembled
        // part under the same parent.
        //
        // worldPositionStays = true means the current world
        // position is preserved while changing parent.

        if (snapPoint.parent != null)
        {
            transform.SetParent(
                snapPoint.parent,
                true
            );
        }

        // -------------------------------------------------
        // APPLY EXACT SNAP AGAIN
        // -------------------------------------------------

        transform.SetPositionAndRotation(
            snapPoint.position,
            snapPoint.rotation
        );

        // -------------------------------------------------
        // ASSEMBLY COMPLETE
        // -------------------------------------------------

        assembled = true;
        snapping = false;

        // Prevent the user from grabbing this part again.
        grab.enabled = false;

        // -------------------------------------------------
        // DEBUG INFORMATION
        // -------------------------------------------------

        Debug.Log(
            "================================="
        );

        Debug.Log(
            "ASSEMBLED: " +
            gameObject.name
        );

        Debug.Log(
            "SNAP POINT: " +
            snapPoint.name
        );

        Debug.Log(
            "POSITION: " +
            snapPoint.position
        );

        Debug.Log(
            "ROTATION: " +
            snapPoint.rotation
        );

        Debug.Log(
            "================================="
        );

        // -------------------------------------------------
        // TELL ASSEMBLY MANAGER
        // -------------------------------------------------

        AssemblyManager.Instance.PartAssembled(
            partType
        );
    }
}