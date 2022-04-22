﻿using Plus.HabboHotel.GameClients;

namespace Plus.HabboHotel.Rooms.Chat.Commands.Moderator
{
    class KickCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_kick"; }
        }

        public string Parameters
        {
            get { return "%username% %reason%"; }
        }

        public string Description
        {
            get { return "Kick a user from a room and send them a reason."; }
        }

        public void Execute(GameClient session, Room room, string[] @params)
        {
            if (@params.Length == 1)
            {
                session.SendWhisper("Please enter the username of the user you wish to summon.");
                return;
            }

            var targetClient = PlusEnvironment.GetGame().GetClientManager().GetClientByUsername(@params[1]);
            if (targetClient == null)
            {
                session.SendWhisper("An error occoured whilst finding that user, maybe they're not online.");
                return;
            }

            if (targetClient.GetHabbo() == null)
            {
                session.SendWhisper("An error occoured whilst finding that user, maybe they're not online.");
                return;
            }

            if (targetClient.GetHabbo().Username == session.GetHabbo().Username)
            {
                session.SendWhisper("Get a life.");
                return;
            }

            if (!targetClient.GetHabbo().InRoom)
            {
                session.SendWhisper("That user currently isn't in a room.");
                return;
            }

            Room targetRoom;
            if (!PlusEnvironment.GetGame().GetRoomManager().TryGetRoom(targetClient.GetHabbo().CurrentRoomId, out targetRoom))
                return;

            if (@params.Length > 2)
                targetClient.SendNotification("A moderator has kicked you from the room for the following reason: " + CommandManager.MergeParams(@params, 2));
            else
                targetClient.SendNotification("A moderator has kicked you from the room.");

            targetRoom.GetRoomUserManager().RemoveUserFromRoom(targetClient, true, false);
        }
    }
}
