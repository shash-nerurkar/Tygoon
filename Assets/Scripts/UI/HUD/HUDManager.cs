using UnityEngine;

public class HUDManager : MonoBehaviour
{
    #region Serialized Fields

    [ SerializeField ] private MainMenu mainMenu;

    [ SerializeField ] private CutsceneDisplay cutsceneDisplay;

    [ SerializeField ] private GameObject inGamePanel;

    [ SerializeField ] private DialogueBox dialogueBox;

    [ SerializeField ] private CardHolder cardHolder;

    [ SerializeField ] private LevelProgressBar levelProgressBar;

    #endregion


    #region Methods

    private void Awake ( ) 
    {
        GameStateManager.OnGameStateChangeAction += OnGameStateChange;
        GameStateManager.OnInGameStateChangeAction += OnInGameStateChange;

        InputManager.OnGoNextPressedAction += GoNextPressed;

        CutsceneManager.StartCutsceneAction += cutsceneDisplay.StartNewCutscene;
        
        CardManager.AddPlayerCardsAction += cardHolder.CreatePlayerCards;
    }
    
    private void OnDestroy ( ) 
    {
        GameStateManager.OnGameStateChangeAction -= OnGameStateChange;
        GameStateManager.OnInGameStateChangeAction -= OnInGameStateChange;

        InputManager.OnGoNextPressedAction -= GoNextPressed;

        CutsceneManager.StartCutsceneAction -= cutsceneDisplay.StartNewCutscene;
        
        CardManager.AddPlayerCardsAction -= cardHolder.CreatePlayerCards;
    }

    private void OnGameStateChange ( GameState state ) 
    {
        switch ( state ) 
        {
            case GameState.MainMenu:
                mainMenu.gameObject.SetActive( true );

                cutsceneDisplay.gameObject.SetActive( false );

                inGamePanel.SetActive ( false );

                break;

            case GameState.Cutscene:
                mainMenu.gameObject.SetActive( false );

                cutsceneDisplay.gameObject.SetActive( true );

                inGamePanel.SetActive ( false );

                break;

            case GameState.InGame:
                mainMenu.gameObject.SetActive( false );

                cutsceneDisplay.gameObject.SetActive( false );

                inGamePanel.SetActive ( true );

                break;
        }
    }

    private void OnInGameStateChange ( InGameState state ) 
    {
        
    }

    private void GoNextPressed ( ) 
    {
        switch ( GameStateManager.CurrentGameState ) 
        {
            case GameState.Cutscene:
                cutsceneDisplay.ShowNextSegment ( );

                break;

            case GameState.InGame:
                dialogueBox.ShowNextDialogue ( );

                break;
        }
    }

    #endregion
}