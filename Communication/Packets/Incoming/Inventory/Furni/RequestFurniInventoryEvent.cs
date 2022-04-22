﻿using System.Linq;
using System.Collections.Generic;

using Plus.HabboHotel.Items;
using Plus.Communication.Packets.Outgoing.Inventory.Furni;
using Plus.HabboHotel.GameClients;

namespace Plus.Communication.Packets.Incoming.Inventory.Furni
{
    class RequestFurniInventoryEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            var items = session.GetHabbo().GetInventoryComponent().GetWallAndFloor;

            var page = 0;
            var pages = ((items.Count() - 1) / 700) + 1;

            if (!items.Any())
            {
                session.SendPacket(new FurniListComposer(items.ToList(), 1, 0));
            }
            else
            {
                foreach (ICollection<Item> batch in items.Chunk(700))
                {
                    session.SendPacket(new FurniListComposer(batch.ToList(), pages, page));

                    page++;
                }
            }
        }
    }
}
