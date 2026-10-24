using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class AssemblyManager : MonoBehaviour
{
    public static AssemblyManager Instance;

    // =====================================================
    // PART TYPES
    // =====================================================

    public enum AssemblyPartType
    {
        Stator,
        Bearing,
        Shaft,
        Fan,
        RotorPart2,
        RotorPart1,
        BackCover,
        FrontCover
    }

    // =====================================================
    // BOLT GROUPS
    // =====================================================

    public enum BoltGroup
    {
        Back,
        Front
    }

    // =====================================================
    // PART REFERENCES
    // =====================================================

    [Header("Assembly Parts")]

    public XRGrabInteractable stator;
    public XRGrabInteractable bearing;
    public XRGrabInteractable shaft;
    public XRGrabInteractable fan;

    public XRGrabInteractable rotorPart2;
    public XRGrabInteractable rotorPart1;

    public XRGrabInteractable backCover;
    public XRGrabInteractable frontCover;

    // =====================================================
    // BOLT COUNTERS
    // =====================================================

    private int backBolts = 0;
    private int frontBolts = 0;

    // =====================================================
    // CURRENT STEP
    // =====================================================

    private AssemblyPartType currentPart =
        AssemblyPartType.Stator;

    // =====================================================
    // AWAKE
    // =====================================================

    private void Awake()
    {
        Instance = this;
    }

    // =====================================================
    // START
    // =====================================================

    private void Start()
    {
        /*
         * IMPORTANT:
         * Keep ALL parts grab-able.
         *
         * The AssemblyPart script decides
         * which part is allowed to snap.
         */

        EnablePart(stator);
        EnablePart(bearing);
        EnablePart(shaft);
        EnablePart(fan);

        EnablePart(rotorPart2);
        EnablePart(rotorPart1);

        EnablePart(backCover);
        EnablePart(frontCover);

        // Disable bolts initially

        SetBoltsActive(
            BoltGroup.Back,
            false
        );

        SetBoltsActive(
            BoltGroup.Front,
            false
        );

        currentPart =
            AssemblyPartType.Stator;

        Debug.Log(
            "ASSEMBLY STARTED"
        );

        Debug.Log(
            "CURRENT PART: STATOR"
        );
    }

    // =====================================================
    // CHECK CURRENT PART
    // =====================================================

    public bool CanAssemble(
        AssemblyPartType part)
    {
        return part == currentPart;
    }

    // =====================================================
    // PART ASSEMBLED
    // =====================================================

    public void PartAssembled(
        AssemblyPartType part)
    {
        if (part != currentPart)
        {
            Debug.Log(
                "WRONG PART: " +
                part +
                " | CURRENT PART: " +
                currentPart
            );

            return;
        }

        switch (part)
        {
            // =============================================
            // 1. STATOR
            // =============================================

            case AssemblyPartType.Stator:

                Debug.Log(
                    "STEP 1 COMPLETE: STATOR"
                );

                currentPart =
                    AssemblyPartType.Bearing;

                Debug.Log(
                    "STEP 2: BEARING"
                );

                break;


            // =============================================
            // 2. BEARING
            // =============================================

            case AssemblyPartType.Bearing:

                Debug.Log(
                    "STEP 2 COMPLETE: BEARING"
                );

                currentPart =
                    AssemblyPartType.Shaft;

                Debug.Log(
                    "STEP 3: SHAFT"
                );

                break;


            // =============================================
            // 3. SHAFT
            // =============================================

            case AssemblyPartType.Shaft:

                Debug.Log(
                    "STEP 3 COMPLETE: SHAFT"
                );

                currentPart =
                    AssemblyPartType.Fan;

                Debug.Log(
                    "STEP 4: FAN"
                );

                break;


            // =============================================
            // 4. FAN
            // =============================================

            case AssemblyPartType.Fan:

                Debug.Log(
                    "STEP 4 COMPLETE: FAN"
                );

                currentPart =
                    AssemblyPartType.RotorPart2;

                Debug.Log(
                    "STEP 5: ROTOR PART 2"
                );

                break;


            // =============================================
            // 5. ROTOR PART 2
            // =============================================

            case AssemblyPartType.RotorPart2:

                Debug.Log(
                    "STEP 5 COMPLETE: ROTOR PART 2"
                );

                currentPart =
                    AssemblyPartType.RotorPart1;

                Debug.Log(
                    "STEP 6: ROTOR PART 1"
                );

                break;


            // =============================================
            // 6. ROTOR PART 1
            // =============================================

            case AssemblyPartType.RotorPart1:

                Debug.Log(
                    "STEP 6 COMPLETE: ROTOR PART 1"
                );

                currentPart =
                    AssemblyPartType.BackCover;

                Debug.Log(
                    "STEP 7: BACK COVER"
                );

                break;


            // =============================================
            // 7. BACK COVER
            // =============================================

            case AssemblyPartType.BackCover:

                Debug.Log(
                    "STEP 7 COMPLETE: BACK COVER"
                );

                SetBoltsActive(
                    BoltGroup.Back,
                    true
                );

                Debug.Log(
                    "STEP 8: BACK BOLTS"
                );

                break;


            // =============================================
            // FRONT COVER
            // =============================================

            case AssemblyPartType.FrontCover:

                Debug.Log(
                    "FRONT COVER COMPLETE"
                );

                SetBoltsActive(
                    BoltGroup.Front,
                    true
                );

                Debug.Log(
                    "STEP 10: FRONT BOLTS"
                );

                break;
        }
    }

    // =====================================================
    // BACK BOLTS COMPLETE
    // =====================================================

    private void BackBoltsCompleted()
    {
        Debug.Log(
            "ALL 4 BACK BOLTS COMPLETE"
        );

        SetBoltsActive(
            BoltGroup.Back,
            false
        );

        currentPart =
            AssemblyPartType.FrontCover;

        Debug.Log(
            "STEP 9: FRONT COVER"
        );
    }

    // =====================================================
    // BOLT ASSEMBLED
    // =====================================================

    public void BoltAssembled(
        BoltGroup group)
    {
        if (group == BoltGroup.Back)
        {
            backBolts++;

            Debug.Log(
                "BACK BOLT " +
                backBolts +
                " / 4"
            );

            if (backBolts >= 4)
            {
                BackBoltsCompleted();
            }
        }

        else if (group == BoltGroup.Front)
        {
            frontBolts++;

            Debug.Log(
                "FRONT BOLT " +
                frontBolts +
                " / 4"
            );

            if (frontBolts >= 4)
            {
                FrontBoltsCompleted();
            }
        }
    }

    // =====================================================
    // FRONT BOLTS COMPLETE
    // =====================================================

    private void FrontBoltsCompleted()
    {
        SetBoltsActive(
            BoltGroup.Front,
            false
        );

        Debug.Log(
            "================================"
        );

        Debug.Log(
            "MOTOR ASSEMBLY COMPLETED!"
        );

        Debug.Log(
            "================================"
        );
    }

    // =====================================================
    // ENABLE PART
    // =====================================================

    private void EnablePart(
        XRGrabInteractable part)
    {
        if (part == null)
            return;

        Rigidbody rb =
            part.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;

            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        // KEEP XR GRAB ENABLED

        part.enabled = true;
    }

    // =====================================================
    // BOLTS
    // =====================================================

    private void SetBoltsActive(
        BoltGroup group,
        bool active)
    {
        AssemblyBolt[] bolts =
            FindObjectsByType<AssemblyBolt>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None
            );

        foreach (AssemblyBolt bolt in bolts)
        {
            if (bolt.boltGroup == group)
            {
                bolt.gameObject.SetActive(active);
            }
        }
    }
}