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

        public bool IsFinished(ActorTask task, Actor actor, GameWorld gameWorld)
        {
            switch (task.State)
            {
                case TaskState.Waiting:
                    task.Start(actor, gameWorld);
                    break;
                case TaskState.Started:
                    task.Continue(actor, gameWorld);
                    break;
                case TaskState.Finished:
                    return true;
                default:
                    break;
            }
            return false;
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
