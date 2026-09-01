using UnityEngine;

public class BoltSocket : MonoBehaviour
{
    public BoltUnscrew bolt;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Entered : " + other.name);

        if (other.CompareTag("WrenchTip"))
        {
            Debug.Log("Wrench Inserted");
            bolt.WrenchInserted(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Exited : " + other.name);

        if (other.CompareTag("WrenchTip"))
        {
            Debug.Log("Wrench Removed");
            bolt.WrenchInserted(false);
        }
    }
}