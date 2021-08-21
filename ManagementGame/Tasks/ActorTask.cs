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
    /// <summary>
    /// A task that can be done by an actor.
    /// </summary>
    abstract class ActorTask
    {
   
        public TaskState State = TaskState.Waiting;

        public void Start(Actor actor, GameWorld gameWorld)
        {
            OnStart(actor, gameWorld);
            
            State = TaskState.Started;
        }

        public void Continue(Actor actor, GameWorld gameWorld)
        {
            OnContinue(actor, gameWorld);
        }

        public void Finish()
        {
            State = TaskState.Finished;
        }

        protected abstract void OnStart(Actor actor, GameWorld gameWorld);
        protected abstract void OnContinue(Actor actor, GameWorld gameWorld);

        
    }

    public enum TaskState
    {
        Waiting,
        Started,
        Finished,
    }

}
