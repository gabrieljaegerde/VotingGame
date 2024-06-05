using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Tower : MonoBehaviour
{
    public int ReferendumId;
    private PolkadotManager polkadotManager;

    void Start()
    {
        polkadotManager = GameObject.Find("PolkadotManager").GetComponent<PolkadotManager>();
    }

    void OnMouseDown()
    {
        // StartCoroutine(FetchReferendumDetails());
    }

    // IEnumerator FetchReferendumDetails()
    // {
        // var referendumTask = polkadotManager.GetReferendumDetails(ReferendumId);
        // yield return new WaitUntil(() => referendumTask.IsCompleted);
        // var referendumDetails = referendumTask.Result;
        // StartCoroutine(FetchPolkassemblyData(referendumDetails));
    // }

    // IEnumerator FetchPolkassemblyData(string referendumDetails)
    // {
    //     string url = $"https://api.polkassembly.io/api/v1/posts/on-chain-post?proposalType=referendums_v2&postId={ReferendumId}";
    //     UnityWebRequest request = UnityWebRequest.Get(url);
    //     request.SetRequestHeader("x-network", "polkadot");
    //     yield return request.SendWebRequest();

    //     if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
    //     {
    //         Debug.LogError(request.error);
    //     }
    //     else
    //     {
    //         string response = request.downloadHandler.text;
    //         UIManager.Instance.DisplayReferendumInfo(this, referendumDetails, response);
    //     }
    // }
}
