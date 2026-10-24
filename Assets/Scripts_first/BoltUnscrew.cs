using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class BoltUnscrew : MonoBehaviour
{
    [Header("Bolt Information")]
    public DisassemblyManager.BoltGroup boltGroup;

    [Header("References")]
    public Transform boltMesh;
    public XRGrabInteractable grabInteractable;

    [Header("Animation")]
    public float moveDistance = 0.03f;
    public float duration = 2f;
    public int rotations = 8;

    private bool wrenchInserted = false;
    private bool isUnscrewing = false;
    private bool counted = false;

    private Vector3 startPosition;
    private Quaternion startRotation;

    private void Start()
    {
        if (boltMesh == null)
        {
            Debug.LogError(
                "Bolt Mesh not assigned: " +
                gameObject.name
            );

            return;
        }

        startPosition = boltMesh.localPosition;
        startRotation = boltMesh.localRotation;

        if (grabInteractable != null)
            grabInteractable.enabled = false;
    }

    private void Update()
    {
        if (!wrenchInserted)
            return;

        if (isUnscrewing)
            return;

        if (counted)
            return;

        // Check with manager
        if (DisassemblyManager.Instance == null)
            return;

        if (!DisassemblyManager.Instance.CanUseBolt(boltGroup))
            return;

        if (Input.GetKeyDown(KeyCode.Z))
        {
            StartCoroutine(UnscrewBolt());
        }
    }

    public void WrenchInserted(bool value)
    {
        wrenchInserted = value;

        if (value)
            Debug.Log(
                gameObject.name +
                " - Wrench Inserted"
            );
    }

    private IEnumerator UnscrewBolt()
    {
        isUnscrewing = true;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.Clamp01(
                elapsed / duration
            );

            // Rotate
            boltMesh.localRotation =
                startRotation *
                Quaternion.Euler(
                    0f,
                    -360f * rotations * t,
                    0f
                );

            // Move upward
            boltMesh.localPosition =
                startPosition +
                Vector3.up *
                (moveDistance * t);

            yield return null;
        }

        // Final position
        boltMesh.localRotation =
            startRotation *
            Quaternion.Euler(
                0f,
                -360f * rotations,
                0f
            );

        boltMesh.localPosition =
            startPosition +
            Vector3.up *
            moveDistance;

        counted = true;

        if (grabInteractable != null)
            grabInteractable.enabled = true;

        Debug.Log(
            gameObject.name +
            " - UNSCREWED"
        );

        if (DisassemblyManager.Instance != null)
        {
            DisassemblyManager.Instance.BoltRemoved(
                boltGroup
            );
        }

        isUnscrewing = false;
    }
}