﻿using System.Linq;
using Plus.HabboHotel.GameClients;

namespace Plus.Communication.Rcon.Commands.Hotel
{
    class ReloadRanksCommand : IRconCommand
    {
        public string Description
        {
            get { return "This command is used to reload user permissions."; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public bool TryExecute(string[] parameters)
        {
            PlusEnvironment.GetGame().GetPermissionManager().Init();

            foreach (var client in PlusEnvironment.GetGame().GetClientManager().GetClients.ToList())
            {
                if (client == null || client.GetHabbo() == null || client.GetHabbo().GetPermissions() == null)
                    continue;

                client.GetHabbo().GetPermissions().Init(client.GetHabbo());
            }
            
            return true;
        }
    }
}