﻿using Plus.HabboHotel.Users;
using Plus.Communication.Packets.Outgoing.Users;
using Plus.HabboHotel.GameClients;

namespace Plus.Communication.Packets.Incoming.Users
{
    class GetSelectedBadgesEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            var userId = packet.PopInt();
            var habbo = PlusEnvironment.GetHabboById(userId);
            if (habbo == null)
                return;

            session.SendPacket(new HabboUserBadgesComposer(habbo));
        }
    }
}