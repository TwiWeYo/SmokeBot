using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SmokeBot.Model;
using System.Collections.Generic;
using System.IO;

namespace SmokeBot.Services
{
    public class JsonSerializer : ISerializer<Room>
    {
        private Newtonsoft.Json.JsonSerializer serializer;
        private readonly ILogger<JsonSerializer> logger;
        public string Path { get; set; }

        public JsonSerializer(ILogger<JsonSerializer> logger)
        {
            this.logger = logger;
            serializer = new Newtonsoft.Json.JsonSerializer();
        }

        public Dictionary<long, Room> Deserialize()
        {
            if (!File.Exists(Path))
            {
                logger.LogWarning("File does not exist");
                return null;
            }

            return JsonConvert.DeserializeObject<Dictionary<long, Room>>(File.ReadAllText(Path));
        }

        public bool Serialize(Dictionary<long, Room> dictionary)
        {
            var dirPath = System.IO.Path.GetDirectoryName(Path);
            if (!Directory.Exists(dirPath))
                Directory.CreateDirectory(dirPath);

            using (StreamWriter sw = new StreamWriter(Path))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                serializer.Serialize(writer, dictionary);
            }
            return true;
        }
    }
}
