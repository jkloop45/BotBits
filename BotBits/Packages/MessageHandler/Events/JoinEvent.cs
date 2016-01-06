using PlayerIOClient;

namespace BotBits.Events
{
    /// <summary>
    ///     Occurs when someone joins world.
    /// </summary>
    /// <seealso cref="PlayerEvent{T}" />
    [ReceiveEvent("add")]
    public sealed class JoinEvent : PlayerEvent<JoinEvent>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="JoinEvent" /> class.
        /// </summary>
        /// <param name="message">The EE message.</param>
        /// <param name="client"></param>
        internal JoinEvent(BotBitsClient client, Message message)
            : base(client, message, create: true)
        {
            Username = message.GetString(1);
            ConnectUserId = message.GetString(2);
            Smiley = (Smiley) message.GetInteger(3);
            X = message.GetDouble(4);
            Y = message.GetDouble(5);
            God = message.GetBoolean(6);
            Admin = message.GetBoolean(7);
            HasChat = message.GetBoolean(8);
            Coins = message.GetInteger(9);
            BlueCoins = message.GetInteger(10);
            Deaths = message.GetInteger(11);
            Friend = message.GetBoolean(12);
            ClubMember = message.GetBoolean(13);
            Mod = message.GetBoolean(14);
            Team = (Team) message.GetInt(15);
            AuraShape = (AuraShape) message.GetInt(16);
            ChatColor = message.GetUInt(17);
            Badge = message.GetBadge(18);
            CrewMember = message.GetBoolean(19);
        }

        /// <summary>
        ///     Gets or sets the amount of deaths.
        /// </summary>
        /// <value>The deaths.</value>
        public int Deaths { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether player is crew member.
        /// </summary>
        /// <value><c>true</c> if player is crew member; otherwise, <c>false</c>.</value>
        public bool CrewMember { get; set; }

        /// <summary>
        ///     Gets or sets the badge.
        /// </summary>
        /// <value>The badge.</value>
        public Badge Badge { get; set; }

        /// <summary>
        ///     Gets or sets the connect user identifier.
        /// </summary>
        /// <value>The connect user identifier.</value>
        public string ConnectUserId { get; set; }

        /// <summary>
        ///     Gets or sets the aura.
        /// </summary>
        /// <value>
        ///     The aura.
        /// </value>
        public AuraShape AuraShape { get; set; }

        /// <summary>
        ///     Gets or sets the team.
        /// </summary>
        /// <value>
        ///     The team.
        /// </value>
        public Team Team { get; set; }

        /// <summary>
        ///     Gets or sets the color of the chat.
        /// </summary>
        /// <value>
        ///     The color of the chat.
        /// </value>
        public uint ChatColor { get; set; }

        /// <summary>
        ///     Gets or sets whether the user is in admin mode or not.
        /// </summary>
        /// <value><c>true</c> if the player has activated admin mode; otherwise, <c>false</c>.</value>
        public bool Admin { get; set; }

        /// <summary>
        ///     Gets or sets the amount of yellow coins the player has.
        /// </summary>
        /// <value>The yellow coins.</value>
        public int Coins { get; set; }

        /// <summary>
        ///     Gets or sets the amount of blue coins the player has.
        /// </summary>
        /// <value>The blue coins.</value>
        public int BlueCoins { get; set; }

        /// <summary>
        ///     Gets or sets the smiley the player has.
        /// </summary>
        /// <value>The face.</value>
        public Smiley Smiley { get; set; }

        /// <summary>
        ///     Gets or sets whether this player may chat using the free-form chat box.
        /// </summary>
        /// <value><c>true</c> if this player has chat; otherwise, <c>false</c>.</value>
        public bool HasChat { get; set; }

        /// <summary>
        ///     Gets or sets whether this player is a club member.
        /// </summary>
        /// <value><c>true</c> if this player is a club member; otherwise, <c>false</c>.</value>
        public bool ClubMember { get; set; }

        /// <summary>
        ///     Gets or sets whether this player has activated god mode.
        /// </summary>
        /// <value><c>true</c> if this player is in god mode; otherwise, <c>false</c>.</value>
        public bool God { get; set; }

        /// <summary>
        ///     Gets or sets whether this player is a moderator.
        /// </summary>
        /// <value><c>true</c> if this player is a moderator; otherwise, <c>false</c>.</value>
        public bool Mod { get; set; }

        /// <summary>
        ///     Gets or sets whether this player is my friend or not.
        /// </summary>
        /// <value><c>true</c> if this player is my friend; otherwise, <c>false</c>.</value>
        public bool Friend { get; set; }

        /// <summary>
        ///     Gets or sets whether the player has toggled a purple switch.
        /// </summary>
        /// <value><c>true</c> if the player has toggled a purple switch; otherwise, <c>false</c>.</value>
        public bool PurpleSwitch { get; set; }

        /// <summary>
        ///     Gets or sets the username of the player.
        /// </summary>
        /// <value>The username.</value>
        public string Username { get; set; }

        /// <summary>
        ///     Gets or sets the x coordinate of the player.
        /// </summary>
        /// <value>The user position x.</value>
        public double X { get; set; }

        /// <summary>
        ///     Gets or sets the y coordinate of the player.
        /// </summary>
        /// <value>The user position y.</value>
        public double Y { get; set; }

        /// <summary>
        ///     Gets the block x.
        /// </summary>
        /// <value>The block x.</value>
        public int BlockX
        {
            get { return WorldUtils.PosToBlock(X); }
        }

        /// <summary>
        ///     Gets the block y.
        /// </summary>
        /// <value>The block y.</value>
        public int BlockY
        {
            get { return WorldUtils.PosToBlock(Y); }
        }
    }
}