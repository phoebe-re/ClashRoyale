namespace ClashRoyale.Database
{
    using ClashRoyale.Database.Models;

    using MongoDB.Driver;

    public static class GameDb
    {
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="GameDb"/> has been already initialized.
        /// </summary>
        public static bool Initialized
        {
            get;
            set;
        }

        public static IMongoCollection<PlayerDb> Players;
        public static IMongoCollection<ClanDb> Clans;
        public static IMongoCollection<BattleDb> Battles;

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        public static void Initialize()
        {
            if (GameDb.Initialized)
            {
                return;
            }

            var MongoClient = new MongoClient("mongodb://localhost:27017");
            var MongoDb = MongoClient.GetDatabase("ClashRoyale");

            Logging.Info(typeof(GameDb), "GameDb is connected to " + MongoClient.Settings.Server.Host + ".");

            // GetCollection never returns null in modern driver — removed null checks
            GameDb.Players = MongoDb.GetCollection<PlayerDb>("Players");
            GameDb.Clans = MongoDb.GetCollection<ClanDb>("Clans");
            GameDb.Battles = MongoDb.GetCollection<BattleDb>("Battles");

            // Players index
            GameDb.Players.Indexes.CreateOne(
                new CreateIndexModel<PlayerDb>(
                    Builders<PlayerDb>.IndexKeys
                        .Ascending(T => T.HighId)
                        .Descending(T => T.LowId),
                    new CreateIndexOptions
                    {
                        Name = "entityIds",
                        Background = true
                    }
                )
            );

            // Clans index
            GameDb.Clans.Indexes.CreateOne(
                new CreateIndexModel<ClanDb>(
                    Builders<ClanDb>.IndexKeys
                        .Ascending(T => T.HighId)
                        .Descending(T => T.LowId),
                    new CreateIndexOptions
                    {
                        Name = "entityIds",
                        Background = true
                    }
                )
            );

            // Battles index
            GameDb.Battles.Indexes.CreateOne(
                new CreateIndexModel<BattleDb>(
                    Builders<BattleDb>.IndexKeys
                        .Ascending(T => T.HighId)
                        .Descending(T => T.LowId),
                    new CreateIndexOptions
                    {
                        Name = "entityIds",
                        Background = true
                    }
                )
            );

            GameDb.Initialized = true;
        }

        /// <summary>
        /// Gets the seed for the Players collection.
        /// </summary>
        public static int GetPlayersSeed()
        {
            return GameDb.Players
                .Find(T => T.HighId == Config.ServerId)
                .Sort(Builders<PlayerDb>.Sort.Descending(T => T.LowId))
                .Limit(1)
                .SingleOrDefault()?.LowId ?? 0;
        }

        /// <summary>
        /// Gets the seed for the Clans collection.
        /// </summary>
        public static int GetClansSeed()
        {
            return GameDb.Clans
                .Find(T => T.HighId == Config.ServerId)
                .Sort(Builders<ClanDb>.Sort.Descending(T => T.LowId))
                .Limit(1)
                .SingleOrDefault()?.LowId ?? 0;
        }

        /// <summary>
        /// Gets the seed for the Battles collection.
        /// </summary>
        public static int GetBattlesSeed()
        {
            return GameDb.Battles
                .Find(T => T.HighId == Config.ServerId)
                .Sort(Builders<BattleDb>.Sort.Descending(T => T.LowId))
                .Limit(1)
                .SingleOrDefault()?.LowId ?? 0;
        }
    }
}