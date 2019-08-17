using GameCreator.Characters;
using UnityEngine;
using UnityEngine.AI;

namespace BehaviorDesigner.Runtime.Tasks.DGD
{
    [TaskDescription("Move character to a specific location, marker or gameobject. If a Navigation marker is on the target, it will use that information")]
    [TaskCategory("Game Creator")]
    public class GameCreatorMoveTo : Action
    {
        public SharedGameObject character;
        public SharedBool useNaviationMarker;

        public SharedGameObject targetMoveTo;

        public SharedVector3 targetLocation;

        public SharedFloat stopThreshold = 0.0f;

        private Character _character;
        private NavigationMarker _marker;
        private Vector3 cPosition = Vector3.zero;
        private ILocomotionSystem.TargetRotation cRotation = null;
        private float cStopThresh = 0f;
        private bool hasArrived = false;

        public override void OnStart()
        {
            hasArrived = false;
            _character = character.Value.GetComponent<Character>();
            if (character == null)
            {
                Debug.LogError("No character component found for 'Game Creator Move To' in behavior designer");
            }

            MoveTo();
        }

        private void MoveTo()
        {
            if (useNaviationMarker.Value)
            {
                _marker = targetMoveTo.Value.GetComponent<NavigationMarker>();
                if (_marker != null)
                {
                    GetMarkerInformation();
                }
                else
                {
                    Debug.LogError("No destination marker component found for 'Game Creator Move To' in BD");
                }
            }

            else
            {
                GetDesinationInformation();
            }

            SetDesitination();
        }


        /// <summary>
        /// Get destination information if not using a marker
        /// </summary>
        private void GetDesinationInformation()
        {
            if (targetMoveTo.IsNone || targetMoveTo.Value == null)
            {
                cPosition = targetLocation.Value;
                Debug.Log("Using vector 3");
            }
            else
            {
                cPosition = targetMoveTo.Value.transform.position;
                Debug.Log("Using transform");
            }

            cStopThresh = stopThreshold.Value;
            cRotation = null;
        }

        /// <summary>
        /// Set character destination
        /// </summary>
        private void SetDesitination()
        {
            _character.characterLocomotion.SetTarget(cPosition, cRotation, cStopThresh, this.CharacterArrivedCallback);
        }

        /// <summary>
        /// Call back for arrival completed
        /// </summary>
        private void CharacterArrivedCallback()
        {
            hasArrived = true;
        }

        /// <summary>
        /// Get marker information if found
        /// </summary>
        private void GetMarkerInformation()
        {
            Debug.Log("Using marker");
            cPosition = _marker.transform.position;
            cRotation = new ILocomotionSystem.TargetRotation(true, _marker.transform.forward);
            cStopThresh = _marker.stopThreshold;
            cStopThresh = Mathf.Max(cStopThresh, stopThreshold.Value);
        }

        public override TaskStatus OnUpdate()
        {
            if (hasArrived)
            {
                return TaskStatus.Success;
            }

            return TaskStatus.Running;
        }

        public override void OnReset()
        {
            targetMoveTo = null;
            stopThreshold = 1f;
            useNaviationMarker = false;
            targetLocation = null;
            character = null;
        }
    }
}