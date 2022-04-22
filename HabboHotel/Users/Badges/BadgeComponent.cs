﻿using System.Collections.Generic;

using Plus.HabboHotel.GameClients;
using Plus.Communication.Packets.Outgoing.Inventory.Badges;
using Plus.Communication.Packets.Outgoing.Inventory.Furni;

using Plus.Database.Interfaces;
using Plus.HabboHotel.Badges;

namespace Plus.HabboHotel.Users.Badges
{
    public class BadgeComponent
    {
        private readonly Habbo _player;
        private readonly Dictionary<string, Badge> _badges;

        public BadgeComponent(Habbo player, UserData.UserData data)
        {
            _player = player;
            _badges = new Dictionary<string, Badge>();

            foreach (var badge in data.Badges)
            {
                BadgeDefinition badgeDefinition = null;
                if (!PlusEnvironment.GetGame().GetBadgeManager().TryGetBadge(badge.Code, out badgeDefinition) || badgeDefinition.RequiredRight.Length > 0 && !player.GetPermissions().HasRight(badgeDefinition.RequiredRight))
                    continue;

                if (!_badges.ContainsKey(badge.Code))
                    _badges.Add(badge.Code, badge);
            }     
        }

        public int Count
        {
            get { return _badges.Count; }
        }

        public int EquippedCount
        {
            get
            {
                var i = 0;

                foreach (var badge in _badges.Values)
                {
                    if (badge.Slot <= 0)
                    {
                        continue;
                    }

                    i++;
                }

                return i;
            }
        }

        public ICollection<Badge> GetBadges()
        {
            return _badges.Values;
        }

        public Badge GetBadge(string badge)
        {
            if (_badges.ContainsKey(badge))
                return _badges[badge];

            return null;
        }

        public bool TryGetBadge(string code, out Badge badge)
        {
            return _badges.TryGetValue(code, out badge);
        }

        public bool HasBadge(string badge)
        {
            return _badges.ContainsKey(badge);
        }

        public void GiveBadge(string code, bool inDatabase, GameClient session)
        {
            if (HasBadge(code))
                return;

            BadgeDefinition badge = null;
            if (!PlusEnvironment.GetGame().GetBadgeManager().TryGetBadge(code.ToUpper(), out badge) || badge.RequiredRight.Length > 0 && !session.GetHabbo().GetPermissions().HasRight(badge.RequiredRight))
                return;

            if (inDatabase)
            {
                using var dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor();
                dbClient.SetQuery("REPLACE INTO `user_badges` (`user_id`,`badge_id`,`badge_slot`) VALUES ('" + _player.Id + "', @badge, '" + 0 + "')");
                dbClient.AddParameter("badge", code);
                dbClient.RunQuery();
            }

            _badges.Add(code, new Badge(code, 0));

            if (session != null)
            {
                session.SendPacket(new BadgesComposer(session));
                session.SendPacket(new FurniListNotificationComposer(1, 4));
            }
        }

        public void ResetSlots()
        {
            foreach (var badge in _badges.Values)
            {
                badge.Slot = 0;
            }
        }

        public void RemoveBadge(string badge)
        {
            if (!HasBadge(badge))
            {
                return;
            }

            using (var dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("DELETE FROM user_badges WHERE badge_id = @badge AND user_id = " + _player.Id + " LIMIT 1");
                dbClient.AddParameter("badge", badge);
                dbClient.RunQuery();
            }

            if (_badges.ContainsKey(badge))
                _badges.Remove(badge);
        }
    }
}