﻿using Plus.Communication.Packets.Outgoing.LandingView;
using Plus.HabboHotel.GameClients;

namespace Plus.Communication.Packets.Incoming.LandingView
{
    class RefreshCampaignEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            try
            {
                var parseCampaings = packet.PopString();
                if (parseCampaings.Contains("gamesmaker"))
                    return;

                var campaingName = "";
                var parser = parseCampaings.Split(';');

                foreach (var value in parser)
                {
                    if (string.IsNullOrEmpty(value) || value.EndsWith(","))
                        continue;

                    var data = value.Split(',');
                    campaingName = data[1];
                }

                session.SendPacket(new CampaignComposer(parseCampaings, campaingName));
            }
            catch
            {
                //ignored
            }
        }
    }
}