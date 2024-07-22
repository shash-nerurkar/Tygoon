using UnityEngine;
using UnityEngine.UI;

public class BackgroundUIManager : MonoBehaviour
{
    #region Fields

    [ SerializeField ] private Image backgroundImage;

    [ Header ( "Data" ) ]
    [ SerializeField ] private Sprite mainMenuBackgroundImage;
    [ SerializeField ] private SerializedDictionary<Level, Sprite> levelBackgroundImages;

    #endregion

    
    #region Methods

    private void Awake ( ) 
    {
        GameStateManager.OnGameStateChangeAction += OnGameStateChange;

        GameManager.OnLevelStartAction += OnLevelStart;
    }
    
    private void OnDestroy ( ) 
    {
        GameStateManager.OnGameStateChangeAction -= OnGameStateChange;

        GameManager.OnLevelStartAction -= OnLevelStart;
    }

    private void OnGameStateChange ( GameState state ) 
    {
        switch ( state ) 
        {
            case GameState.MainMenu:
                backgroundImage.sprite = mainMenuBackgroundImage;

                break;

            case GameState.Cutscene:
                backgroundImage.sprite = null;

                break;

            case GameState.InGame:
                break;
        }
    }

    private void OnLevelStart ( Level level ) 
    {
        backgroundImage.sprite = levelBackgroundImages.ToDictionary ( ) [ level ];
    }

    #endregion
}
