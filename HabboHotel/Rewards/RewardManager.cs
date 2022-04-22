﻿using Plus.Database.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using Plus.HabboHotel.GameClients;
using Plus.Communication.Packets.Outgoing.Inventory.Purse;

namespace Plus.HabboHotel.Rewards
{
    public class RewardManager
    {
        private ConcurrentDictionary<int, Reward> _rewards;
        private ConcurrentDictionary<int, List<int>> _rewardLogs;

        public RewardManager()
        {
            _rewards = new ConcurrentDictionary<int, Reward>();
            _rewardLogs = new ConcurrentDictionary<int, List<int>>();
        }

        public void Init()
        {
            using var dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor();
            dbClient.SetQuery("SELECT * FROM `server_rewards` WHERE enabled = '1'");
            var dTable = dbClient.GetTable();
            if (dTable != null)
            {
                foreach (DataRow dRow in dTable.Rows)
                {
                    _rewards.TryAdd((int)dRow["id"], new Reward(Convert.ToDouble(dRow["reward_start"]), Convert.ToDouble(dRow["reward_end"]), Convert.ToString(dRow["reward_type"]), Convert.ToString(dRow["reward_data"]), Convert.ToString(dRow["message"])));
                }
            }

            dbClient.SetQuery("SELECT * FROM `server_reward_logs`");
            dTable = dbClient.GetTable();
            if (dTable != null)
            {
                foreach (DataRow dRow in dTable.Rows)
                {
                    var id = (int)dRow["user_id"];
                    var rewardId = (int)dRow["reward_id"];

                    if (!_rewardLogs.ContainsKey(id))
                        _rewardLogs.TryAdd(id, new List<int>());

                    if (!_rewardLogs[id].Contains(rewardId))
                        _rewardLogs[id].Add(rewardId);
                }
            }
        }

        public bool HasReward(int id, int rewardId)
        {
            if (!_rewardLogs.ContainsKey(id))
                return false;

            if (_rewardLogs[id].Contains(rewardId))
                return true;

            return false;
        }

        public void LogReward(int id, int rewardId)
        {
            if (!_rewardLogs.ContainsKey(id))
                _rewardLogs.TryAdd(id, new List<int>());

            if (!_rewardLogs[id].Contains(rewardId))
                _rewardLogs[id].Add(rewardId);
            using var dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor();
            dbClient.SetQuery("INSERT INTO `server_reward_logs` VALUES ('', @userId, @rewardId)");
            dbClient.AddParameter("userId", id);
            dbClient.AddParameter("rewardId", rewardId);
            dbClient.RunQuery();
        }

        public void CheckRewards(GameClient session)
        {
            if (session == null || session.GetHabbo() == null)
                return;

            foreach (var entry in _rewards)
            {
                var id = entry.Key;
                var reward = entry.Value;

                if (HasReward(session.GetHabbo().Id, id))
                    continue;

                if (reward.Active)
                {
                    switch (reward.Type)
                    {
                        case RewardType.Badge:
                            {
                                if (!session.GetHabbo().GetBadgeComponent().HasBadge(reward.RewardData))
                                    session.GetHabbo().GetBadgeComponent().GiveBadge(reward.RewardData, true, session);
                                break;
                            }

                        case RewardType.Credits:
                            {
                                session.GetHabbo().Credits += Convert.ToInt32(reward.RewardData);
                                session.SendPacket(new CreditBalanceComposer(session.GetHabbo().Credits));
                                break;
                            }

                        case RewardType.Duckets:
                            {
                                session.GetHabbo().Duckets += Convert.ToInt32(reward.RewardData);
                                session.SendPacket(new HabboActivityPointNotificationComposer(session.GetHabbo().Duckets, Convert.ToInt32(reward.RewardData)));
                                break;
                            }

                        case RewardType.Diamonds:
                            {
                                session.GetHabbo().Diamonds += Convert.ToInt32(reward.RewardData);
                                session.SendPacket(new HabboActivityPointNotificationComposer(session.GetHabbo().Diamonds, Convert.ToInt32(reward.RewardData), 5));
                                break;
                            }
                    }

                    if (!String.IsNullOrEmpty(reward.Message))
                        session.SendNotification(reward.Message);

                    LogReward(session.GetHabbo().Id, id);
                }
                else
                    continue;
            }
        }
    }
}
