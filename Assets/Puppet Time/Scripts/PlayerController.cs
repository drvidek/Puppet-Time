using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public enum PlayerState { Idle, Walk }

    [SerializeField] private float _walkSpeed, _turnSpeed;
    [SerializeField] private Transform _hips;
    [SerializeField] private IKLegPair _legs;
    [SerializeField] private Rigidbody[] _nonLimbBodies;
    [SerializeField] private bool _debugHeldToggle;

    private Vector3[] _lastPositions = new Vector3[10];
    private int _lastPositionIndex;

    private UltimateXR.Manipulation.UxrGrabbableObject grabbable;

    public Transform Hips => _hips;

    public bool IsHeld => grabbable.IsBeingGrabbed || _debugHeldToggle;

    public float Scale => transform.parent.localScale.x;

    #region Singleton + Awake
    private static PlayerController _singleton;
    public static PlayerController main
    {
        get => _singleton;
        private set
        {
            if (_singleton == null)
            {
                _singleton = value;
            }
            else if (_singleton != value)
            {
                Debug.LogWarning("PlayerController instance already exists, destroy duplicate!");
                Destroy(value);
            }
        }
    }

    private void Awake()
    {
        main = this;
    }
    #endregion


    public Vector3 AverageDirection
    {
        get
        {
            Vector3 average = new();
            for (int i = 1; i < _lastPositions.Length; i++)
            {
                average += QMath.Direction(_lastPositions[i - 1], _lastPositions[i]);
            }
            average.y = 0;
            return average.normalized;
        }
    }

    //bool _shiftingHeight;

    private void Start()
    {
        grabbable = GetComponent<UltimateXR.Manipulation.UxrGrabbableObject>();
    }

    private void FixedUpdate()
    {

        foreach (var rb in _nonLimbBodies)
        {
            rb.isKinematic = IsHeld;
            rb.useGravity = !IsHeld;
        }

        _lastPositions[_lastPositionIndex] = transform.position;
        _lastPositionIndex++;
        _lastPositionIndex = QMath.WrapIndex(_lastPositionIndex, 0, _lastPositions.Length - 1);

        float vertAxis = Input.GetAxis("Vertical");
        if (vertAxis > 0)
        {
            Vector3 moveDir = new Vector3(0, 0, Input.GetAxis("Vertical"));
            moveDir = transform.TransformDirection(moveDir);
            transform.position += moveDir * _walkSpeed * Time.deltaTime;
        }

        float horAxis = Input.GetAxis("Horizontal");
        if (horAxis != 0)
        {
            transform.Rotate(0, horAxis * _turnSpeed * Time.deltaTime, 0);
        }


        Debug.Log(AverageDirection);

        // float height = (_legs.LeftFollow.Target.position.y + _legs.LeftFollow.Target.position.y) / 2;
        // Debug.Log($"{_legs.LeftFollow.Target.position.y} + {_legs.LeftFollow.Target.position.y} ({_legs.LeftFollow.Target.position.y + _legs.LeftFollow.Target.position.y}) / 2 = {height}");
        // transform.position = QMath.ReplaceVectorValue(transform.position, VectorValue.y, height);
    }
}
