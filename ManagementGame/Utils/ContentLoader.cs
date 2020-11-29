using ManagementGame.Objects;
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
        static Dictionary<string, SpriteFont> fonts;

        public static void LoadTileTextures(ContentManager content)
        {
            textures = new Dictionary<string, Texture2D>();

            string path = Path.Combine(FileLoader.DistDirectory, "Data", "tiles.yaml");
            var tiles = FileLoader.LoadYaml<TileProperties[]>(path);
            Tile.Properties = tiles.OrderBy(x => x.Id).ToArray();

            foreach (var tile in tiles)
            {
                try
                {
                    Console.WriteLine($"Loading {tile.Name}");
                    textures.Add(tile.Name, content.Load<Texture2D>($"Art/{tile.Name}"));
                }
                catch (ContentLoadException e)
                {
                    Console.WriteLine($"Could not find texture for {tile.Name}");
                }
            }
        }

        public static void LoadShaders(ContentManager content)
        {
            shaders = new Dictionary<string, Effect>();
            shaders.Add("tiling", content.Load<Effect>("Shaders/tiling"));
        }

        public static void LoadFonts(ContentManager content)
        {
            fonts = new Dictionary<string, SpriteFont>();
            fonts.Add("x32", content.Load<SpriteFont>("Fonts/x32"));
        }

        public static Effect GetShader(string name)
        {
            return shaders[name];
        }

        public static Texture2D GetTexture2D(string name)
        {
            return textures[name];
        }
        public static SpriteFont GetFont(string name)
        {
            return fonts[name];
        }

        public static Texture2D DebugTexture => textures["Blank"];

    }
}
