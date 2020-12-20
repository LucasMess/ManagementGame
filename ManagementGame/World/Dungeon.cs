using ManagementGame.Objects;
using ManagementGame.Objects.Entities;
using ManagementGame.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagementGame.World
{
    class Dungeon
    {
        public const int MapHeightInChunks = 5;

        public const float GravitationalForce = 10;
        public const float FrictionCoeffAir = .999f;

        public static Random Random = new Random();

        public Player player;

        private Room room;

        public Dungeon()
        {
            room = new Room();

            player = new Player();
            SpawnEntity(player, 0, 0);

            //for (int i = 0; i < 100; i++)
            //{
            //    int x = Random.Next(0, 200);
            //    int y = Random.Next(0, 200);
            //    SpawnEntity(new Ball(x, y), x, y);
            //}
        }

        public void Update(GameTime gameTime, Camera camera)
        {
            room.Update(gameTime);
            
        }
        

        public void Draw(SpriteBatch spriteBatch, Camera camera)
        {                
            room.Draw(spriteBatch, camera);
        }

        public void SpawnEntity(Entity entity, int x, int y)
        {
            room.AddEntity(entity);
        }
    }
}
