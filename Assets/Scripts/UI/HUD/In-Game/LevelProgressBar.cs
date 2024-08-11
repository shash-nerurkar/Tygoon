using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelProgressBar : MonoBehaviour
{
    #region Fields
    
    [ SerializeField ] private Slider slider;
    
    [ SerializeField ] private Image sliderHandle;
    
    [ SerializeField ] private ContentSizeFitter loseTextPanelFitter;
    
    [ SerializeField ] private TextMeshProUGUI loseTextLabel;
    
    [ SerializeField ] private ContentSizeFitter winTextPanelFitter;
    
    [ SerializeField ] private TextMeshProUGUI winTextLabel;

    [ Header ( "Data" ) ]
    [ SerializeField ] private SerializedDictionary<Level, string> loseTexts;
    public Dictionary<Level, string> LoseTexts => loseTexts.ToDictionary ( );
    [ SerializeField ] private SerializedDictionary<Level, string> winTexts;
    public Dictionary<Level, string> WinTexts => winTexts.ToDictionary ( );

    #endregion


    #region Methods

    public async void InitProgressBar ( Level level ) 
    {
        slider.minValue = -Constants.MaxProgress;
        slider.maxValue = Constants.MaxProgress;

        slider.value = 0;

        sliderHandle.color = Color.Lerp ( Color.red, Color.green, Mathf.InverseLerp ( slider.minValue, slider.maxValue, 0 ) );
        
        loseTextLabel.text = LoseTexts [ level ];
        winTextLabel.text = WinTexts [ level ];
        loseTextPanelFitter.enabled = false;
        winTextPanelFitter.enabled = false;

        await Task.Delay ( 100 );

        loseTextPanelFitter.enabled = true;
        winTextPanelFitter.enabled = true;
    }

    public void UpdateProgressBar ( int newValue ) 
    {
        var startValue = slider.value;
        DOTween.To ( ( ) => startValue, x => slider.value = x, Mathf.Clamp ( newValue, slider.minValue, slider.maxValue ), 0.5f );

        sliderHandle.DOColor ( Color.Lerp ( Color.red, Color.green, Mathf.InverseLerp ( slider.minValue, slider.maxValue, newValue ) ), 0.5f );
    }
    
    #endregion
}
