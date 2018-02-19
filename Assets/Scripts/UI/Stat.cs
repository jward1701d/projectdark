using UnityEngine;
using System;
using System.Collections;

[Serializable]
public class Stat
{
    #region Fields / Variables
    [SerializeField]
    private BarScript bar;              // Bar script object.

    [SerializeField]
    private float maxVal;               // the maxinum value.

    [SerializeField]
    private float currentVal;           // the current value.

    public float MaxVal                 // Getter / setter for maxValue
    {
        get
        {
            return maxVal;              // returns the max value.
        }

        set
        {
            maxVal = value;             // sets the max value.
            bar.MaxValue = maxVal;      // sets the bars max value.
        }
    }

    public float CurrentVal             // Getter / Setter for current value.
    {
        get
        {
            return currentVal;          // returns the current value.
        }

        set
        {
            this.currentVal = Mathf.Clamp(value,0,MaxVal);  // Makes sure the current value can not exceed the max value.
            bar.Value = currentVal;                         // sets the bars value to this.
        }
    }
    #endregion

    #region Methods{Initialize()}
    /// <summary>
    /// Initializes the valuse the bar calss usues to draw the bar.
    /// </summary>
    public void Initialize()
    {
        this.MaxVal = maxVal;
        this.CurrentVal = currentVal;
    }
    #endregion
}
