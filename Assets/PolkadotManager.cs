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

public class PolkadotManager : MonoBehaviour
{
    [SerializeField]
    private string _nodeUrl = "wss://polkadot-rpc.dwellir.com";

    private SubstrateNetwork client;

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
            var ongoingReferenda = await GetAllReferendaAsync(client, CancellationToken.None);
            foreach (var referendum in ongoingReferenda)
            {
                Debug.Log(referendum);
            }
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

    static async Task<string[]> GetAllReferendaAsync(SubstrateNetwork client, CancellationToken token)
    {
        try
        {
            // Getting all referendas
            Dictionary<U32, EnumReferendumInfo> referendumInfoDict = await client.GetAllStorageAsync<U32, EnumReferendumInfo>("Referenda", "ReferendumInfoFor", true, token);

            Debug.Log($"There are currently {referendumInfoDict.Count} referendas on Polkadot!");

            // Getting a single one
            EnumReferendumInfo enumReferendumInfo = await client.SubstrateClient.ReferendaStorage.ReferendumInfoFor(referendumInfoDict.Keys.First(), null, token);

            Debug.Log($"The referanda with the key {referendumInfoDict.Keys.First().Value} has the following information {enumReferendumInfo.Value}!");

            Dictionary<uint, ReferendumInfoSharp> finalDict = new Dictionary<uint, ReferendumInfoSharp>();
            foreach (var item in referendumInfoDict)
            {
                finalDict[item.Key.Value] = new ReferendumInfoSharp(item.Value);
            }

            // Filter out the referenda that are not "Ongoing"
            var ongoingReferenda = finalDict.Where(item => item.Value.ReferendumInfo == ReferendumInfo.Ongoing).ToDictionary(item => item.Key, item => item.Value);

            Debug.Log($"There are currently {ongoingReferenda.Count} ongoing referendas on Polkadot!");

            // Construct the array of strings in the format "key: { value }"
            var ongoingReferendaArray = ongoingReferenda.Select(item => $"{item.Key}: {JsonSerializer.Serialize(item.Value, new JsonSerializerOptions { WriteIndented = true, Converters = { new JsonStringEnumConverter(), new Substrate.Integration.Helper.BigIntegerConverter() } })}").ToArray();

            return ongoingReferendaArray;
        }
        catch (OperationCanceledException ex)
        {
            Debug.LogError($"Operation was canceled: {ex.Message}");
            return Array.Empty<string>();
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error fetching referenda: {ex.Message}");
            return Array.Empty<string>();
        }
    }
}
