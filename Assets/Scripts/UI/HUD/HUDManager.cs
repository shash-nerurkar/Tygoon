using UnityEngine;

public class HUDManager : MonoBehaviour
{
    #region Serialized Fields

    [ SerializeField ] private MainMenu mainMenu;

    [ SerializeField ] private CutsceneDisplay cutsceneDisplay;

    [ SerializeField ] private GameObject inGamePanel;

    [ SerializeField ] private GameBoardDisplay gameBoardDisplay;

    [ SerializeField ] private DialogueBox dialogueBox;

    [ SerializeField ] private CardHolder cardHolder;

    [ SerializeField ] private LevelProgressBar levelProgressBar;

    [ SerializeField ] private CharacterContainer characterContainer;

    #endregion


    #region Methods

    private void Awake ( ) 
    {
        GameStateManager.OnGameStateChangeAction += OnGameStateChange;
        GameStateManager.OnInGameStateChangeAction += OnInGameStateChange;

        InputManager.OnGoNextPressedAction += GoNextPressed;

        CutsceneManager.StartCutsceneAction += cutsceneDisplay.StartNewCutscene;

        Card.PlacementValidityCheckAction += gameBoardDisplay.CheckCardPlacementValidity;
        BoardManager.InitializeBoardAction += gameBoardDisplay.OnLevelStart;
        BoardManager.SpawnEnemyCardAction += gameBoardDisplay.SpawnEnemyCard;
        BoardManager.MoveEnemyCardAction += gameBoardDisplay.MoveEnemyCard;
        BoardManager.CardAttackAction += gameBoardDisplay.PlayCardAttackAnimation;
        
        BoardManager.InitProgressBarAction += levelProgressBar.InitProgressBar;
        BoardManager.UpdateProgressBarAction += levelProgressBar.UpdateProgressBar;
        
        GameBoardDisplay.PlaceCardAction += cardHolder.ClearCard;
        CardManager.AddPlayerCardsAction += cardHolder.CreatePlayerCards;
        CardManager.ClearAllCardsAction += cardHolder.ClearAllCards;

        CharacterManager.GenerateLevelCharactersAction += characterContainer.AddCharacters;
        CharacterManager.DestroyLevelCharactersAction += characterContainer.ClearAllCharacters;

        DialogueManager.StartDialogueSequenceAction += dialogueBox.StartNewDialogueSequence;
    }
    
    private void OnDestroy ( ) 
    {
        GameStateManager.OnGameStateChangeAction -= OnGameStateChange;
        GameStateManager.OnInGameStateChangeAction -= OnInGameStateChange;

        InputManager.OnGoNextPressedAction -= GoNextPressed;

        CutsceneManager.StartCutsceneAction -= cutsceneDisplay.StartNewCutscene;

        Card.PlacementValidityCheckAction -= gameBoardDisplay.CheckCardPlacementValidity;
        BoardManager.InitializeBoardAction -= gameBoardDisplay.OnLevelStart;
        BoardManager.SpawnEnemyCardAction -= gameBoardDisplay.SpawnEnemyCard;
        BoardManager.MoveEnemyCardAction -= gameBoardDisplay.MoveEnemyCard;
        BoardManager.CardAttackAction -= gameBoardDisplay.PlayCardAttackAnimation;
        
        BoardManager.InitProgressBarAction -= levelProgressBar.InitProgressBar;
        BoardManager.UpdateProgressBarAction -= levelProgressBar.UpdateProgressBar;
        
        GameBoardDisplay.PlaceCardAction -= cardHolder.ClearCard;
        CardManager.AddPlayerCardsAction -= cardHolder.CreatePlayerCards;
        CardManager.ClearAllCardsAction -= cardHolder.ClearAllCards;

        CharacterManager.GenerateLevelCharactersAction -= characterContainer.AddCharacters;
        CharacterManager.DestroyLevelCharactersAction -= characterContainer.ClearAllCharacters;

        DialogueManager.StartDialogueSequenceAction -= dialogueBox.StartNewDialogueSequence;
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
        switch ( state ) 
        {
            case InGameState.None:
                levelProgressBar.gameObject.SetActive( false );
                dialogueBox.gameObject.SetActive( false );
                cardHolder.gameObject.SetActive( false );

                break;

            case InGameState.DialogueShowing:
                levelProgressBar.gameObject.SetActive( false );
                dialogueBox.gameObject.SetActive( true );
                cardHolder.gameObject.SetActive( false );

                break;

            case InGameState.EnemyPlayingCard:
                levelProgressBar.gameObject.SetActive( true );
                dialogueBox.gameObject.SetActive( false );
                cardHolder.gameObject.SetActive( false );

                break;

            case InGameState.WaitingForPlayerInput:
                levelProgressBar.gameObject.SetActive( true );
                dialogueBox.gameObject.SetActive( false );
                cardHolder.gameObject.SetActive( true );
                cardHolder.SetCards ( );

                break;

            case InGameState.AllCardsAttacking:
                levelProgressBar.gameObject.SetActive( true );
                dialogueBox.gameObject.SetActive( false );
                cardHolder.gameObject.SetActive( false );

                break;
        }
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