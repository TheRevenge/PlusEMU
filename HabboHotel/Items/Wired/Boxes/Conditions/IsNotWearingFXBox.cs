﻿using System;
using System.Collections.Concurrent;

using Plus.Communication.Packets.Incoming;
using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.Users;

namespace Plus.HabboHotel.Items.Wired.Boxes.Conditions
{
    class IsNotWearingFxBox : IWiredItem
    {
        public Room Instance { get; set; }
        public Item Item { get; set; }
        public WiredBoxType Type { get { return WiredBoxType.ConditionIsWearingFx; } }
        public ConcurrentDictionary<int, Item> SetItems { get; set; }
        public string StringData { get; set; }
        public bool BoolData { get; set; }
        public string ItemsData { get; set; }

        public IsNotWearingFxBox(Room instance, Item item)
        {
            this.Instance = instance;
            this.Item = item;
            SetItems = new ConcurrentDictionary<int, Item>();
        }

        public void HandleSave(ClientPacket packet)
        {
            var unknown = packet.PopInt();
            var unknown2 = packet.PopInt();

            StringData = unknown2.ToString();
        }

        public bool Execute(params object[] @params)
        {
            if (@params.Length == 0)
                return false;

            if (String.IsNullOrEmpty(StringData))
                return false;

            var player = (Habbo)@params[0];
            if (player == null)
                return false;

            if (player.Effects().CurrentEffect != int.Parse(StringData))
                return true;
            return false;
        }
    }
}
