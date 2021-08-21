using ManagementGame.Objects;
using ManagementGame.Objects.Entities;
using ManagementGame.World;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagementGame.Tasks
{
    class MoveTask : ActorTask
    {
        float TargetRadius = (float)Math.Pow((float)Tile.GridSize, 2f);
        const int MaxForce = 10;

        Vector2 destination;
        List<Vector2> path;

        int i = 0;

        int currIdx = 0;

        public MoveTask(Vector2 destination)
        {
            this.destination = destination;
        }

        protected override void OnStart(Actor actor, GameWorld gameWorld)
        {
            path = gameWorld.Pathfind(actor.Position, destination);
            if (path.Count == 0)
            {
                Console.WriteLine("No path found");
                Finish();
            }
        }

        protected override void OnContinue(Actor actor, GameWorld gameWorld)
        {
            if (currIdx < path.Count)
            {
                Vector2 nextPoint = path[currIdx];
                Vector2 distance = nextPoint - actor.Position;
                if (distance.LengthSquared() < TargetRadius)
                {
                    Console.WriteLine("Arrived on node");
                    currIdx++;
                }
                else
                {
                    Vector2 force;

                    if (distance.LengthSquared() < Math.Pow(MaxForce, 2f))
                    {
                        force = distance;
                    }
                    else
                    {
                        Vector2 dir = Vector2.Normalize(distance);
                        force = dir * MaxForce;
                    }
                    Console.WriteLine(force);
                    actor.ApplyForce(force);

                }
            } else
            {
                Console.WriteLine("Finished");
                Finish();
            }


            if (i >= path.Count)
            {
                i = 0;
            }
            if (i < path.Count)
            {
                var p = path[i];
                Tile tile = gameWorld.GetTileAt(p.X, p.Y);
                tile.isPath = !tile.isPath;
                i++;
            }
        }
    }
}
