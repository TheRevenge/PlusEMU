﻿using System;
using System.Collections.Concurrent;

using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.Users;
using Plus.Communication.Packets.Incoming;

namespace Plus.HabboHotel.Items.Wired.Boxes.Effects
{
    class BotFollowsUserBox: IWiredItem
    {
        public Room Instance { get; set; }
        public Item Item { get; set; }
        public WiredBoxType Type { get { return WiredBoxType.EffectBotFollowsUserBox; } }
        public ConcurrentDictionary<int, Item> SetItems { get; set; }
        public string StringData { get; set; }
        public bool BoolData { get; set; }
        public string ItemsData { get; set; }

        public BotFollowsUserBox(Room instance, Item item)
        {
            this.Instance = instance;
            this.Item = item;
            SetItems = new ConcurrentDictionary<int, Item>();
        }

        public void HandleSave(ClientPacket packet)
        {
            var unknown = packet.PopInt();
            var followMode = packet.PopInt();//1 = follow, 0 = don't.
            var botConfiguration = packet.PopString();
       
            if (SetItems.Count > 0)
                SetItems.Clear();

            StringData = followMode + ";" +botConfiguration;
        }

        public bool Execute(params object[] @params)
        {
            if (@params == null || @params.Length == 0)
                return false;

            if (String.IsNullOrEmpty(StringData))
                return false;

            var player = (Habbo)@params[0];
            if (player == null)
                return false;

            var human = Instance.GetRoomUserManager().GetRoomUserByHabbo(player.Id);
            if (human == null)
                return false;

            var stuff = StringData.Split(';');
            if (stuff.Length != 2)
                return false;//This is important, incase a cunt scripts.

            var username = stuff[1];

            var user = Instance.GetRoomUserManager().GetBotByName(username);
            if (user == null)
                return false;

            var followMode = 0;
            if (!int.TryParse(stuff[0], out followMode))
                return false;

            if (followMode == 0)
            {
                user.BotData.ForcedUserTargetMovement = 0;

                if (user.IsWalking)
                    user.ClearMovement(true);
            }
            else if (followMode == 1)
            {
                user.BotData.ForcedUserTargetMovement = player.Id;

                if (user.IsWalking)
                    user.ClearMovement(true);
                user.MoveTo(human.X, human.Y);
            }

            return true;
        }
    }
}