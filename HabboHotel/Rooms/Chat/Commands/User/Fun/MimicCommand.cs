﻿using Plus.HabboHotel.GameClients;
using Plus.Communication.Packets.Outgoing.Rooms.Engine;
using Plus.Database.Interfaces;
using Plus.Communication.Packets.Outgoing.Rooms.Avatar;

namespace Plus.HabboHotel.Rooms.Chat.Commands.User.Fun
{
    class MimicCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_mimic"; }
        }

        public string Parameters
        {
            get { return "%username%"; }
        }

        public string Description
        {
            get { return "Liking someone elses swag? Copy it!"; }
        }

        public void Execute(GameClient session, Room room, string[] @params)
        {
            if (@params.Length == 1)
            {
                session.SendWhisper("Please enter the username of the user you wish to mimic.");
                return;
            }

            var targetClient = PlusEnvironment.GetGame().GetClientManager().GetClientByUsername(@params[1]);
            if (targetClient == null)
            {
                session.SendWhisper("An error occoured whilst finding that user, maybe they're not online.");
                return;
            }

            if (!targetClient.GetHabbo().AllowMimic)
            {
                session.SendWhisper("Oops, you cannot mimic this user - sorry!");
                return;
            }

            var targetUser = session.GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(targetClient.GetHabbo().Id);
            if (targetUser == null)
            {
                session.SendWhisper("An error occoured whilst finding that user, maybe they're not online or in this room.");
                return;
            }

            session.GetHabbo().Gender = targetUser.GetClient().GetHabbo().Gender;
            session.GetHabbo().Look = targetUser.GetClient().GetHabbo().Look;

            using (var dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE `users` SET `gender` = @gender, `look` = @look WHERE `id` = @id LIMIT 1");
                dbClient.AddParameter("gender", session.GetHabbo().Gender);
                dbClient.AddParameter("look", session.GetHabbo().Look);
                dbClient.AddParameter("id", session.GetHabbo().Id);
                dbClient.RunQuery();
            }

            var user = room.GetRoomUserManager().GetRoomUserByHabbo(session.GetHabbo().Id);
            if (user != null)
            {
                session.SendPacket(new AvatarAspectUpdateComposer(session.GetHabbo().Look, session.GetHabbo().Gender));
                session.SendPacket(new UserChangeComposer(user, true));
                room.SendPacket(new UserChangeComposer(user, false));
            }
        }
    }
}