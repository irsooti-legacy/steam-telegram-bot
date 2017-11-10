using SteamWebAPI2;
using SteamWebAPI2.Interfaces;
using Steam.Models.SteamCommunity;
using System.Threading.Tasks;
using SteamWebAPI2.Utilities;

namespace SteamInfoPlayerBot.Services
{
    public class SteamService
    {
        private static ISteamUser steamInterface;
        public SteamService(string token)
        {
            steamInterface = new SteamUser(token);
        }

        public async Task<PlayerSummaryModel> PlayerInfo(string val)
        {
            ulong num = 0u;
            if (ulong.TryParse(val, out num)) {
                return await PlayerInfoId(num);
            }

            else {
                var steamId = await PlayerInfoVanity(val);
                return await PlayerInfoId(steamId);
            }
        }

        public async Task<PlayerSummaryModel> PlayerInfoId(ulong steamId)
        {
            ISteamWebResponse<PlayerSummaryModel> player = await steamInterface.GetPlayerSummaryAsync(steamId);
            return player.Data;
        }

        public async Task<ulong> PlayerInfoVanity(string steamId)
        {
            ISteamWebResponse<ulong> playerId = await steamInterface.ResolveVanityUrlAsync(steamId);
            return playerId.Data;
        }
    }
}