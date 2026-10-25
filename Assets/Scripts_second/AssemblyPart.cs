using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRGrabInteractable))]
[RequireComponent(typeof(Rigidbody))]
public class AssemblyPart : MonoBehaviour
{
    [Header("Assembly")]
    public AssemblyManager.AssemblyPartType partType;

    [Header("Colour feedback")]
    public Color correctColour = Color.green;
    public Color wrongColour = Color.red;

    private XRGrabInteractable grab;
    private Rigidbody rb;
    private Renderer[] renderers;
    private Color[][] originalColours;
    private bool assembled;
    private bool wrongSelection;

    private void Awake()
    {
        grab = GetComponent<XRGrabInteractable>();
        rb = GetComponent<Rigidbody>();
        renderers = GetComponentsInChildren<Renderer>(true);
        originalColours = new Color[renderers.Length][];

        for (int i = 0; i < renderers.Length; i++)
        {
            Material[] mats = renderers[i].materials;
            originalColours[i] = new Color[mats.Length];
            for (int j = 0; j < mats.Length; j++)
                originalColours[i][j] = mats[j].color;
        }

        rb.isKinematic = true;
        rb.useGravity = false;
    }

    private void OnEnable()
    {
        if (grab != null)
        {
            grab.selectEntered.AddListener(OnGrabbed);
            grab.selectExited.AddListener(OnReleased);
        }
    }

    private void OnDisable()
    {
        if (grab != null)
        {
            grab.selectEntered.RemoveListener(OnGrabbed);
            grab.selectExited.RemoveListener(OnReleased);
        }
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        if (assembled || AssemblyManager.Instance == null) return;

        bool correct = AssemblyManager.Instance.CanAssemble(partType);
        wrongSelection = !correct;
        SetColour(correct ? correctColour : wrongColour);

        if (!correct)
            Debug.LogWarning($"WRONG PART: {partType}. Required: {AssemblyManager.Instance.CurrentStepName()}");
        else
            Debug.Log($"Correct part selected: {partType}. Move it into its matching placement trigger.");
    }

    private void OnReleased(SelectExitEventArgs args)
    {
        if (assembled || args.isCanceled) return;

        // Colour feedback remains while the user is holding the part.
        // On release, restore the source material; only the matching trigger can place it.
        RestoreColour();
    }

    public bool IsAssembled() => assembled;

    public bool TryPlace(Transform snapPoint)
    {
        if (assembled || snapPoint == null || AssemblyManager.Instance == null)
            return false;

        if (!AssemblyManager.Instance.CanAssemble(partType))
        {
            SetColour(wrongColour);
            Debug.LogWarning($"Cannot place {partType}. Required: {AssemblyManager.Instance.CurrentStepName()}");
            return false;
        }

        transform.SetPositionAndRotation(snapPoint.position, snapPoint.rotation);
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true;
        rb.useGravity = false;

        assembled = true;
        RestoreColour();

        // Disable grabbing after successful placement.
        if (grab != null) grab.enabled = false;

        // Disable the part's solid colliders so it won't block later parts.
        Collider[] colliders = GetComponentsInChildren<Collider>(true);
        foreach (Collider col in colliders) col.enabled = false;

        AssemblyManager.Instance.PartAssembled(partType);
        return true;
    }

    private void SetColour(Color colour)
    {
        foreach (Renderer rend in renderers)
        {
            if (rend == null) continue;
            foreach (Material mat in rend.materials)
            {
                if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", colour);
                if (mat.HasProperty("_Color")) mat.SetColor("_Color", colour);
            }
        }
    }

    private void RestoreColour()
    {
        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i] == null) continue;
            Material[] mats = renderers[i].materials;
            for (int j = 0; j < mats.Length && j < originalColours[i].Length; j++)
            {
                if (mats[j].HasProperty("_BaseColor")) mats[j].SetColor("_BaseColor", originalColours[i][j]);
                if (mats[j].HasProperty("_Color")) mats[j].SetColor("_Color", originalColours[i][j]);
            }
        }
    }
}
