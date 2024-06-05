using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json.Linq;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public Text infoText;
    public Text referendumTitle;
    public Text referendumContent;
    public Text referendumStatus;
    public Text referendumEnd;
    public Text referendumComments;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        infoText = GameObject.Find("InfoText")?.GetComponent<Text>();
        referendumTitle = GameObject.Find("ReferendumTitle")?.GetComponent<Text>();
        referendumContent = GameObject.Find("ReferendumContent")?.GetComponent<Text>();
        referendumStatus = GameObject.Find("ReferendumStatus")?.GetComponent<Text>();
        referendumEnd = GameObject.Find("ReferendumEnd")?.GetComponent<Text>();
        referendumComments = GameObject.Find("CommentsText")?.GetComponent<Text>();

        if (infoText == null) Debug.LogError("infoText is not assigned in UIManager Awake.");
        if (referendumTitle == null) Debug.LogError("referendumTitle is not assigned in UIManager Awake.");
        if (referendumContent == null) Debug.LogError("referendumContent is not assigned in UIManager Awake.");
        if (referendumStatus == null) Debug.LogError("referendumStatus is not assigned in UIManager Awake.");
        if (referendumEnd == null) Debug.LogError("referendumEnd is not assigned in UIManager Awake.");
        if (referendumComments == null) Debug.LogError("referendumComments is not assigned in UIManager Awake.");
    }

    // public void DisplayReferendumInfo(Tower tower, string referendumDetails, string polkassemblyData)
    // {
    //     Debug.Log("DisplayReferendumInfo called");
    //     if (infoText == null)
    //     {
    //         Debug.LogError("infoText is null in DisplayReferendumInfo.");
    //         return;
    //     }

    //     JObject referendumJson = JObject.Parse(referendumDetails);
    //     JObject polkassemblyJson = JObject.Parse(polkassemblyData);

    //     referendumTitle.text = referendumJson["title"]?.ToString();
    //     referendumContent.text = referendumJson["content"]?.ToString();
    //     referendumStatus.text = referendumJson["status"]?.ToString();
    //     referendumEnd.text = referendumJson["end"]?.ToString();

    //     JArray commentsArray = (JArray)polkassemblyJson["comments"];
    //     if (commentsArray != null)
    //     {
    //         string comments = "";
    //         foreach (var comment in commentsArray)
    //         {
    //             comments += comment["content"]?.ToString() + "\n\n";
    //         }
    //         referendumComments.text = comments;
    //     }
    // }
}
