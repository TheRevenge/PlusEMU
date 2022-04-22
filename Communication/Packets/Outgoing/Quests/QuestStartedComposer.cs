﻿using Plus.HabboHotel.Quests;
using Plus.HabboHotel.GameClients;

namespace Plus.Communication.Packets.Outgoing.Quests
{
    class QuestStartedComposer : ServerPacket
    {
        public QuestStartedComposer(GameClient session, Quest quest)
            : base(ServerPacketHeader.QuestStartedMessageComposer)
        {
            SerializeQuest(this, session, quest, quest.Category);
        }

        private void SerializeQuest(ServerPacket message, GameClient session, Quest quest, string category)
        {
            if (message == null || session == null)
                return;

            var amountInCat = PlusEnvironment.GetGame().GetQuestManager().GetAmountOfQuestsInCategory(category);
            var number = quest == null ? amountInCat : quest.Number - 1;
            var userProgress = quest == null ? 0 : session.GetHabbo().GetQuestProgress(quest.Id);

            if (quest != null && quest.IsCompleted(userProgress))
                number++;

            message.WriteString(category);
            message.WriteInteger(quest == null ? 0 : ((quest.Category.Contains("xmas2012")) ? 0 : number));  // Quest progress in this cat
            message.WriteInteger(quest == null ? 0 : (quest.Category.Contains("xmas2012")) ? 0 : amountInCat); // Total quests in this cat
            message.WriteInteger(quest == null ? 3 : quest.RewardType);// Reward type (1 = Snowflakes, 2 = Love hearts, 3 = Pixels, 4 = Seashells, everything else is pixels
            message.WriteInteger(quest == null ? 0 : quest.Id); // Quest id
            message.WriteBoolean(quest == null ? false : session.GetHabbo().GetStats().QuestId == quest.Id);  // Quest started
            message.WriteString(quest == null ? string.Empty : quest.ActionName);
            message.WriteString(quest == null ? string.Empty : quest.DataBit);
            message.WriteInteger(quest == null ? 0 : quest.Reward);
            message.WriteString(quest == null ? string.Empty : quest.Name);
            message.WriteInteger(userProgress); // Current progress
            message.WriteInteger(quest == null ? 0 : quest.GoalData); // Target progress
            message.WriteInteger(quest == null ? 0 : quest.TimeUnlock); // "Next quest available countdown" in seconds
            message.WriteString("");
            message.WriteString("");
            message.WriteBoolean(true);
        }
    }
}
