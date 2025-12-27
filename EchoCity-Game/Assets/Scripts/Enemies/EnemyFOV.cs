
using Unity.VisualScripting;
using UnityEngine;

namespace EchoCity
{
    [System.Serializable]
    public class FOVParams
    {
        public float Range;
        public float Angle;

        public FOVParams(float range, float angle)
        {
            Range = range;
            Angle = angle;
        }

        public FOVParams(FOVParams other)
        {
            Range = other.Range;
            Angle = other.Angle;
        }
    }

    [System.Serializable]
    public struct TargetData
    {
        public Transform Transform;
        public TargetVisibilityEnum VisibilityStatus;
        public float Velocity;
        public Vector3 LastPosition;
        // public Vector3 AcquiredPosition;

        public TargetData(Transform targetTransform, TargetVisibilityEnum visibilityStatus, float velocity)
        {
            Transform = targetTransform;
            VisibilityStatus = visibilityStatus;
            Velocity = velocity;
            LastPosition = targetTransform ? targetTransform.position : Vector3.zero;
            // AcquiredPosition = targetTransform ? targetTransform.position : Vector3.zero;
        }
    }

    [RequireComponent(typeof(SphereCollider))]
    public class EnemyFOV : MonoBehaviour, IFOV
    {
        [SerializeField] private SphereCollider fovCollider;

        [Header("Debug")]
        [SerializeField] private SOEnemyData enemyData;
        [SerializeField] private FOVParams _params;
        private Transform _owner;
        [SerializeField] private TargetData[] _targetsInFOV = new TargetData[4]; //max 4 targets
        [SerializeField] private TargetData _closestTarget;
        [SerializeField] private TargetData _activeTarget;


        public FOVParams Params
        {
            get => _params; private set
            {
                _params = value;
                fovCollider.radius = _params.Range;
            }
        }
        public Transform Owner => _owner;
        public TargetData[] TargetsInFOV => _targetsInFOV;
        public TargetData ClosestTarget => _closestTarget;
        public TargetData ActiveTarget
        {
            get => _activeTarget;
            set => _activeTarget = value;
        }

        void Start()
        {
            Debug.Assert(TryGetComponent(out fovCollider), $"{nameof(fovCollider)} is not assigned on {gameObject.name}");
            fovCollider.isTrigger = true;
            enemyData = null; //deactivate gizmo debug by default
        }

        public void Initialize(FOVParams fovData, Transform owner)
        {
            fovCollider.radius = fovData.Range;
            this._params = fovData;
            this._owner = owner;
            _closestTarget = new TargetData(null, TargetVisibilityEnum.OutOfRange, 0f);
        }

        // check for targets in FOV among those in range
        // the closest visible target is stored in _closestTarget
        // each target's visibility status is updated
        // does not change the ActiveTarget transform!
        // does not add/remove targets from TargetsInFOV
        // does not update the velocity of EVERY target OutOfRange
        public void UpdateTargets()
        {
            if (fovCollider == null || !fovCollider.enabled) return;

            Vector3 currentPos; // fov owner position
            float distMoved; // distance moved since last frame
            Vector3 directionToTarget; // direction from fov owner to target
            float distanceToTarget; // distance from fov owner to target
            float closestDistance; // distance to closest target found so far
            float targetAngle; // angle between fov forward and direction to target


            //update _closestTarget 
            if (_closestTarget.Transform != null)
            {
                currentPos = _closestTarget.Transform.position;
                distMoved = Vector3.Distance(currentPos, _closestTarget.LastPosition);

                // Update velocity and last position
                _closestTarget.Velocity = Time.deltaTime > 0 ? distMoved / Time.deltaTime : 0f;
                _closestTarget.LastPosition = currentPos;


                // update visibility
                if (_closestTarget.VisibilityStatus != TargetVisibilityEnum.OutOfRange)
                {
                    directionToTarget = (_closestTarget.Transform.position - _owner.position).normalized;
                    targetAngle = Vector3.Angle(transform.forward, directionToTarget);
                    if (targetAngle < Params.Angle / 2f && IsTargetVisible(_closestTarget.Transform))
                        _closestTarget.VisibilityStatus = TargetVisibilityEnum.VisibleInFOV;
                    else
                        _closestTarget.VisibilityStatus = TargetVisibilityEnum.InRangeHidden;
                }
            }


            //update ActiveTarget
            if (_activeTarget.Transform != null)
            {
                currentPos = _activeTarget.Transform.position;
                distMoved = Vector3.Distance(currentPos, _activeTarget.LastPosition);

                // Update velocity and last position
                _activeTarget.Velocity = Time.deltaTime > 0 ? distMoved / Time.deltaTime : 0f;
                _activeTarget.LastPosition = currentPos;

                // update visibility
                if (_activeTarget.VisibilityStatus != TargetVisibilityEnum.OutOfRange)
                {
                    directionToTarget = (_activeTarget.Transform.position - _owner.position).normalized;
                    targetAngle = Vector3.Angle(transform.forward, directionToTarget);
                    if (targetAngle < Params.Angle / 2f && IsTargetVisible(_activeTarget.Transform))
                        _activeTarget.VisibilityStatus = TargetVisibilityEnum.VisibleInFOV;
                    else
                        _activeTarget.VisibilityStatus = TargetVisibilityEnum.InRangeHidden;
                }
            }



            // update each target in range
            for (int i = 0; i < _targetsInFOV.Length; i++)
            {
                {
                    if (_targetsInFOV[i].Transform == null) continue;

                    TargetData dataCopy = _targetsInFOV[i];

                    currentPos = dataCopy.Transform.position;
                    distMoved = Vector3.Distance(currentPos, dataCopy.LastPosition);

                    // Update velocity and last position
                    _targetsInFOV[i].Velocity = Time.deltaTime > 0 ? distMoved / Time.deltaTime : 0f;
                    _targetsInFOV[i].LastPosition = currentPos;

                    // Update visibility
                    directionToTarget = (dataCopy.Transform.position - _owner.position).normalized;
                    targetAngle = Vector3.Angle(transform.forward, directionToTarget);
                    if (targetAngle < Params.Angle / 2f && IsTargetVisible(dataCopy.Transform))
                    {
                        _targetsInFOV[i].VisibilityStatus = TargetVisibilityEnum.VisibleInFOV;
                        distanceToTarget = Vector3.Distance(_owner.position, dataCopy.Transform.position);

                        //NOTE reassign _closestTarget only if VisibleInFOV
                        if (_closestTarget.Transform == null)
                            closestDistance = float.MaxValue;
                        else if (_closestTarget.VisibilityStatus == TargetVisibilityEnum.VisibleInFOV)
                            closestDistance = Vector3.Distance(_owner.position, _closestTarget.Transform.position);
                        else closestDistance = float.MaxValue;

                        if (distanceToTarget < closestDistance)
                            _closestTarget = new TargetData(dataCopy.Transform, TargetVisibilityEnum.VisibleInFOV, dataCopy.Velocity);
                    }
                    else
                        _targetsInFOV[i].VisibilityStatus = TargetVisibilityEnum.InRangeHidden;

                }
            }
        }


        private bool IsTargetVisible(Transform target)
        {
            Vector3 startPoint = transform.position;
            // player height offset
            Vector3 endPoint = target.position + Vector3.up * 1f;

            Vector3 direction = (endPoint - startPoint).normalized;
            float distance = Vector3.Distance(startPoint, endPoint);

            int layerMask = ~(1 << LayerMask.NameToLayer("Enemy"));

            // Use the effective visual range as the Raycast limit
            if (Physics.Raycast(startPoint, direction, out RaycastHit hit, Params.Range + 0.5f, layerMask))
            {
                if (hit.transform == target || hit.transform.IsChildOf(target))
                {
                    return true;
                }
            }
            return false;
        }

        private void OnTriggerEnter(Collider other)
        {

            if (other.gameObject.CompareTag("Player"))
            {
                bool alreadyPresent = false;
                // Add to targetsInRange if not already present
                for (int i = 0; i < _targetsInFOV.Length; i++)
                {
                    if (_targetsInFOV[i].Transform == other.transform)
                    {
                        alreadyPresent = true;
                        break;
                    }
                }
                if (!alreadyPresent)
                {
                    for (int i = 0; i < _targetsInFOV.Length; i++)
                    {
                        if (_targetsInFOV[i].Transform == null)
                        {
                            _targetsInFOV[i] = new TargetData(other.transform, TargetVisibilityEnum.InRangeHidden, 0f);
                            break;
                        }
                    }
                }
                if (other.transform == ActiveTarget.Transform)
                    _activeTarget.VisibilityStatus = TargetVisibilityEnum.InRangeHidden;
                if (other.transform == ClosestTarget.Transform)
                    _closestTarget.VisibilityStatus = TargetVisibilityEnum.InRangeHidden;
            }


        }

        private void OnTriggerExit(Collider other)
        {
            // Remove from targetsInRange
            if (other.gameObject.CompareTag("Player"))
            {
                for (int i = 0; i < _targetsInFOV.Length; i++)
                {
                    if (_targetsInFOV[i].Transform == other.transform)
                    {
                        _targetsInFOV[i] = new TargetData(null, TargetVisibilityEnum.OutOfRange, 0f);
                        break;
                    }
                }
                if (other.transform == ActiveTarget.Transform)
                    _activeTarget.VisibilityStatus = TargetVisibilityEnum.OutOfRange;
                if (other.transform == ClosestTarget.Transform)
                    _closestTarget.VisibilityStatus = TargetVisibilityEnum.OutOfRange;
            }
        }

        private void OnDrawGizmos()
        {
            float range; float angle;

            if (enemyData != null)
            {
                range = enemyData.FOVData.Range;
                angle = enemyData.FOVData.Angle;
            }
            else
            {
                range = Params.Range;
                angle = Params.Angle;
            }
            // range radius
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, range);
            // cone boundaries directions
            Vector3 leftBoundary = Quaternion.AngleAxis(-angle / 2f, Vector3.up) * transform.forward;
            Vector3 rightBoundary = Quaternion.AngleAxis(angle / 2f, Vector3.up) * transform.forward;
            // color for target line
            Gizmos.color = ActiveTarget.Transform ? Color.red : Color.blue;
            // draw boundaries
            Gizmos.DrawRay(transform.position, leftBoundary * range);
            Gizmos.DrawRay(transform.position, rightBoundary * range);
            // draw line to active target
            if (ActiveTarget.Transform != null)
                Gizmos.DrawLine(transform.position, ActiveTarget.Transform.position);
        }

        void OnValidate()
        {
            float range; float angle;

            if (enemyData != null)
            {
                range = enemyData.FOVData.Range;
                angle = enemyData.FOVData.Angle;
            }
            else
            {
                range = Params.Range;
                angle = Params.Angle;
            }
            fovCollider.radius = range;
        }
    }
}