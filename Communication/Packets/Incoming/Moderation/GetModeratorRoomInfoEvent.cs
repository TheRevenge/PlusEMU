﻿using Plus.HabboHotel.Rooms;
using Plus.Communication.Packets.Outgoing.Moderation;
using Plus.HabboHotel.GameClients;

namespace Plus.Communication.Packets.Incoming.Moderation
{
    class GetModeratorRoomInfoEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            if (!session.GetHabbo().GetPermissions().HasRight("mod_tool"))
                return;

            var roomId = packet.PopInt();

            if (!RoomFactory.TryGetData(roomId, out var data))
                return;

            if (!PlusEnvironment.GetGame().GetRoomManager().TryGetRoom(roomId, out var room))
                return;

            session.SendPacket(new ModeratorRoomInfoComposer(data, room.GetRoomUserManager().GetRoomUserByHabbo(data.OwnerName) != null));
        }
    }
}
