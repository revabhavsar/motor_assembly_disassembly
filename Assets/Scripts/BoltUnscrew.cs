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
    public float duration = 4f;
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
            Debug.LogError("Bolt Mesh not assigned on " + gameObject.name);
            return;
        }

        startPosition = boltMesh.localPosition;
        startRotation = boltMesh.localRotation;

        if (grabInteractable != null)
            grabInteractable.enabled = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (wrenchInserted && !isUnscrewing)
            {
                StartCoroutine(UnscrewBolt());
            }
        }
    }

    public void WrenchInserted(bool value)
    {
        wrenchInserted = value;

        if (value)
            Debug.Log(gameObject.name + " : Wrench Inserted");
        else
            Debug.Log(gameObject.name + " : Wrench Removed");
    }

    IEnumerator UnscrewBolt()
    {
        isUnscrewing = true;

        // Restart animation every time
        boltMesh.localPosition = startPosition;
        boltMesh.localRotation = startRotation;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float t = elapsed / duration;

            boltMesh.localRotation =
                startRotation *
                Quaternion.Euler(0f, -360f * rotations * t, 0f);

            boltMesh.localPosition =
                startPosition +
                Vector3.up * (moveDistance * t);

            yield return null;
        }

        boltMesh.localRotation =
            startRotation *
            Quaternion.Euler(0f, -360f * rotations, 0f);

        boltMesh.localPosition =
            startPosition +
            Vector3.up * moveDistance;

        if (grabInteractable != null)
            grabInteractable.enabled = true;

        Debug.Log(gameObject.name + " Ready To Grab");

        // Count this bolt only once
        if (!counted)
        {
            counted = true;

            if (DisassemblyManager.Instance != null)
            {
                DisassemblyManager.Instance.BoltRemoved(boltGroup);
            }
        }

        isUnscrewing = false;
    }
}