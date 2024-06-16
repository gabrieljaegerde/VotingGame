using Substrate.Integration.Model.PalletReferenda;
using Substrate.NetApi;
using Substrate.NetApi.Model.Types;
using Substrate.NetApi.Model.Types.Base;
using Substrate.NetApi.Model.Types.Primitive;
using Substrate.Polkadot.NET.NetApiExt.Client;
using Substrate.Polkadot.NET.NetApiExt;
using Substrate.Polkadot.NET.NetApiExt.Generated;
using Substrate.Polkadot.NET.NetApiExt.Generated.Model.pallet_referenda.types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using System.Text.Json;
using System.Text.Json.Serialization;
using Substrate.Integration.Helper;
using Substrate;
using Substrate.Integration;
using TMPro;

public class PolkadotManager : MonoBehaviour
{
    [SerializeField]
    private string _nodeUrl = "wss://polkadot-rpc.dwellir.com";

    public GameObject towerPrefab;
    public GameObject infoWindow;
    public TextMeshProUGUI referendumIdText;
    public TextMeshProUGUI trackText;
    public TextMeshProUGUI statusText;

    private SubstrateNetwork client;
    private Dictionary<uint, ReferendumInfoSharp> ongoingReferenda;

    private void Start()
    {
        InitializeClient();
    }

    private void InitializeClient()
    {
        if (client != null)
        {
            return;
        }

        client = new SubstrateNetwork(null, _nodeUrl);
        ConnectClientAsync();
    }

    private async void ConnectClientAsync()
    {
        try
        {
            await client.ConnectAsync(true, true, CancellationToken.None);
            Debug.Log("Connected to Polkadot node");
            ongoingReferenda = await GetAllReferendaAsync(client, CancellationToken.None);
            CreateTowersForReferenda(ongoingReferenda);
        }
        catch (UriFormatException ex)
        {
            Debug.LogError($"Invalid URL format: {ex.Message}");
        }
        catch (TimeoutException ex)
        {
            Debug.LogError($"Connection timed out: {ex.Message}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to connect to Polkadot node: {ex.Message}");
        }
    }

    static async Task<Dictionary<uint, ReferendumInfoSharp>> GetAllReferendaAsync(SubstrateNetwork client, CancellationToken token)
    {
        try
        {
            Dictionary<U32, EnumReferendumInfo> referendumInfoDict = await client.GetAllStorageAsync<U32, EnumReferendumInfo>("Referenda", "ReferendumInfoFor", true, token);

            Debug.Log($"There are currently {referendumInfoDict.Count} referendas on Polkadot!");

            Dictionary<uint, ReferendumInfoSharp> finalDict = new Dictionary<uint, ReferendumInfoSharp>();
            foreach (var item in referendumInfoDict)
            {
                finalDict[item.Key.Value] = new ReferendumInfoSharp(item.Value);
            }

            var ongoingReferenda = finalDict.Where(item => item.Value.ReferendumInfo == ReferendumInfo.Ongoing).ToDictionary(item => item.Key, item => item.Value);

            Debug.Log($"There are currently {ongoingReferenda.Count} ongoing referendas on Polkadot!");

            foreach (var item in ongoingReferenda)
            {
                Debug.Log($"Ongoing Referendum: {item.Key}");
                Debug.Log($"{JsonSerializer.Serialize(item.Value, new JsonSerializerOptions { WriteIndented = true, Converters = { new JsonStringEnumConverter(), new Substrate.Integration.Helper.BigIntegerConverter() } })}\n");
            }

            return ongoingReferenda;
        }
        catch (OperationCanceledException ex)
        {
            Debug.LogError($"Operation was canceled: {ex.Message}");
            return new Dictionary<uint, ReferendumInfoSharp>();
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error fetching referenda: {ex.Message}");
            return new Dictionary<uint, ReferendumInfoSharp>();
        }
    }

    private void CreateTowersForReferenda(Dictionary<uint, ReferendumInfoSharp> referenda)
    {
        var groupedReferenda = referenda.GroupBy(r => r.Value.ReferendumStatus.Track);

        float xOffset = 0;
        float yOffset = 0;
        float towerSpacing = 2.0f;
        float trackSpacing = 5.0f;

        foreach (var group in groupedReferenda)
        {
            foreach (var referendum in group)
            {
                Vector3 position = new Vector3(xOffset, yOffset, 0);
                CreateTowerForReferendum(position, referendum.Key);
                xOffset += towerSpacing;
            }
            xOffset = 0;
            yOffset += trackSpacing;
        }
    }

    private void CreateTowerForReferendum(Vector3 position, uint referendumId)
    {
        try
        {
            GameObject tower = Instantiate(towerPrefab, position, Quaternion.identity);
            Tower towerComponent = tower.GetComponent<Tower>();
            towerComponent.ReferendumId = (int)referendumId;

            // Assign the Info Window UI elements
            towerComponent.infoWindow = infoWindow;
            towerComponent.referendumIdText = referendumIdText;
            towerComponent.trackText = trackText;
            towerComponent.statusText = statusText;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to create tower for referendum {referendumId}: {ex.Message}");
        }
    }

    public ReferendumInfoSharp GetReferendumInfo(int referendumId)
    {
        ongoingReferenda.TryGetValue((uint)referendumId, out ReferendumInfoSharp referendumInfo);
        return referendumInfo;
    }
}
