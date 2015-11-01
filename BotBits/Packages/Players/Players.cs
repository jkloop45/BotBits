﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BotBits.Events;
using JetBrains.Annotations;

namespace BotBits
{
    public sealed class Players : EventListenerPackage<Players>, IEnumerable<Player>
    {
        private readonly Dictionary<int, Player> _players = new Dictionary<int, Player>();

        [Obsolete("Invalid to use \"new\" on this class. Use the static .Of(BotBits) method instead.", true)]
        public Players()
        {
        }

        public Player OwnPlayer { get; private set; } = Player.Nobody;

        public Player CrownPlayer { get; private set; } = Player.Nobody;

        public int Count
        {
            get
            {
                lock (this._players)
                    return this._players.Count;
            }
        }

        public Player this[int userId]
        {
            get
            {
                if (userId == Player.Nobody.UserId)
                    return Player.Nobody;

                lock (this._players)
                    return this._players[userId];
            }
        }

        public IEnumerator<Player> GetEnumerator()
        {
            return ((IEnumerable<Player>) this.GetPlayers()).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        [Pure]
        public bool Contains(int userId)
        {
            lock (this._players)
                return this._players.ContainsKey(userId);
        }

        [Pure]
        public Player[] GetPlayers()
        {
            lock (this._players)
                return this._players.Values.Where(p => p.Connected).ToArray();
        }

        [Pure]
        public Player[] FromUsername(string username)
        {
            lock (this._players)
                return this._players.Values
                    .Where(p => p.Username.Equals(username, StringComparison.OrdinalIgnoreCase))
                    .ToArray();
        }

        public bool TryGetPlayer(int userId, out Player player)
        {
            if (userId == Player.Nobody.UserId)
            {
                player = Player.Nobody;
                return true;
            }

            lock (this._players)
                return this._players.TryGetValue(userId, out player);
        }

        internal Player TryAddPlayer(int userId)
        {
            lock (this._players)
            {
                if (this._players.ContainsKey(userId)) return null;
                var player = new Player(this.BotBits, userId);
                this._players.Add(userId, player);
                return player;
            }
        }

        [EventListener]
        private void On(InitEvent e)
        {
            this.OwnPlayer = e.Player;
            this.OwnPlayer.Connected = true;
            this.OwnPlayer.Username = e.Username;
            this.OwnPlayer.ConnectUserId = ConnectionManager.Of(this.BotBits).ConnectUserId;
            this.OwnPlayer.Smiley = e.Smiley;
            this.OwnPlayer.Aura = e.Aura;
            this.OwnPlayer.Badge = e.Badge;
            this.OwnPlayer.ChatColor = e.ChatColor;
            this.OwnPlayer.X = e.SpawnX;
            this.OwnPlayer.Y = e.SpawnY;
            this.OwnPlayer.CrewMember = e.CrewMember;
        }

        [EventListener]
        private void On(JoinEvent e)
        {
            var p = e.Player;
            p.Connected = true;
            p.Username = e.Username;
            p.ConnectUserId = e.ConnectUserId;
            p.Smiley = e.Smiley;
            p.Aura = e.Aura;
            p.Badge = e.Badge;
            p.HasChat = e.HasChat;
            p.GodMode = e.God;
            p.AdminMode = e.Admin;
            p.ModMode = e.Mod;
            p.Friend = e.Friend;
            p.GoldCoins = e.Coins;
            p.BlueCoins = e.BlueCoins;
            p.Deaths = e.Deaths;
            p.X = e.X;
            p.Y = e.Y;
            p.ClubMember = e.ClubMember;
            p.ChatColor = e.ChatColor;
            p.Team = e.Team;
            p.CrewMember = e.CrewMember;
        }

        [EventListener]
        private void On(CrownEvent e)
        {
            this.CrownPlayer = e.Player;
        }

        [EventListener]
        private void On(LeaveEvent e)
        {
            Player leftPlayer;
            lock (this._players)
                if (this._players.TryGetValue(e.Player.UserId, out leftPlayer))
                {
                    this._players.Remove(e.Player.UserId);
                    leftPlayer.Connected = false;
                }
        }

        [EventListener]
        private void On(CoinEvent e)
        {
            var p = e.Player;
            p.GoldCoins = e.GoldCoins;
            p.BlueCoins = e.BlueCoins;

            var x = (int) e.X;
            var y = (int) e.Y;
            var blocks = Blocks.Of(this.BotBits);
            if (!blocks.Area.Contains(new Point(x, y))) return;
            var block = blocks.Foreground[x, y].Block.Id;

            switch (block)
            {
                case Foreground.Coin.Gold:
                    p.AddGoldCoin(new Point(x, y));
                    new GoldCoinEvent(p, e.GoldCoins, x, y)
                        .RaiseIn(this.BotBits);
                    break;
                case Foreground.Coin.Blue:
                    p.AddBlueCoin(new Point(x, y));
                    new BlueCoinEvent(p, e.BlueCoins, x, y)
                        .RaiseIn(this.BotBits);
                    break;
            }
        }

        [EventListener]
        private void On(ForegroundPlaceEvent e)
        {
            if (e.Old.Block.Id == Foreground.Coin.Gold)
            {
                foreach (var player in this)
                {
                    player.RemoveGoldCoin(new Point(e.X, e.Y));
                }
            }
            else if (e.Old.Block.Id == Foreground.Coin.Blue)
            {
                foreach (var player in this)
                {
                    player.RemoveBlueCoin(new Point(e.X, e.Y));
                }
            }
        }

        [EventListener]
        private void On(SmileyEvent e)
        {
            var p = e.Player;
            p.Smiley = e.Smiley;
        }

        [EventListener]
        private void On(MoveEvent e)
        {
            var p = e.Player;
            p.Horizontal = e.Horizontal;
            p.Vertical = e.Vertical;
            p.ModifierX = e.ModifierX;
            p.ModifierY = e.ModifierY;
            p.SpeedX = e.SpeedX;
            p.SpeedY = e.SpeedY;
            p.X = e.X;
            p.Y = e.Y;
            p.SpaceDown = e.SpaceDown;
            p.SpaceJustDown = e.SpaceJustDown;
        }


        [EventListener]
        private void On(RestoreProgressEvent e)
        {
            var p = e.Player;
            p.X = e.X;
            p.Y = e.Y;
            p.SpeedX = e.SpeedX;
            p.SpeedY = e.SpeedY;
            p.GoldCoins = e.GoldCoins;
            p.BlueCoins = e.BlueCoins;
            p.Deaths = e.Deaths;

            foreach (var ps in e.PurpleSwitches)
            {
                p.AddSwitch(ps);
                new PurpleSwitchEvent(p, ps, true)
                    .RaiseIn(this.BotBits);
            }

            foreach (var loc in e.GoldCoinPoints)
            {
                p.AddGoldCoin(loc);
                new GoldCoinEvent(p, p.GoldCoins, loc.X, loc.Y)
                    .RaiseIn(this.BotBits);
            }

            foreach (var loc in e.BlueCoinPoints)
            {
                p.AddBlueCoin(loc);
                new BlueCoinEvent(p, p.BlueCoins, loc.X, loc.Y)
                    .RaiseIn(this.BotBits);
            }
        }

        [EventListener(EventPriority.Low)]
        private void On(GodModeEvent e)
        {
            var p = e.Player;
            p.GodMode = e.God;

            if (!p.ModMode && !p.AdminMode)
            {
                new FlyEvent(p, p.Flying)
                    .RaiseIn(this.BotBits);
            }
        }

        [EventListener(EventPriority.Low)]
        private void On(AdminModeEvent e)
        {
            var p = e.Player;
            p.AdminMode = e.Admin;

            if (!p.ModMode && !p.GodMode)
            {
                new FlyEvent(p, p.Flying)
                    .RaiseIn(this.BotBits);
            }
        }

        [EventListener(EventPriority.Low)]
        private void On(ModModeEvent e)
        {
            var p = e.Player;
            p.ModMode = e.Mod;

            if (!p.GodMode && !p.AdminMode)
            {
                new FlyEvent(p, p.Flying)
                    .RaiseIn(this.BotBits);
            }
        }

        [EventListener]
        private void On(SilverCrownEvent e)
        {
            var p = e.Player;
            p.HasSilverCrown = true;
        }

        [EventListener]
        private void On(EffectEvent e)
        {
            var p = e.Player;

            if (e.Enabled)
            {
                var effect = new ActiveEffect(e.Effect, e.Expires, e.TimeLeft, e.Duration);
                p.AddEffect(effect);
            }
            else
            {
                p.RemoveEffect(e.Effect);
            }
        }

        [EventListener]
        private void On(AuraEvent e)
        {
            var p = e.Player;
            p.Aura = e.Aura;
        }

        [EventListener]
        private void On(TeamEvent e)
        {
            var p = e.Player;
            p.Team = e.Team;
        }

        [EventListener]
        private void On(AllowToggleGodEvent e)
        {
            var p = e.Player;
            p.HasGodRights = e.AllowToggle;
            p.GodMode &= e.AllowToggle;
        }

        [EventListener]
        private void On(EditRightsEvent e)
        {
            var p = e.Player;
            p.HasEditRights = e.AllowEdit;
        }

        [EventListener]
        private void On(PurpleSwitchInitEvent e)
        {
            var p = e.Player;
            foreach (var ps in e.PurpleSwitches)
            {
                p.AddSwitch(ps);
                new PurpleSwitchEvent(p, ps, true)
                    .RaiseIn(this.BotBits);
            }
        }

        [EventListener(EventPriority.Low)]
        private void On(PurpleSwitchUpdateEvent e)
        {
            var p = e.Player;
            var enabled = e.Enabled != 0;
            if (enabled)
                p.AddSwitch(e.SwitchId);
            else
                p.RemoveSwitch(e.SwitchId);

            new PurpleSwitchEvent(p, e.SwitchId, enabled)
                .RaiseIn(this.BotBits);
        }

        [EventListener]
        private void On(TeleportEvent e)
        {
            var p = e.Player;
            p.X = e.X;
            p.Y = e.Y;
        }

        [EventListener(EventPriority.Low)]
        private void On(MultiRespawnEvent e)
        {
            foreach (var tele in e.Data)
            {
                var p = tele.Player;
                var causedByDeath = p.Deaths < tele.Deaths;
                p.X = tele.X;
                p.Y = tele.Y;
                p.Deaths = tele.Deaths;

                if (e.ResetCoins)
                {
                    p.GoldCoins = default(int);
                    p.BlueCoins = default(int);
                    p.ClearGoldCoins();
                    p.ClearBlueCoins();
                }

                new RespawnEvent(p, tele.X, tele.Y, tele.Deaths, e.ResetCoins, causedByDeath)
                    .RaiseIn(this.BotBits);
            }
        }

        [EventListener]
        private void On(MutedEvent e)
        {
            var p = e.Player;
            p.Muted = e.Muted;
        }
    }
}