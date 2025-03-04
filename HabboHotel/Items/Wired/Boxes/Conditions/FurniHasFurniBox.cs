﻿using System.Collections.Concurrent;
using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Rooms;

namespace Plus.HabboHotel.Items.Wired.Boxes.Conditions;

internal class FurniHasFurniBox : IWiredItem
{
    public FurniHasFurniBox(Room instance, Item item)
    {
        Instance = instance;
        Item = item;
        SetItems = new ConcurrentDictionary<int, Item>();
    }

    public Room Instance { get; set; }

    public Item Item { get; set; }

    public WiredBoxType Type => WiredBoxType.ConditionFurniHasFurni;

    public ConcurrentDictionary<int, Item> SetItems { get; set; }

    public string StringData { get; set; }

    public bool BoolData { get; set; }

    public string ItemsData { get; set; }

    public void HandleSave(IIncomingPacket packet)
    {
        var unknown = packet.ReadInt();
        var unknown2 = packet.ReadString();
        if (SetItems.Count > 0)
            SetItems.Clear();
        var furniCount = packet.ReadInt();
        for (var i = 0; i < furniCount; i++)
        {
            var selectedItem = Instance.GetRoomItemHandler().GetItem(packet.ReadInt());
            if (selectedItem != null)
                SetItems.TryAdd(selectedItem.Id, selectedItem);
        }
    }

    public bool Execute(params object[] @params)
    {
        foreach (var item in SetItems.Values.ToList())
        {
            if (item == null || !Instance.GetRoomItemHandler().GetFloor.Contains(item))
                continue;
            var hasFurni = false;
            var items = Instance.GetGameMap().GetAllRoomItemForSquare(item.GetX, item.GetY);
            if (items.Count(x => x.GetZ >= item.GetZ) > 1)
                hasFurni = true;
            if (!hasFurni)
                return false;
        }
        return true;
    }
}