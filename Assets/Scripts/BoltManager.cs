using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class BoltManager : MonoBehaviour
{
    public static BoltManager Instance;

    [Header("Front Cover")]
    public XRGrabInteractable frontCoverGrab;

    private int boltsRemoved = 0;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (frontCoverGrab != null)
            frontCoverGrab.enabled = false;
    }

    public void BoltRemoved()
    {
        boltsRemoved++;

        Debug.Log("Bolts Removed : " + boltsRemoved);

        if (boltsRemoved >= 4)
        {
            Debug.Log("All bolts removed!");

            if (frontCoverGrab != null)
                frontCoverGrab.enabled = true;
        }
    }
}