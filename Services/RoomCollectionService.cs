using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SmokeBot.Configuration;
using SmokeBot.Model;
using System;
using System.Collections.Generic;

namespace SmokeBot.Services
{
    public class RoomCollectionService : ICollectionService<Room>
    {
        private readonly ILogger<RoomCollectionService> logger;
        public ISerializer<Room> Serializer { get; }

        public Dictionary<long, Room> Rooms { get; }

        public RoomCollectionService(ILogger<RoomCollectionService> logger, ISerializer<Room> serializer, IOptions<AppSettings> options)
        {
            this.logger = logger;
            Serializer = serializer;
            Serializer.Path = options.Value.CollectionPath;
            try
            {
                Rooms = Serializer.Deserialize() ?? new Dictionary<long, Room>();
                logger.LogDebug("DB set succesfully");
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                Rooms = new Dictionary<long, Room>();
            }
        }

        public bool Save()
        {
            try
            {
                var output = Serializer.Serialize(Rooms);
                logger.LogDebug("Save succesfull");
                return output;
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return false;
            }
        }
    }
}
