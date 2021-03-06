using System.Collections.Generic;
using PlayerIOClient;

namespace BotBits.Events
{
    /// <summary>
    ///     Occurs when mutliple players are teleported. This event gets raised for respawns of any kind, including death.
    /// </summary>
    /// <seealso cref="ReceiveEvent{T}" />
    [ReceiveEvent("tele")]
    public sealed class MultiRespawnEvent : ReceiveEvent<MultiRespawnEvent>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="MultiRespawnEvent" /> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="client"></param>
        internal MultiRespawnEvent(BotBitsClient client, Message message)
            : base(client, message)
        {
            this.Data = new List<RespawnData>();

            this.ResetPlayers = message.GetBoolean(0);
            this.ResetLevel = message.GetBoolean(1);

            for (uint i = 2; i <= message.Count - 1u; i += 4)
            {
                var userId = message.GetInteger(i);
                if (!Players.Of(client).Contains(userId)) continue;
                var player = Players.Of(client)[userId];
                var x = message.GetInteger(i + 1u);
                var y = message.GetInteger(i + 2u);
                var deaths = message.GetInteger(i + 3u);

                this.Data.Add(new RespawnData(player, x, y, deaths));
            }
        }

        public bool ResetLevel { get; set; }

        /// <summary>
        ///     Gets or sets the coordinates.
        /// </summary>
        /// <value>The coordinates.</value>
        public List<RespawnData> Data { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the players need to be reset.
        /// </summary>
        /// <value><c>true</c> if the players need to be reset; otherwise, <c>false</c>.</value>
        public bool ResetPlayers { get; set; }
    }
}