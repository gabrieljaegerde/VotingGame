using Substrate.Integration.Model.PalletReferenda;
using Substrate.NetApi;
using Substrate.NetApi.Model.Types;
using Substrate.NetApi.Model.Types.Base;
using Substrate.NetApi.Model.Types.Primitive;
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

public class PolkadotManager : MonoBehaviour
{
    [SerializeField]
    private string _nodeUrl = "wss://polkadot-rpc.dwellir.com";

    private SubstrateClientExt _client;

    private void Start()
    {
        InitializeClient();
    }

    private void InitializeClient()
    {
        if (_client != null)
        {
            return;
        }

        _client = new SubstrateClientExt(new Uri(_nodeUrl), Substrate.NetApi.Model.Extrinsics.ChargeTransactionPayment.Default());
        ConnectClientAsync();
    }

    private async void ConnectClientAsync()
    {
        try
        {
            await _client.ConnectAsync();
            Debug.Log("Connected to Polkadot node");
            await GetAllReferendaAsync(_client, CancellationToken.None);
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

    static async Task GetAllReferendaAsync(SubstrateClientExt client, CancellationToken token)
    {
        Dictionary<U32, EnumReferendumInfo> referendumInfoDict = new Dictionary<U32, EnumReferendumInfo>();

        try
        {
            // getting a single one
            EnumReferendumInfo enumReferendumInfo = await client.ReferendaStorage.ReferendumInfoFor(new U32(0), token);
            referendumInfoDict[new U32(0)] = enumReferendumInfo;

            Dictionary<uint, ReferendumInfoSharp> finalDict = new Dictionary<uint, ReferendumInfoSharp>();
            foreach (var item in referendumInfoDict)
            {
                finalDict[item.Key.Value] = new ReferendumInfoSharp(item.Value);
            }

            foreach (var item in finalDict)
            {
                Debug.Log($"Referendum: {item.Key}");
                Debug.Log($"{JsonSerializer.Serialize(item.Value, new JsonSerializerOptions { WriteIndented = true, Converters = { new JsonStringEnumConverter(), new Substrate.Integration.Helper.BigIntegerConverter() } })}\n");
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
