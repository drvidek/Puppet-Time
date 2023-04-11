using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKLimb : MonoBehaviour
{
    [SerializeField] private int _chainLength = 2;
    [SerializeField] private Transform _follow, _pole;

    [SerializeField] private int _iterations;

    [SerializeField] private float _snappingDist = 0.001f;

    [Range(0, 1)]
    [SerializeField] private float _snapBackStrength;

    [SerializeField] private LineRenderer _lineRenderer;

    [SerializeField] private Rigidbody[] _rigidbodies;

    protected float[] _bonesLength;
    protected float _completeLength;
    protected Transform[] _joints;
    protected Vector3[] _positions;
    protected Vector3[] _startDirectionSucc;
    protected Quaternion[] _startRotationBone;
    protected Quaternion _startRotationTarget;
    protected Quaternion _startRotationRoot;

    private bool _isRagdoll;

    public bool IsRagdoll => _isRagdoll;

    public float LimbLength => _completeLength;
    public Transform Follow => _follow;

    public Transform[] Joints => _joints;

    private void Awake()
    {
        //_lineRenderer.GetComponent<LineRenderer>();
        Initialise();
    }

    void Initialise()
    {
        //set the size of the arrways based on our IK rig size
        _joints = new Transform[_chainLength + 1];
        _positions = new Vector3[_chainLength + 1];
        _bonesLength = new float[_chainLength];
        _startDirectionSucc = new Vector3[_chainLength + 1];
        _startRotationBone = new Quaternion[_chainLength + 1];
        _startRotationTarget = _follow.rotation;

        _rigidbodies = new Rigidbody[_chainLength];

        _completeLength = 0;

        if (_lineRenderer != null)
            _lineRenderer.positionCount = _chainLength + 1;


        //get the tip bone
        Transform currentBone = transform;
        //working our way from the tip to the root...
        for (int i = _joints.Length - 1; i >= 0; i--)
        {
            //store the current bone
            _joints[i] = currentBone;
            _startRotationBone[i] = currentBone.rotation;

            //if this is our tip, it has no bone
            if (i == _joints.Length - 1)
            {
                //point towards the target
                _startDirectionSucc[i] = _follow.position - currentBone.position;
            }
            else
            {
                //point towards the next joint
                _startDirectionSucc[i] = _joints[i + 1].position - currentBone.position;
                //set the length of the bone from this joint to the previous joint
                _bonesLength[i] = (_joints[i + 1].position - currentBone.position).magnitude;
                //and add that length to the total length of this rig
                _completeLength += _bonesLength[i];

                _rigidbodies[i] = currentBone.GetComponent<Rigidbody>();

            }

            currentBone = currentBone.parent;
        }

    }

    private void LateUpdate()
    {
        ResolveIK();
        HandleLineRenderer();
    }

    void ResolveIK()
    {
        //if we don't have a target, don't move
        if (_follow == null)
            return;

        //if our bone rig stops matching the chain length, re-initialise the rig
        if (_bonesLength.Length != _chainLength)
            Initialise();

        if (!PlayerController.main.IsHeld)
        {
            if (!_isRagdoll)
                ToggleKinematicLimb(false);

            return;
        }

        //store our current jount positions
        for (int i = 0; i < _joints.Length; i++)
        {
            _positions[i] = _joints[i].position;
        }

        var rootRot = (_joints[0].parent != null ? _joints[0].parent.rotation : Quaternion.identity);
        var rootRotDif = rootRot * Quaternion.Inverse(_startRotationRoot);

        //if the target is too far away (based on the total legnth of the rig and the distance of the target from the root joint)
        if ((_follow.position - _joints[0].position).sqrMagnitude >= _completeLength * _completeLength)
        {
            //if we're really too far, make the limb ragdoll
            if ((_follow.position - _joints[0].position).magnitude >= _completeLength * 1.1f)
            {
                if (!_isRagdoll)
                    ToggleKinematicLimb(false);
            }
            else
            {
                if (_isRagdoll)
                    ToggleKinematicLimb(true);
                //get the direction to the target from the root
                Vector3 direction = (_follow.position - _positions[0]).normalized;
                //for every joint, just stretch it torward that direction
                for (int i = 1; i < _positions.Length; i++)
                {
                    //set the current joint position based on the direction to the target and the length of the last bone
                    _positions[i] = _positions[i - 1] + direction * _bonesLength[i - 1];
                }
            }
        }
        //if the target is within reach
        else
        {
            if (_isRagdoll)
                ToggleKinematicLimb(true);

            //apply snapback
            for (int i = 0; i < _positions.Length - 1; i++)
            {
                _positions[i + 1] = Vector3.Lerp(_positions[i + 1], _positions[i] + rootRotDif * _startDirectionSucc[i], _snapBackStrength);
            }

            //defines how many times to repeat the kinematics
            for (int iteration = 0; iteration < _iterations; iteration++)
            {
                //from the tip backward, not touching the root, forcefully pull the joints towards the target
                for (int i = _positions.Length - 1; i > 0; i--)
                {
                    //if we're the tip, snap to the target
                    if (i == _positions.Length - 1)
                        _positions[i] = _follow.position;
                    //else set the position based on the direction from the previous bone to this bone and this bone's length
                    else
                        _positions[i] = _positions[i + 1] + (_positions[i] - _positions[i + 1]).normalized * _bonesLength[i];
                }

                //from the root forward, pull towards the root based on the bone lengths and desired angles
                for (int i = 1; i < _positions.Length; i++)
                {
                    _positions[i] = _positions[i - 1] + (_positions[i] - _positions[i - 1]).normalized * _bonesLength[i - 1];
                }

                //if we're within snapping distance of the target, stop
                if ((_positions[_positions.Length - 1] - _follow.position).sqrMagnitude < _snappingDist * _snappingDist)
                    break;
            }
        }

        if (_isRagdoll)
            return;

        //if we have a direction to bend towards
        if (_pole != null)
        {
            //for each joint position
            for (int i = 1; i < _positions.Length - 1; i++)
            {
                //create an infinite plane around the previous point, using the axis between the previous point and the next point
                Plane plane = new Plane(_positions[i + 1] - _positions[i - 1], _positions[i - 1]);
                //get the projected position of the pole on the plane
                Vector3 projectedPole = plane.ClosestPointOnPlane(_pole.position);
                //do the same with the current joint
                Vector3 projectedJoint = plane.ClosestPointOnPlane(_positions[i]);
                //get the angle on the plane at which point the projected joint is closest to the projected pole
                float angle = Vector3.SignedAngle(projectedJoint - _positions[i - 1], projectedPole - _positions[i - 1], plane.normal);
                //set the position of the joint based on that angle around the plane direction
                _positions[i] = Quaternion.AngleAxis(angle, plane.normal) * (_positions[i] - _positions[i - 1]) + _positions[i - 1];
            }
        }

        //reapply the positions and the rotations to the joints
        for (int i = 0; i < _positions.Length; i++)
        {
            if (i == _positions.Length - 1)
            {
                _joints[i].rotation = _follow.rotation * Quaternion.Inverse(_startRotationTarget) * _startRotationBone[i];
            }
            else
            {
                _joints[i].rotation = Quaternion.FromToRotation(_startDirectionSucc[i], _positions[i + 1] - _positions[i]) * _startRotationBone[i];
            }
            _joints[i].position = _positions[i];
        }
    }


    private void HandleLineRenderer()
    {
        if (_lineRenderer == null)
            return;

        for (int i = 0; i < _chainLength + 1; i++)
        {
            _lineRenderer.SetPosition(i, _joints[i].position);
        }
    }

    public void ToggleKinematicLimb(bool on)
    {
        foreach (var rb in _rigidbodies)
        {
            rb.isKinematic = on;
            rb.useGravity = !on;
        }
        _isRagdoll = !on;
    }
}
