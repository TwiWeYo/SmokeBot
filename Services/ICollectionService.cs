using SmokeBot.Model;
using System.Collections.Generic;

namespace SmokeBot.Services
{
    public interface ICollectionService<T> where T : class
    {
        ISerializer<T> Serializer { get; }
        Dictionary<long, Room> Rooms { get; }

        bool Save();

    }
}
