using System.Collections.Generic;

namespace SmokeBot.Services
{
    public interface ISerializer<T>
    {
        string Path { get; set; }
        Dictionary<long, T> Deserialize();
        bool Serialize(Dictionary<long, T> dictionary);
    }
}