using GameCreator.Characters;
using UnityEngine;
using UnityEngine.AI;

namespace BehaviorDesigner.Runtime.Tasks.DGD
{
    [TaskDescription("Stop character movement")]
    [TaskCategory("Game Creator")]
    public class GameCreatorMovementStop : Action
    {
        public SharedGameObject character;
        private Character _character;

        public override void OnStart()
        {
            _character = character.Value.GetComponent<Character>();
            if (character == null)
            {
                Debug.LogError("No character component found for 'Game Creator Stop' in behavior designer");
            }
        }

        public override TaskStatus OnUpdate()
        {
            _character.characterLocomotion.Stop();
            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            character = null;
        }
    }
}