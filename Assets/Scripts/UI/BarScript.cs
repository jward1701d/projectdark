using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BarScript : MonoBehaviour {

    #region Fields / Variable
    private float fillAmount;           // Amount the bar should be filled.

    [SerializeField]
    private float lerpSpeed;            // A speed to pass to the Mathf.Lerp function for smooth shifts in the bar position.

    [SerializeField]
    private Image content;              // Reference to the image being filled.

    [SerializeField]
    private Text valueText;             // the text displayed on the bar.

    [SerializeField]
    private Color fullColor;            // the color the bar should be when it is full.

    [SerializeField]
    private Color lowColor;             // the color the bar should be when it is low.

    [SerializeField]
    private bool changeColors;          // Allows us to change the color as the bar decreases or stay the same depending on what is called for.

    public float MaxValue { get; set; } // Gets and sets the max value of the bar.
    // Sets the value in the bars text field.
    public float Value
    {
        set
        {
            string[] tmp = valueText.text.Split(':');   // Splits the strig in the text at in this case the colon.
            valueText.text = tmp[0] + ": " + value;     // sets the bars text to the to wording plus the value.
            fillAmount = Map(value, 0, MaxValue, 0, 1); // makes sure that fill amount is correctly set.
        }
    }
    #endregion

    #region Unity Methods{Start(), Update()}
    // Use this for initialization
    void Start () {
        if (changeColors)   // checks if we are using the color changing bar feature.
        {
            content.color = fullColor;      // sets the full color in the bar.
        }
	}
	
	// Update is called once per frame
	void Update () {
        HandleBar();
	}
    #endregion

    #region Methods {HandleBar(), Map(float, float, float, float, float)}
    private void HandleBar()
    {
        if (fillAmount != content.fillAmount)   // Checks if the fillamount has changed since the last update.
        {
            content.fillAmount = Mathf.Lerp(content.fillAmount, fillAmount, Time.deltaTime * lerpSpeed); // Smoothly reduces or increase the bar depending on the fillAmount.
        }
        if (changeColors) 
        {
            content.color = Color.Lerp(lowColor, fullColor, fillAmount);    // Smoothyl blends the colors as the bar decreases and increase.
        }
    }
    // Function for returning the amoutn the bar should be filled.
    private float Map(float value, float inMin, float inMax, float outMin, float outMax )
    {
        return (value - inMin) * (outMax - outMin) / (inMax - inMin) + outMin;
    }
    #endregion
}
