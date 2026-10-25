using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class AssemblyManager : MonoBehaviour
{
    public static AssemblyManager Instance;

    public enum AssemblyPartType { Stator, Rotor, Shaft, Bearing, Fan, BackCover, FrontCover }
    public enum BoltGroup { Back, Front }

    [Header("Assembly Parts (assign the root objects' XRGrabInteractable)")]
    public XRGrabInteractable stator;
    public XRGrabInteractable rotor;
    public XRGrabInteractable shaft;
    public XRGrabInteractable bearing;
    public XRGrabInteractable fan;
    public XRGrabInteractable backCover;
    public XRGrabInteractable frontCover;

    [Header("Optional existing bolt system")]
    public bool useBoltSystem = false;

    private readonly AssemblyPartType[] sequence = {
        AssemblyPartType.Stator, AssemblyPartType.Rotor, AssemblyPartType.Shaft,
        AssemblyPartType.Bearing, AssemblyPartType.Fan, AssemblyPartType.BackCover,
        AssemblyPartType.FrontCover
    };
    private int stepIndex = 0;
    private int backBolts = 0;
    private int frontBolts = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Debug.LogWarning("More than one AssemblyManager exists in the scene.");
        Instance = this;
    }

    private void Start()
    {
        Debug.Log("Assembly started. First part: Stator");
        SetBoltsActive(BoltGroup.Back, false);
        SetBoltsActive(BoltGroup.Front, false);
        SetPartGrabEnabled(stator, true);
        SetPartGrabEnabled(rotor, true);
        SetPartGrabEnabled(shaft, true);
        SetPartGrabEnabled(bearing, true);
        SetPartGrabEnabled(fan, true);
        SetPartGrabEnabled(backCover, true);
        SetPartGrabEnabled(frontCover, true);
    }

    public bool CanAssemble(AssemblyPartType type)
    {
        return stepIndex < sequence.Length && sequence[stepIndex] == type;
    }

    public AssemblyPartType CurrentStep()
    {
        return stepIndex < sequence.Length ? sequence[stepIndex] : sequence[sequence.Length - 1];
    }

    public string CurrentStepName()
    {
        return stepIndex < sequence.Length ? sequence[stepIndex].ToString() : "Complete";
    }

    public void PartAssembled(AssemblyPartType type)
    {
        if (!CanAssemble(type))
        {
            Debug.LogWarning($"Rejected {type}; current step is {CurrentStepName()}.");
            return;
        }

        Debug.Log($"ASSEMBLED: {type}");
        stepIndex++;

        if (stepIndex >= sequence.Length)
        {
            Debug.Log("MOTOR ASSEMBLY COMPLETED!");
            if (useBoltSystem)
                SetBoltsActive(BoltGroup.Front, true);
            return;
        }

        Debug.Log($"NEXT ASSEMBLY STEP: {CurrentStepName()}");
        if (useBoltSystem && type == AssemblyPartType.BackCover)
            SetBoltsActive(BoltGroup.Back, true);
        if (useBoltSystem && type == AssemblyPartType.FrontCover)
            SetBoltsActive(BoltGroup.Front, true);
    }

    private void SetPartGrabEnabled(XRGrabInteractable part, bool enabledState)
    {
        if (part != null) part.enabled = enabledState;
    }

    private void SetBoltsActive(BoltGroup group, bool activeState)
    {
        AssemblyBolt[] bolts = FindObjectsByType<AssemblyBolt>(
            FindObjectsInactive.Include, FindObjectsSortMode.None);

        foreach (AssemblyBolt bolt in bolts)
            if (bolt.boltGroup == group)
                bolt.gameObject.SetActive(activeState);
    }

    // Retained so the existing AssemblyBolt script still compiles.
    public void BoltAssembled(BoltGroup group)
    {
        if (group == BoltGroup.Back)
        {
            backBolts++;
            Debug.Log($"BACK BOLT {backBolts}/4");
            if (backBolts >= 4) SetBoltsActive(BoltGroup.Back, false);
        }
        else
        {
            frontBolts++;
            Debug.Log($"FRONT BOLT {frontBolts}/4");
            if (frontBolts >= 4) SetBoltsActive(BoltGroup.Front, false);
        }
    }
}
