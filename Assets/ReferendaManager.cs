using UnityEngine;
using Assets.Scripts; // Add this line to include the namespace where PolkadotManager is defined

public class ReferendaManager : MonoBehaviour
{
    private PolkadotManager polkadotManager;

    private void Start()
    {
        polkadotManager = FindObjectOfType<PolkadotManager>();

        if (polkadotManager != null)
        {
            Debug.Log("PolkadotManager found.");
        }
        else
        {
            Debug.LogError("PolkadotManager not found.");
        }
    }

    // Your other code here...
}
