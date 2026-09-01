using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class DisassemblyManager : MonoBehaviour
{
    public static DisassemblyManager Instance;

    //===========================
    // ENUM
    //===========================

    public enum PartType
    {
        FrontCover,
        BackCover,
        Rotor,
        Fan,
        Shaft,
        Bearing,
        Stator
    }

    //===========================
    // REFERENCES
    //===========================

    [Header("Motor Parts")]

    public XRGrabInteractable frontCover;
    public XRGrabInteractable backCover;

    public XRGrabInteractable rotor;

    public XRGrabInteractable fan;

    public XRGrabInteractable shaft;

    public XRGrabInteractable bearing;

    public XRGrabInteractable stator;

    //===========================
    // FRONT BOLTS
    //===========================

    private int frontBoltCount = 0;

    //===========================
    // REAR BOLTS
    //===========================

    private int rearBoltCount = 0;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        // Initially disable everything

        frontCover.enabled = false;
        backCover.enabled = false;

        rotor.enabled = false;
        fan.enabled = false;
        shaft.enabled = false;
        bearing.enabled = false;
        stator.enabled = false;
    }

    //==================================================
    // FRONT BOLTS
    //==================================================

    public void FrontBoltRemoved()
    {
        frontBoltCount++;

        Debug.Log("Front Bolt : " + frontBoltCount);

        if (frontBoltCount >= 4)
        {
            Debug.Log("Front Cover Unlocked");

            frontCover.enabled = true;
        }
    }

    //==================================================
    // REAR BOLTS
    //==================================================

    public void RearBoltRemoved()
    {
        rearBoltCount++;

        Debug.Log("Rear Bolt : " + rearBoltCount);

        if (rearBoltCount >= 4)
        {
            Debug.Log("Back Cover Unlocked");

            backCover.enabled = true;
        }
    }

    //==================================================
    // PART REMOVED
    //==================================================

    public void PartRemoved(PartType part)
    {
        switch (part)
        {
            case PartType.FrontCover:

                Debug.Log("Front Cover Removed");

                // Enable rear bolts here if required
                break;

            case PartType.BackCover:

                Debug.Log("Back Cover Removed");

                rotor.enabled = true;

                break;

            case PartType.Rotor:

                Debug.Log("Rotor Removed");

                fan.enabled = true;

                break;

            case PartType.Fan:

                Debug.Log("Fan Removed");

                shaft.enabled = true;

                break;

            case PartType.Shaft:

                Debug.Log("Shaft Removed");

                bearing.enabled = true;

                break;

            case PartType.Bearing:

                Debug.Log("Bearing Removed");

                stator.enabled = true;

                break;

            case PartType.Stator:

                Debug.Log("Motor Completely Disassembled!");

                break;
        }
    }
}