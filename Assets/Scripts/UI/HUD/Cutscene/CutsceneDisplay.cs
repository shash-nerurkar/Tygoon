using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CutsceneDisplay : MonoBehaviour
{
    #region Actions

    public static event Action OnSequenceCompleteAction;

    #endregion


    #region Fields

    [ SerializeField ] private Image backgroundImage;

    [ SerializeField ] private TextMeshProUGUI narrationLabel;

    private CutsceneSequenceData _currentSequenceData;

    private int _currentSegmentIndex;

    #endregion


    #region Methods

    public void StartNewCutscene ( CutsceneSequenceData cutsceneSequenceData ) 
    {
        if ( !cutsceneSequenceData.Segments.Any ( ) ) 
            OnSequenceCompleteAction?.Invoke ( );
        else 
        {
            _currentSequenceData = cutsceneSequenceData;
            _currentSegmentIndex = 0;
            ShowCurrentSegment ( );
        }
    }

    public void ShowNextSegment ( ) 
    {
        if ( _currentSegmentIndex < _currentSequenceData.Segments.Count - 1 ) 
        {
            ++_currentSegmentIndex;
            ShowCurrentSegment ( );
        }
        else 
        {
            _currentSequenceData = null;
            _currentSegmentIndex = -1;
            OnSequenceCompleteAction?.Invoke ( );
        }
    }

    public void ShowCurrentSegment ( ) 
    {
        backgroundImage.sprite = _currentSequenceData.Segments [ _currentSegmentIndex ].Image;
        narrationLabel.text = _currentSequenceData.Segments [ _currentSegmentIndex ].Text;
    }

    #endregion
}
