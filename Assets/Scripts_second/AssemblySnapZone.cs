using UnityEngine;

[RequireComponent(typeof(Collider))]
public class AssemblySnapZone : MonoBehaviour
{
    [Header("This destination accepts only this part")]
    public AssemblyManager.AssemblyPartType expectedPart;

    [Header("Exact final pose; assign a child empty GameObject")]
    public Transform snapPoint;

    private Collider triggerCollider;

    private void Awake()
    {
        triggerCollider = GetComponent<Collider>();
        triggerCollider.isTrigger = true;

        if (snapPoint == null)
            Debug.LogError($"{name}: Assign a Snap Point Transform in the Inspector.");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (snapPoint == null || AssemblyManager.Instance == null) return;

        AssemblyPart part = other.GetComponentInParent<AssemblyPart>();
        if (part == null || part.IsAssembled()) return;

        if (part.partType != expectedPart)
        {
            Debug.Log($"Wrong destination: {part.partType} entered {name}; this zone accepts {expectedPart}.");
            return;
        }

        if (!AssemblyManager.Instance.CanAssemble(expectedPart))
        {
            Debug.Log($"Sequence blocked: {expectedPart} is not the current step.");
            return;
        }

        part.TryPlace(snapPoint);
    }
}
