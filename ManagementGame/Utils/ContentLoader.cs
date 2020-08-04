﻿using ManagementGame.Objects;
using ManagementGame.Objects.Tiles;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagementGame.Utils
{
    static class ContentLoader
    {
        static Dictionary<string, Texture2D> textures;
        static Dictionary<string, Effect> shaders;

        public static void LoadTileTextures(ContentManager content)
        {
            textures = new Dictionary<string, Texture2D>();

            string path = Path.Combine(FileLoader.DistDirectory, "Data", "tiles.yaml");
            var tiles = FileLoader.LoadYaml<TileProperties[]>(path);
            Tile.Properties = tiles;

            foreach (var tile in tiles)
            {
                Console.WriteLine($"Loading {tile.Name}");
                textures.Add(tile.Name, content.Load<Texture2D>($"Art/{tile.Name}"));
            }
        }

        public static void LoadShaders(ContentManager content)
        {
            shaders = new Dictionary<string, Effect>();
            shaders.Add("tiling", content.Load<Effect>("Shaders/tiling"));
        }

        public static Effect GetShader(string name)
        {
            return shaders[name];
        }

        public static Texture2D GetTexture2D(string name)
        {
            return textures[name];
        }
    }
}
