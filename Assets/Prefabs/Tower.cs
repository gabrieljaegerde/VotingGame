using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Tower : MonoBehaviour
{
    public int ReferendumId;
    private PolkadotManager polkadotManager;

    // Reference to the Info Window UI elements
    public GameObject infoWindow;
    public TextMeshProUGUI referendumIdText;
    public TextMeshProUGUI trackText;
    public TextMeshProUGUI statusText;
    public Button closeButton; // Reference to the Close button

    private CanvasGroup canvasGroup;

    void Start()
    {
        polkadotManager = GameObject.Find("PolkadotManager").GetComponent<PolkadotManager>();

        // Check if polkadotManager is assigned
        if (polkadotManager == null)
        {
            Debug.LogError("PolkadotManager is not found in the scene.");
        }

        // Get the CanvasGroup component
        canvasGroup = infoWindow.GetComponent<CanvasGroup>();

        // Check if canvasGroup is assigned
        if (canvasGroup == null)
        {
            Debug.LogError("CanvasGroup component is not found on the InfoWindow.");
        }

        // Hide the Info Window initially
        HideInfoWindow();

        // Check if closeButton is assigned
        if (closeButton == null)
        {
            Debug.LogError("CloseButton is not assigned.");
        }
        else
        {
            // Assign the Close button's onClick listener
            closeButton.onClick.AddListener(CloseInfoWindow);
            Debug.Log("CloseButton listener assigned.");
        }
    }

    void OnMouseDown()
    {
        OpenInfoWindow();
    }

    public void OpenInfoWindow()
    {
        // Check if referendumIdText is assigned
        if (referendumIdText == null)
        {
            Debug.LogError("ReferendumIdText is not assigned.");
            return;
        }

        // Check if trackText is assigned
        if (trackText == null)
        {
            Debug.LogError("TrackText is not assigned.");
            return;
        }

        // Check if statusText is assigned
        if (statusText == null)
        {
            Debug.LogError("StatusText is not assigned.");
            return;
        }

        // Set the text fields with referendum information
        referendumIdText.text = "Referendum ID: " + ReferendumId;
        var referendumInfo = polkadotManager.GetReferendumInfo(ReferendumId);
        trackText.text = "Track: " + referendumInfo.ReferendumStatus.Track;
        statusText.text = "Status: " + referendumInfo.ReferendumInfo.ToString();

        // Show the Info Window
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    public void CloseInfoWindow()
    {
        // Hide the Info Window
        HideInfoWindow();
    }

    private void HideInfoWindow()
    {
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }
}
