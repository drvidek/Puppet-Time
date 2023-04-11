using System;
using UnityEngine;

[Serializable]
public class MultiMeter
{
    [SerializeField] private Meter[] _meters;
    private int _meterIndex;

    /// <summary>
    /// Returns the total numerical value of the range of all meters
    /// </summary>
    public float Total
    {
        get
        {
            float total = 0;
            for (int i = 0; i < _meters.Length; i++)
            {
                total += _meters[i].Range;
            }
            return total;
        }
    }
    /// <summary>
    /// Returns the meters in the MultiMeter
    /// </summary>
    public Meter[] Meters => _meters;
    /// <summary>
    /// Returns the currently active meter
    /// </summary>
    public Meter ActiveMeter { get => _meters[_meterIndex]; }
    /// <summary>
    /// Returns the minimum value of the final meter
    /// </summary>
    public float Min { get => _meters[0].Min; }
    /// <summary>
    /// Returns the maximum value of the first meter
    /// </summary>
    public float Max { get => _meters[_meters.Length - 1].Max; }

    /// <summary>
    /// Adjust the value of the multimeter by f, optionally enable clamping at the end of the current meter
    /// </summary>
    /// <param name="f"></param>
    /// <param name="clamp"></param>
    public void Adjust(float f, bool clamp = true)
    {
        while (Mathf.Abs(f) > 0)
        {
            ActiveMeter.Adjust(f, out f);
            if (ActiveMeter.IsEmpty)
            {
                if (clamp)
                    return;

                if (_meterIndex == 0)
                {
                    _meterIndex = 0;
                    break;
                }
                _meterIndex += f > 0 ? 1 : -1;

            }
            f = Mathf.Abs(f);
        }
    }

    public MultiMeter(Meter[] meters)
    {
        _meters = meters;
        _meterIndex = meters.Length;
    }

}