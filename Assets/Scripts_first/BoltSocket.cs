using UnityEngine;

public class BoltSocket : MonoBehaviour
{
    public BoltUnscrew bolt;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("WrenchTip"))
            return;

        if (bolt == null)
            return;

        bolt.WrenchInserted(true);

        Debug.Log(
            "Wrench inserted into " +
            gameObject.name
        );
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("WrenchTip"))
            return;

        if (bolt == null)
            return;

        bolt.WrenchInserted(false);

        Debug.Log(
            "Wrench removed from " +
            gameObject.name
        );
    }
}