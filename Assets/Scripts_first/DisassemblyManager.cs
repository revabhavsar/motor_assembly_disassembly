using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class DisassemblyManager : MonoBehaviour
{
    public static DisassemblyManager Instance;

    // =========================
    // PART TYPES
    // =========================

    public enum PartType
    {
        FrontCover,
        BackCover,
        RotorPart1,
        RotorPart2,
        Fan,
        Shaft,
        Bearing,
        Stator
    }

    // =========================
    // BOLT GROUP
    // =========================

    public enum BoltGroup
    {
        Front,
        Back
    }

    // =========================
    // MOTOR PARTS
    // =========================

    [Header("Motor Parts")]

    public XRGrabInteractable frontCover;
    public XRGrabInteractable backCover;

    public XRGrabInteractable rotorPart1;
    public XRGrabInteractable rotorPart2;

    public XRGrabInteractable fan;
    public XRGrabInteractable shaft;
    public XRGrabInteractable bearing;
    public XRGrabInteractable stator;

    // =========================
    // BOLT COUNTERS
    // =========================

    private int frontBoltsRemoved = 0;
    private int backBoltsRemoved = 0;

    // =========================
    // CURRENT STEP
    // =========================

    private PartType currentPart = PartType.FrontCover;

    private bool frontCoverUnlocked = false;
    private bool backBoltsUnlocked = false;
    private bool backCoverUnlocked = false;

    // =========================
    // AWAKE
    // =========================

    private void Awake()
    {
        Instance = this;
    }

    // =========================
    // START
    // =========================

    private void Start()
    {
        // Lock every removable part
        LockPart(frontCover);
        LockPart(backCover);

        LockPart(rotorPart1);
        LockPart(rotorPart2);

        LockPart(fan);
        LockPart(shaft);
        LockPart(bearing);
        LockPart(stator);

        // Only FRONT bolts active
        SetBoltGroupActive(BoltGroup.Front, true);
        SetBoltGroupActive(BoltGroup.Back, false);

        Debug.Log("STEP 1: Remove all 4 Front Bolts");
    }

    // =========================================================
    // BOLT CHECK
    // =========================================================

    public bool CanUseBolt(BoltGroup group)
    {
        if (group == BoltGroup.Front)
            return !frontCoverUnlocked;

        if (group == BoltGroup.Back)
            return backBoltsUnlocked;

        return false;
    }

    // =========================================================
    // BOLT REMOVED
    // =========================================================

    public void BoltRemoved(BoltGroup group)
    {
        if (!CanUseBolt(group))
            return;

        // -------------------------
        // FRONT BOLTS
        // -------------------------

        if (group == BoltGroup.Front)
        {
            frontBoltsRemoved++;

            Debug.Log(
                "Front Bolt: " +
                frontBoltsRemoved +
                " / 4"
            );

            if (frontBoltsRemoved >= 4)
            {
                frontCoverUnlocked = true;

                UnlockPart(frontCover);

                Debug.Log("STEP 2: Front Cover Unlocked");
            }
        }

        // -------------------------
        // BACK BOLTS
        // -------------------------

        else if (group == BoltGroup.Back)
        {
            backBoltsRemoved++;

            Debug.Log(
                "Back Bolt: " +
                backBoltsRemoved +
                " / 4"
            );

            if (backBoltsRemoved >= 4)
            {
                backCoverUnlocked = true;

                UnlockPart(backCover);

                Debug.Log("STEP 4: Back Cover Unlocked");
            }
        }
    }

    // =========================================================
    // PART CHECK
    // =========================================================

    public bool CanRemovePart(PartType part)
    {
        return part == currentPart;
    }

    // =========================================================
    // PART REMOVED
    // =========================================================

    public void PartRemoved(PartType part)
    {
        // Wrong part
        if (!CanRemovePart(part))
        {
            Debug.Log(
                "LOCKED: " +
                part +
                " cannot be removed yet!"
            );

            return;
        }

        switch (part)
        {
            // =========================
            // FRONT COVER
            // =========================

            case PartType.FrontCover:

                Debug.Log("Front Cover Removed");

                currentPart = PartType.BackCover;

                backBoltsUnlocked = true;

                SetBoltGroupActive(
                    BoltGroup.Back,
                    true
                );

                Debug.Log(
                    "STEP 3: Remove all 4 Back Bolts"
                );

                break;


            // =========================
            // BACK COVER
            // =========================

            case PartType.BackCover:

                Debug.Log("Back Cover Removed");

                currentPart = PartType.RotorPart1;

                UnlockPart(rotorPart1);

                Debug.Log(
                    "STEP 5: Rotor Part 1 Unlocked"
                );

                break;


            // =========================
            // ROTOR PART 1
            // =========================

            case PartType.RotorPart1:

                Debug.Log("Rotor Part 1 Removed");

                currentPart = PartType.RotorPart2;

                UnlockPart(rotorPart2);

                Debug.Log(
                    "STEP 6: Rotor Part 2 Unlocked"
                );

                break;


            // =========================
            // ROTOR PART 2
            // =========================

            case PartType.RotorPart2:

                Debug.Log("Rotor Part 2 Removed");

                currentPart = PartType.Fan;

                UnlockPart(fan);

                Debug.Log(
                    "STEP 7: Fan Unlocked"
                );

                break;


            // =========================
            // FAN
            // =========================

            case PartType.Fan:

                Debug.Log("Fan Removed");

                currentPart = PartType.Shaft;

                UnlockPart(shaft);

                Debug.Log(
                    "STEP 8: Shaft Unlocked"
                );

                break;


            // =========================
            // SHAFT
            // =========================

            case PartType.Shaft:

                Debug.Log("Shaft Removed");

                currentPart = PartType.Bearing;

                UnlockPart(bearing);

                Debug.Log(
                    "STEP 9: Bearing Unlocked"
                );

                break;


            // =========================
            // BEARING
            // =========================

            case PartType.Bearing:

                Debug.Log("Bearing Removed");

                currentPart = PartType.Stator;

                UnlockPart(stator);

                Debug.Log(
                    "STEP 10: Stator Unlocked"
                );

                break;


            // =========================
            // STATOR
            // =========================

            case PartType.Stator:

                Debug.Log(
                    "MOTOR COMPLETELY DISASSEMBLED!"
                );

                break;
        }
    }

    // =========================================================
    // UNLOCK PART
    // =========================================================

    private void UnlockPart(XRGrabInteractable part)
    {
        if (part == null)
            return;

        Rigidbody rb = part.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;

            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        part.enabled = true;
    }

    // =========================================================
    // LOCK PART
    // =========================================================

    private void LockPart(XRGrabInteractable part)
    {
        if (part == null)
            return;

        part.enabled = false;

        Rigidbody rb = part.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;

            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    // =========================================================
    // BOLT GROUP
    // =========================================================

    private void SetBoltGroupActive(
        BoltGroup group,
        bool active
    )
    {
        BoltUnscrew[] bolts =
            FindObjectsByType<BoltUnscrew>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None
            );

        foreach (BoltUnscrew bolt in bolts)
        {
            if (bolt.boltGroup == group)
            {
                bolt.gameObject.SetActive(active);
            }
        }
    }
}