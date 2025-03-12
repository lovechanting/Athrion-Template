using PlayFab.ClientModels;
using PlayFab;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Athrion
{
    internal class PingPatch
    {
        public static void GetPlayerPing()
        {
            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                PlayFabClientAPI.GetPlayerCombinedInfo(
                    new GetPlayerCombinedInfoRequest
                    {
                        PlayFabId = vrrig.Creator.GetPlayerRef().UserId,
                        InfoRequestParameters = new GetPlayerCombinedInfoRequestParams
                        {
                            GetUserAccountInfo = true
                        }
                    },
                    result =>
                    {
                        stopwatch.Stop();

                        string notifimessage = $"Ping to {vrrig.Creator.GetPlayerRef().NickName} successful. Response time: {stopwatch.ElapsedMilliseconds}ms";
                        Debug.WriteLine(notifimessage);

                    },
                    error =>
                    {
                        stopwatch.Stop();

                        string notifimessage = $"Ping to {vrrig.Creator.GetPlayerRef().NickName} failed. Time taken: {stopwatch.ElapsedMilliseconds}ms";
                        Debug.WriteLine(notifimessage);
                    }
                );
            }
        }
    }
}
