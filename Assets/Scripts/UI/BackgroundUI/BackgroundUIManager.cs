using UnityEngine;

public class BackgroundUIManager : MonoBehaviour
{
    
    #region Methods

    private void Awake ( ) 
    {
        GameStateManager.OnGameStateChangeAction += OnGameStateChange;
        GameStateManager.OnInGameStateChangeAction += OnInGameStateChange;
    }
    
    private void OnDestroy ( ) 
    {
        GameStateManager.OnGameStateChangeAction -= OnGameStateChange;
        GameStateManager.OnInGameStateChangeAction -= OnInGameStateChange;
    }

    private void OnGameStateChange ( GameState state ) 
    {
        switch ( state ) 
        {
            case GameState.MainMenu:
                

                break;

            case GameState.Cutscene:
                

                break;

            case GameState.InGame:
                

                break;
        }
    }

    private void OnInGameStateChange ( InGameState state ) 
    { }

    #endregion
}
