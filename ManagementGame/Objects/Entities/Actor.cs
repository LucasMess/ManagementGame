using ManagementGame.Tasks;
using ManagementGame.Utils;
using ManagementGame.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagementGame.Objects.Entities
{
    class Actor : Entity
    {

        private Queue<ActorTask> taskQueue = new Queue<ActorTask>();
        private ActorTask currentTask = null;
        private GameWorld gameWorld;

        public Actor(GameWorld gameWorld)
        {
            this.gameWorld = gameWorld;
            Texture = ContentLoader.GetTexture2D("Grass");
            CanCollideWithTerrain = true;
            AffectedByGravity = true;
            DrawRectangleSize = new Point(Tile.DrawSize, Tile.DrawSize);
            CollisionRectangleSize = new Point(Tile.GridSize, Tile.GridSize);
            Elasticity = .8f;
        }

        public override void Update(GameTime gameTime)
        {
            if (currentTask == null && taskQueue.Count != 0)
            {
                currentTask = taskQueue.Dequeue();
            }

            if (currentTask != null)
            {
                switch (currentTask.State)
                {
                    case TaskState.Waiting:
                        currentTask.Start(this, gameWorld);
                        break;
                    case TaskState.Started:
                        currentTask.Continue(this, gameWorld);
                        break;
                    case TaskState.Finished:
                        currentTask = null;
                        break;
                    default:
                        break;
                }
            }

            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, DrawRectangle, Color.White);
            base.Draw(spriteBatch);
        }


        public void GiveTask(ActorTask task)
        {
            taskQueue.Enqueue(task);
        }
    }
}
