﻿using Plus.Communication.Packets.Outgoing.Navigator;

using Plus.Database.Interfaces;
using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Rooms;

namespace Plus.Communication.Packets.Incoming.Navigator
{
    public class AddFavouriteRoomEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            if (session == null)
                return;

            var roomId = packet.PopInt();

            if (!RoomFactory.TryGetData(roomId, out var data))
                return;

            if (data == null || session.GetHabbo().FavoriteRooms.Count >= 30 || session.GetHabbo().FavoriteRooms.Contains(roomId))
            {
                // send packet that favourites is full.
                return;
            }

            session.GetHabbo().FavoriteRooms.Add(roomId);
            session.SendPacket(new UpdateFavouriteRoomComposer(roomId, true));
            using var dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor();
            dbClient.RunQuery("INSERT INTO user_favorites (user_id,room_id) VALUES (" + session.GetHabbo().Id + "," + roomId + ")");
        }
    }
}