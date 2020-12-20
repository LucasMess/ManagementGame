using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManagementGame.World;
using Newtonsoft.Json;
using YamlDotNet.Serialization;

namespace ManagementGame.Utils
{
    static class FileLoader
    {
        /// <summary>
        /// The file path of the main directory.
        /// </summary>
        public static string AppDataDirectory;

        /// <summary>
        /// The file path of the folder containing world data.
        /// </summary>
        public static string WorldsDirectory;

        /// <summary>
        /// The file path of the distributable folder.
        /// </summary>
        public static string DistDirectory;


        /// <summary>
        /// Checks to see if the directory exists upon creation, and if it does not, it will create it.
        /// </summary>
        public static void Initialize()
        {
            // The folder for the roaming current user.
            string folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string specificFolder = Path.Combine(folder, "ManagementGame");

            // Check if main Game directory folder exists and if not, create it.
            if (!Directory.Exists(specificFolder))
                Directory.CreateDirectory(specificFolder);
            AppDataDirectory = specificFolder;

            // Check if Levels folder exists else create it.
            specificFolder = Path.Combine(AppDataDirectory, "worlds");
            if (!Directory.Exists(specificFolder))
                Directory.CreateDirectory(specificFolder);
            WorldsDirectory = specificFolder;

            DistDirectory = Environment.CurrentDirectory;
        }

        //public static WorldData LoadWorld(string worldName)
        //{
        //    string path = Path.Combine(WorldsDirectory, worldName, "world.json");
        //    return LoadJson<WorldData>(path);
        //}

        //public static ChunkData LoadChunk(string worldName, int x, int y)
        //{
        //    string path = Path.Combine(WorldsDirectory, worldName, "chunks",  $"({x},{y}).json");
        //    return LoadJson<ChunkData>(path);
        //}

        public static void SaveWorld(string worldName, WorldData worldData)
        {
            string path = Path.Combine(WorldsDirectory, worldName, "world.json");
            SaveJson(path, worldData);
        }

        //public static void SaveChunk(string worldName, Room chunk)
        //{
        //    string path = Path.Combine(WorldsDirectory, worldName, "chunks", $"({chunk.ChunkX},{chunk.ChunkY}).json");
        //    SaveJson(path, chunk);
        //}

        public static T LoadJson<T>(string filePath) {
            string text = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<T>(text);
        }

        public static void SaveJson(string filePath, object data)
        {
            string text = JsonConvert.SerializeObject(data);
            File.WriteAllText(filePath, text);
        }

        public static T LoadYaml<T>(string filePath)
        {
            string text = File.ReadAllText(filePath);
            var deserializer = new Deserializer();
            return deserializer.Deserialize<T>(text);
        }
    }
}
