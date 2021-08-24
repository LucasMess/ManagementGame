using ManagementGame.Objects;
using ManagementGame.Objects.Entities;
using ManagementGame.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagementGame.Tasks
{
    class InteractTask : ActorTask
    {

        MoveTask moveTask;
        Entity interactable;

        public InteractTask(Entity interactable)
        {
            this.interactable = interactable;
        }

        protected override void OnContinue(Actor actor, GameWorld gameWorld)
        {
            if (!IsFinished(moveTask, actor, gameWorld))
                return;

            if (interactable is Interactable)
            {
                ((Interactable)interactable).Interact(actor);
            }

            Finish();
        }

        protected override void OnStart(Actor actor, GameWorld gameWorld)
        {
            moveTask = new MoveTask(interactable.Position);
        }
    }
}
