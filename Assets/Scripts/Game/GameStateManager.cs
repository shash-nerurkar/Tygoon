using System;

public static class GameStateManager
{
    #region Actions

    public static event Action<GameState> OnGameStateChangeAction;

    public static event Action<InGameState> OnInGameStateChangeAction;

    #endregion


    #region Fields

    public static GameState CurrentGameState { get; private set; }

    public static InGameState CurrentInGameState { get; private set; }

    #endregion
    

    #region Methods

    public static void ChangeGameState ( GameState newGameState ) 
    {
        CurrentGameState = newGameState;
        
        OnGameStateChangeAction?.Invoke ( CurrentGameState );
    }

    public static void ChangeInGameState ( InGameState newInGameState ) 
    {
        CurrentInGameState = newInGameState;

        OnInGameStateChangeAction?.Invoke ( CurrentInGameState );
    }
    
    #endregion

}
