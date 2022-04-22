﻿namespace Plus.HabboHotel.Rooms.Chat.Commands.Moderator.Fun
{
    class CoordsCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_coords"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "Used to get your current position within the room."; }
        }

        public void Execute(GameClients.GameClient session, Room room, string[] @params)
        {
            var thisUser = room.GetRoomUserManager().GetRoomUserByHabbo(session.GetHabbo().Id);
            if (thisUser == null)
                return;

            session.SendNotification("X: " + thisUser.X + "\n - Y: " + thisUser.Y + "\n - Z: " + thisUser.Z + "\n - Rot: " + thisUser.RotBody + ", sqState: " + room.GetGameMap().GameMap[thisUser.X, thisUser.Y].ToString() + "\n\n - RoomID: " + session.GetHabbo().CurrentRoomId);                           
        }
    }
}
