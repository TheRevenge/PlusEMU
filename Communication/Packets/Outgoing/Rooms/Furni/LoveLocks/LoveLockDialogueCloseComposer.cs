﻿using Plus.HabboHotel.GameClients;

namespace Plus.Communication.Packets.Outgoing.Rooms.Furni.LoveLocks;

public class LoveLockDialogueCloseComposer : IServerPacket
{
    private readonly int _itemId;
    public uint MessageId => ServerPacketHeader.LoveLockDialogueCloseComposer;

    public LoveLockDialogueCloseComposer(int itemId)
    {
        _itemId = itemId;
    }

    public void Compose(IOutgoingPacket packet) => packet.WriteInteger(_itemId);
}