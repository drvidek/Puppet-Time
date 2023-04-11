using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKLegPair : MonoBehaviour
{
    [SerializeField] private IKFootFollow _leftFootFollow, _rightFootFollow;
    [SerializeField] private IKLimb _leftLeg, _rightLeg;
    [SerializeField] private Transform _leftHip, _rightHip;
    private Vector3 _leftHipOffset, _rightHipOffset;

    public IKFootFollow LeftFollow => _leftFootFollow;
    public IKFootFollow RightFollow => _rightFootFollow;

    public bool EitherLegMoving => _leftFootFollow.IsMoving || _rightFootFollow.IsMoving;

    private void Start()
    {
        _leftHipOffset = QMath.Direction(transform.position, _leftHip.position, false);
        _rightHipOffset = QMath.Direction(transform.position, _rightHip.position, false);
    }

    private void Update()
    {
        _leftLeg.Joints[0].position = transform.position + transform.TransformDirection(_leftHipOffset);
        _rightLeg.Joints[0].position = transform.position  + transform.TransformDirection(_rightHipOffset);

        if (!EitherLegMoving)
        {
            if (!_leftFootFollow.IsMoving && _leftFootFollow.DistanceToTarget > _leftFootFollow.MaxDistance && !_leftLeg.IsRagdoll)
            {
                _leftFootFollow.StartCoroutine(_leftFootFollow.MoveTowardsTarget(_rightFootFollow));
            }
            else
            if (!_rightFootFollow.IsMoving && _rightFootFollow.DistanceToTarget > _rightFootFollow.MaxDistance && !_rightLeg.IsRagdoll)
            {
                _rightFootFollow.StartCoroutine(_rightFootFollow.MoveTowardsTarget(_leftFootFollow));
            }
        }


    }

}
