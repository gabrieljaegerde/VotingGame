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


        // _client = new SubstrateClientExt(new Uri(_nodeUrl), Substrate.NetApi.Model.Extrinsics.ChargeTransactionPayment.Default());
        ConnectClientAsync();
    }

    private async void ConnectClientAsync()
    {
        try
        {
            // await _client.ConnectAsync();
            await client.ConnectAsync(true, true, CancellationToken.None);
            Debug.Log("Connected to Polkadot node");
            await GetAllReferendaAsync(client, CancellationToken.None);
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

    static async Task GetAllReferendaAsync(SubstrateNetwork client, CancellationToken token)
    {

        try
        {
            // getting all referendas
            Dictionary<U32, EnumReferendumInfo> referendumInfoDict = await client.GetAllStorageAsync<U32, EnumReferendumInfo>("Referenda", "ReferendumInfoFor", true, token);

            Debug.Log($"There are currently {referendumInfoDict.Count} referendas on Polkadot!");

            // getting a single one
            EnumReferendumInfo enumReferendumInfo = await client.SubstrateClient.ReferendaStorage.ReferendumInfoFor(referendumInfoDict.Keys.First(), null, token);

            Debug.Log($"The referanda with the key {referendumInfoDict.Keys.First().Value} has the following information {enumReferendumInfo.Value}!");


            Dictionary<uint, ReferendumInfoSharp> finalDict = new Dictionary<uint, ReferendumInfoSharp>();
            foreach (var item in referendumInfoDict)
            {
                finalDict[item.Key.Value] = new ReferendumInfoSharp(item.Value);
            }

            // Now `finalDict` contains `uint` keys and `ReferendumInfoSharp` values
            foreach (var item in finalDict)
            {
                Debug.Log($"Referendum: {item.Key}");
                Debug.Log($"{JsonSerializer.Serialize(item.Value, new JsonSerializerOptions { WriteIndented = true, Converters = { new JsonStringEnumConverter(), new Substrate.Integration.Helper.BigIntegerConverter() } })}\n" );
            }

        }
        catch (OperationCanceledException ex)
        {
            Debug.LogError($"Operation was canceled: {ex.Message}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error fetching referenda: {ex.Message}");
        }
    }
}
