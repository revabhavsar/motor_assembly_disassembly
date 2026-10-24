using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRGrabInteractable))]
[RequireComponent(typeof(Rigidbody))]
public class AssemblyBolt : MonoBehaviour
{
    [Header("Bolt Group")]
    public AssemblyManager.BoltGroup boltGroup;

    [Header("Bolt Socket")]
    public Transform socketPoint;

    [Header("Screw Settings")]
    public float screwDistance = 0.03f;
    public float screwTime = 2f;
    public int rotations = 8;

    private XRGrabInteractable grab;
    private Rigidbody rb;

    private bool placed = false;
    private bool wrenchInserted = false;
    private bool tightening = false;

    private Vector3 startPosition;
    private Quaternion startRotation;

    private void Awake()
    {
        grab =
            GetComponent<XRGrabInteractable>();

        rb =
            GetComponent<Rigidbody>();

        rb.isKinematic = true;
        rb.useGravity = false;
    }

    private void OnEnable()
    {
        if (grab != null)
            grab.selectExited.AddListener(OnReleased);
    }

    private void OnDisable()
    {
        if (grab != null)
            grab.selectExited.RemoveListener(OnReleased);
    }

    // =====================================================
    // RELEASE BOLT
    // =====================================================

    private void OnReleased(
        SelectExitEventArgs args)
    {
        if (placed)
            return;

        if (args.isCanceled)
            return;

        if (socketPoint == null)
        {
            Debug.LogError(
                "Socket missing for " +
                gameObject.name
            );

            return;
        }

        // EXACT SOCKET POSITION

        transform.SetPositionAndRotation(
            socketPoint.position,
            socketPoint.rotation
        );

        rb.isKinematic = true;
        rb.useGravity = false;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        placed = true;

        startPosition =
            transform.localPosition;

        startRotation =
            transform.localRotation;

        grab.enabled = false;

        Debug.Log(
            gameObject.name +
            " placed."
        );
    }

    // =====================================================
    // WRENCH
    // =====================================================

    public void WrenchInserted(
        bool value)
    {
        if (!placed)
            return;

        wrenchInserted = value;

        if (value)
        {
            Debug.Log(
                "Wrench inserted: " +
                gameObject.name
            );
        }
    }

    // =====================================================
    // UPDATE
    // =====================================================

    private void Update()
    {
        if (!placed)
            return;

        if (!wrenchInserted)
            return;

        if (tightening)
            return;

        if (Input.GetKeyDown(KeyCode.Z))
        {
            StartCoroutine(
                TightenBolt()
            );
        }
    }

    // =====================================================
    // TIGHTEN
    // =====================================================

    private IEnumerator TightenBolt()
    {
        tightening = true;

        float time = 0f;

        while (time < screwTime)
        {
            time += Time.deltaTime;

            float t =
                Mathf.Clamp01(
                    time / screwTime
                );

            transform.localRotation =
                startRotation *
                Quaternion.Euler(
                    0f,
                    360f * rotations * t,
                    0f
                );

            transform.localPosition =
                startPosition -
                Vector3.up *
                (screwDistance * t);

            yield return null;
        }

        transform.localRotation =
            startRotation *
            Quaternion.Euler(
                0f,
                360f * rotations,
                0f
            );

        transform.localPosition =
            startPosition -
            Vector3.up *
            screwDistance;

        Debug.Log(
            gameObject.name +
            " TIGHTENED"
        );

        if (AssemblyManager.Instance != null)
        {
            AssemblyManager.Instance.BoltAssembled(
                boltGroup
            );
        }

        tightening = false;
    }
}