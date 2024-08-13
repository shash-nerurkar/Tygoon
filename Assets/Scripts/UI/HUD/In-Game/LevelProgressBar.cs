using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelProgressBar : MonoBehaviour
{
    #region Fields
    
    [ SerializeField ] private Slider slider;
    
    [ SerializeField ] private Image sliderHandle;
    
    [ SerializeField ] private TextMeshProUGUI loseTextLabel;
    
    [ SerializeField ] private TextMeshProUGUI winTextLabel;

    [ Header ( "Data" ) ]
    [ SerializeField ] private SerializedDictionary<Level, string> loseTexts;
    public Dictionary<Level, string> LoseTexts => loseTexts.ToDictionary ( );
    [ SerializeField ] private SerializedDictionary<Level, string> winTexts;
    public Dictionary<Level, string> WinTexts => winTexts.ToDictionary ( );

    #endregion


    #region Methods

    public void InitProgressBar ( Level level ) 
    {
        slider.minValue = -Constants.MaxProgress;
        slider.maxValue = Constants.MaxProgress;

        slider.value = 0;

        sliderHandle.color = Constants.ProgressBarNeutralColor;
        
        loseTextLabel.text = LoseTexts [ level ];
        winTextLabel.text = WinTexts [ level ];
    }

    public void UpdateProgressBar ( int newValue ) 
    {
        var startValue = slider.value;
        DOTween.To ( ( ) => startValue, x => slider.value = x, Mathf.Clamp ( newValue, slider.minValue, slider.maxValue ), 0.5f )
            .OnUpdate ( ( ) => {
                float tValue;
                Color startColor;
                Color endColor;

                if ( slider.value < 0 ) 
                {
                    tValue = EaseInQuad ( Mathf.InverseLerp ( slider.minValue, 0, newValue ) );
                    startColor = Constants.ProgressBarNegativeColor;
                    endColor = Constants.ProgressBarNeutralColor;
                }
                else 
                {
                    tValue = EaseInQuad ( Mathf.InverseLerp ( 0, slider.maxValue, newValue ) );
                    startColor = Constants.ProgressBarNeutralColor;
                    endColor = Constants.ProgressBarPositiveColor;
                }

                sliderHandle.color = Color.Lerp ( startColor, endColor, tValue );
            } );
        
        return;


        float EaseInQuad ( float t ) => t * t;
    }
    
    #endregion
}
