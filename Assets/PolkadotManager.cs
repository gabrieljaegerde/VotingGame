using Substrate.NetApi;
using Substrate.NetApi.Model.Types;
using Substrate.NetApi.Model.Types.Primitive;
using Substrate.Polkadot.NET.NetApiExt.Generated;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

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
            await FetchAllReferenda();
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to connect to Polkadot node: {ex.Message}");
        }
    }

    private async Task FetchAllReferenda()
    {
        try
        {
            List<ReferendumInfo> referenda = await GetAllReferendaAsync(_client, CancellationToken.None);

            foreach (var referendum in referenda)
            {
                Debug.Log($"Referendum ID: {referendum.Id}, Status: {referendum.Status}, Details:");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to fetch referenda: {ex.Message}");
        }
    }

    /// <summary>
    /// Helper method that queries all referenda.
    /// </summary>
    /// <param name="client">Your SubstrateClient</param>
    /// <param name="token">CancellationToken</param>
    /// <returns>List<ReferendumInfo></returns>
    static async Task<List<ReferendumInfo>> GetAllReferendaAsync(SubstrateClientExt client, CancellationToken token)
    {
        // Get the Storage prefix
        byte[] prefix = RequestGenerator.GetStorageKeyBytesHash("Referenda", "ReferendumInfoFor");

        // First startKey is unknown
        byte[] startKey = null;

        List<string[]> storageChanges = new List<string[]>();

        while (true)
        {
            // Get List of all of the keys starting with the Storage prefix
            var keysPaged = await client.State.GetKeysPagedAsync(prefix, 1000, startKey, string.Empty, token);
            Debug.Log($"keysPaged: {keysPaged[0]}");



            if (keysPaged == null || !keysPaged.Any())
            {
                break;
            }
            else
            {
                // temp variable
                var tt = await client.State.GetQueryStorageAtAsync(keysPaged.Select(p => Utils.HexToByteArray(p.ToString())).ToList(), string.Empty, token);

                storageChanges.AddRange(new List<string[]>(tt.ElementAt(0).Changes));

                // update the startKey to query next keys
                Debug.Log($"startKey: {startKey}");

                startKey = Utils.HexToByteArray(tt.ElementAt(0).Changes.Last()[0]);
            }
        }

        // Result list
        var referendumInfoList = new List<ReferendumInfo>();

        if (storageChanges != null)
        {
            foreach (var storageChangeSet in storageChanges)
            {
                // Decoding Storage changes to the desired type.
                var referendumInfo = new ReferendumInfo();

                Debug.Log($"Storagechangeset0: {storageChangeSet[0]}");
                Debug.Log($"Storagechangeset1: {storageChangeSet[1]}");
                Debug.Log($"Storagechangeset1: {storageChangeSet[2]}");
                int offset = 0;

                var parts = Utils.HexToByteArray(storageChangeSet[0]);
                var id = new U32();
                id.Decode(parts, ref offset);

                Debug.Log($"Id: {id}");



                var status = "Rejected"; // This should be determined based on the encoded data structure.
                referendumInfo.Create(id, status, Utils.HexToByteArray(storageChangeSet[1]), ref offset);

                referendumInfoList.Add(referendumInfo);
            }
        }

        return referendumInfoList;
    }


    public class ReferendumInfo
    {
        public U32 Id { get; set; }
        public string Status { get; set; }
        public OngoingInfo Ongoing { get; set; }
        public ApprovedInfo Approved { get; set; }
        public RejectedInfo Rejected { get; set; }

        public void Create(U32 id, string status, byte[] data, ref int offset)
        {
            Id = id;
            Status = status;

            switch (status)
            {
                case "Ongoing":
                    Ongoing = new OngoingInfo();
                    Ongoing.Create(data, ref offset);
                    break;

                case "Approved":
                    Approved = new ApprovedInfo();
                    Approved.Create(data, ref offset);
                    break;

                case "Rejected":
                    Rejected = new RejectedInfo();
                    Rejected.Create(data, ref offset);
                    break;
            }
        }
    }

    public class OngoingInfo
    {
        public U32 Track { get; set; }
        public string Origin { get; set; }
        public ProposalInfo Proposal { get; set; }
        public EnactmentInfo Enactment { get; set; }
        public U32 Submitted { get; set; }
        public DepositInfo SubmissionDeposit { get; set; }
        public DepositInfo DecisionDeposit { get; set; }
        public DecidingInfo Deciding { get; set; }
        public TallyInfo Tally { get; set; }
        public bool InQueue { get; set; }
        public AlarmInfo Alarm { get; set; }

        public void Create(byte[] data, ref int offset)
        {
            Track = new U32();
            Track.Decode(data, ref offset);

            Debug.Log($"Track: {Track}");


            var originBytes = new byte[32];
            Array.Copy(data, offset, originBytes, 0, 32);
            Origin = System.Text.Encoding.UTF8.GetString(originBytes);
            offset += 32;

            Debug.Log($"Origin: {Origin}");

            Proposal = new ProposalInfo();
            Proposal.Create(data, ref offset);

            Debug.Log($"Proposal: {Proposal}");

            Enactment = new EnactmentInfo();
            Enactment.Create(data, ref offset);

            Submitted = new U32();
            Submitted.Decode(data, ref offset);

            SubmissionDeposit = new DepositInfo();
            SubmissionDeposit.Create(data, ref offset);

            DecisionDeposit = new DepositInfo();
            DecisionDeposit.Create(data, ref offset);

            Deciding = new DecidingInfo();
            Deciding.Create(data, ref offset);

            Tally = new TallyInfo();
            Tally.Create(data, ref offset);

            var inQueue = new Bool();
            inQueue.Decode(data, ref offset);
            InQueue = inQueue.Value;

            Alarm = new AlarmInfo();
            Alarm.Create(data, ref offset);
        }
    }

    public class ApprovedInfo
    {
        public U32 Since { get; set; }
        public object Confirming { get; set; }

        public void Create(byte[] data, ref int offset)
        {
            Since = new U32();
            Since.Decode(data, ref offset);

            var confirmingBytes = new byte[32];
            Array.Copy(data, offset, confirmingBytes, 0, 32);
            Confirming = System.Text.Encoding.UTF8.GetString(confirmingBytes);
            offset += 32;
        }
    }

    public class RejectedInfo
    {
        public U32 Since { get; set; }
        public DepositInfo ReturnDeposit { get; set; }
        public object Confirming { get; set; }

        public void Create(byte[] data, ref int offset)
        {
            Since = new U32();
            Since.Decode(data, ref offset);

            ReturnDeposit = new DepositInfo();
            ReturnDeposit.Create(data, ref offset);

            var confirmingBytes = new byte[32];
            Array.Copy(data, offset, confirmingBytes, 0, 32);
            Confirming = System.Text.Encoding.UTF8.GetString(confirmingBytes);
            offset += 32;
        }
    }

    public class ProposalInfo
    {
        public string Hash { get; set; }
        public U32 Len { get; set; }

        public void Create(byte[] data, ref int offset)
        {
            var hashArray = new byte[32];
            Array.Copy(data, offset, hashArray, 0, 32);
            Hash = Utils.Bytes2HexString(hashArray);
            offset += 32;

            Len = new U32();
            Len.Decode(data, ref offset);
        }
    }

    public class EnactmentInfo
    {
        public U32 After { get; set; }

        public void Create(byte[] data, ref int offset)
        {
            After = new U32();
            After.Decode(data, ref offset);
        }
    }

    public class DepositInfo
    {
        public string Who { get; set; }
        public U128 Amount { get; set; }

        public void Create(byte[] data, ref int offset)
        {
            var whoArray = new byte[32];
            Array.Copy(data, offset, whoArray, 0, 32);
            Who = Utils.Bytes2HexString(whoArray);
            offset += 32;

            Amount = new U128();
            Amount.Decode(data, ref offset);
        }
    }

    public class DecidingInfo
    {
        public U32 Since { get; set; }
        public object Confirming { get; set; }

        public void Create(byte[] data, ref int offset)
        {
            Since = new U32();
            Since.Decode(data, ref offset);

            var confirmingBytes = new byte[32];
            Array.Copy(data, offset, confirmingBytes, 0, 32);
            Confirming = System.Text.Encoding.UTF8.GetString(confirmingBytes);
            offset += 32;
        }
    }

    public class TallyInfo
    {
        public U128 Ayes { get; set; }
        public U128 Nays { get; set; }
        public U128 Support { get; set; }

        public void Create(byte[] data, ref int offset)
        {
            Ayes = new U128();
            Ayes.Decode(data, ref offset);

            Nays = new U128();
            Nays.Decode(data, ref offset);

            Support = new U128();
            Support.Decode(data, ref offset);
        }
    }

    public class AlarmInfo
    {
        public U32 Alarm1 { get; set; }
        public List<U32> Alarm2 { get; set; }

        public void Create(byte[] data, ref int offset)
        {
            Alarm1 = new U32();
            Alarm1.Decode(data, ref offset);

            Alarm2 = new List<U32>();
            for (int i = 0; i < 2; i++)
            {
                var alarm = new U32();
                alarm.Decode(data, ref offset);
                Alarm2.Add(alarm);
            }
        }
    }
}
