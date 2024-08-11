using System;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    #region Actions

    public static event Action OnRestartPressedAction;

    #endregion

    
    #region Serialized Fields

    [ SerializeField ] private Transition transition;

    [ SerializeField ] private MainMenu mainMenu;

    [ SerializeField ] private CutsceneDisplay cutsceneDisplay;

    [ SerializeField ] private GameObject inGamePanel;

    [ SerializeField ] private Button restartLevelButton;

    [ SerializeField ] private BoardDisplay boardDisplay;

    [ SerializeField ] private DialogueBox dialogueBox;

    [ SerializeField ] private CardHolder cardHolder;

    [ SerializeField ] private LevelProgressBar levelProgressBar;

    [ SerializeField ] private CharacterDisplay characterDisplay;

    #endregion


    #region Methods

    private void Awake ( ) 
    {
        GameStateManager.OnGameStateChangeAction += OnGameStateChanged;
        GameStateManager.OnInGameStateChangeAction += OnInGameStateChanged;

        GameManager.ShowTransitionAction += transition.FadeIn;

        InputManager.OnGoNextPressedAction += GoNextPressed;

        CutsceneManager.StartCutsceneAction += cutsceneDisplay.StartNewCutscene;

        Card.PlacementValidityCheckAction += boardDisplay.CheckCardPlacementValidity;
        BoardManager.InitializeBoardAction += boardDisplay.OnLevelStart;
        BoardManager.ShowLevelObjectiveAction += boardDisplay.ShowLevelObjective;
        BoardManager.CardSpawnAction += boardDisplay.CardSpawn;
        BoardManager.CardMoveAction += boardDisplay.CardMove;
        BoardManager.CardAttackAction += boardDisplay.CardAttack;
        BoardManager.CardDespawnAction += boardDisplay.CardDespawn;
        BoardManager.OnAllCardsAttackingCompleteAction += boardDisplay.OnAllCardsAttackingComplete;
        InputManager.OnCardZoomStartedAction += boardDisplay.ZoomCard;
        InputManager.OnCardZoomEndedAction += boardDisplay.StopZoomingCard;
        
        BoardManager.InitProgressBarAction += levelProgressBar.InitProgressBar;
        BoardManager.UpdateProgressBarAction += levelProgressBar.UpdateProgressBar;
        
        InputManager.OnCardDragStartedAction += cardHolder.DragCard;
        InputManager.OnCardDragEndedAction += cardHolder.StopDraggingCard;
        CardManager.AddPlayerCardsAction += cardHolder.CreatePlayerCards;
        BoardDisplay.PlaceCardAction += cardHolder.ClearCard;
        CardManager.ClearAllCardsAction += cardHolder.ClearAllCards;

        CharacterManager.GenerateLevelCharactersAction += characterDisplay.AddCharacters;
        CharacterManager.DestroyLevelCharactersAction += characterDisplay.ClearAllCharacters;
        BoardDisplay.PlayCharacterDamageAnimationAction += characterDisplay.PlayCharacterDamageAnimation;

        DialogueManager.StartDialogueSequenceAction += dialogueBox.StartNewDialogueSequence;
    }
    
    private void OnDestroy ( ) 
    {
        GameStateManager.OnGameStateChangeAction -= OnGameStateChanged;
        GameStateManager.OnInGameStateChangeAction -= OnInGameStateChanged;

        InputManager.OnGoNextPressedAction -= GoNextPressed;

        GameManager.ShowTransitionAction -= transition.FadeIn;

        CutsceneManager.StartCutsceneAction -= cutsceneDisplay.StartNewCutscene;

        Card.PlacementValidityCheckAction -= boardDisplay.CheckCardPlacementValidity;
        BoardManager.InitializeBoardAction -= boardDisplay.OnLevelStart;
        BoardManager.ShowLevelObjectiveAction -= boardDisplay.ShowLevelObjective;
        BoardManager.CardSpawnAction -= boardDisplay.CardSpawn;
        BoardManager.CardMoveAction -= boardDisplay.CardMove;
        BoardManager.CardAttackAction -= boardDisplay.CardAttack;
        BoardManager.CardDespawnAction -= boardDisplay.CardDespawn;
        BoardManager.OnAllCardsAttackingCompleteAction -= boardDisplay.OnAllCardsAttackingComplete;
        InputManager.OnCardZoomStartedAction -= boardDisplay.ZoomCard;
        InputManager.OnCardZoomEndedAction -= boardDisplay.StopZoomingCard;
        
        BoardManager.InitProgressBarAction -= levelProgressBar.InitProgressBar;
        BoardManager.UpdateProgressBarAction -= levelProgressBar.UpdateProgressBar;
        
        InputManager.OnCardDragStartedAction -= cardHolder.DragCard;
        InputManager.OnCardDragEndedAction -= cardHolder.StopDraggingCard;
        CardManager.AddPlayerCardsAction -= cardHolder.CreatePlayerCards;
        BoardDisplay.PlaceCardAction -= cardHolder.ClearCard;
        CardManager.ClearAllCardsAction -= cardHolder.ClearAllCards;

        CharacterManager.GenerateLevelCharactersAction -= characterDisplay.AddCharacters;
        CharacterManager.DestroyLevelCharactersAction -= characterDisplay.ClearAllCharacters;
        BoardDisplay.PlayCharacterDamageAnimationAction -= characterDisplay.PlayCharacterDamageAnimation;

        DialogueManager.StartDialogueSequenceAction -= dialogueBox.StartNewDialogueSequence;
    }

    private void OnGameStateChanged ( GameState state ) 
    {
        switch ( state ) 
        {
            case GameState.MainMenu:
                mainMenu.gameObject.SetActive ( true );

                cutsceneDisplay.gameObject.SetActive ( false );

                inGamePanel.SetActive  ( false );

                break;

            case GameState.Cutscene:
                mainMenu.gameObject.SetActive ( false );

                cutsceneDisplay.gameObject.SetActive ( true );

                inGamePanel.SetActive  ( false );

                break;

            case GameState.InGame:
                mainMenu.gameObject.SetActive ( false );

                cutsceneDisplay.gameObject.SetActive ( false );

                inGamePanel.SetActive  ( true );
                
                break;
        }
    }

    private void OnInGameStateChanged ( InGameState state ) 
    {
        boardDisplay.OnInGameStateChanged ( state );

        switch ( state ) 
        {
            case InGameState.DialogueShowing:
                boardDisplay.ShowBattleElements ( false );
                levelProgressBar.gameObject.SetActive ( false );
                dialogueBox.gameObject.SetActive ( true );
                cardHolder.gameObject.SetActive ( false );
                restartLevelButton.gameObject.SetActive ( false );

                break;

            case InGameState.EnemyPlayingCard:
                boardDisplay.ShowBattleElements ( true );
                levelProgressBar.gameObject.SetActive ( true );
                dialogueBox.gameObject.SetActive ( false );
                cardHolder.gameObject.SetActive ( false );
                restartLevelButton.gameObject.SetActive ( false ); // true );

                break;

            case InGameState.WaitingForPlayerInput:
                boardDisplay.ShowBattleElements ( true );
                levelProgressBar.gameObject.SetActive ( true );
                dialogueBox.gameObject.SetActive ( false );
                cardHolder.gameObject.SetActive ( true );
                restartLevelButton.gameObject.SetActive ( false ); // true );

                cardHolder.SetCards ( );

                break;

            case InGameState.AllCardsAttacking:
                boardDisplay.ShowBattleElements ( true );
                levelProgressBar.gameObject.SetActive ( true );
                dialogueBox.gameObject.SetActive ( false );
                cardHolder.gameObject.SetActive ( false );
                restartLevelButton.gameObject.SetActive ( false ); // true );

                break;
        }
    }

    private void GoNextPressed ( ) 
    {
        switch ( GameStateManager.CurrentGameState ) 
        {
            case GameState.Cutscene:
                SoundManager.Instance.Play ( SoundType.OnCutscenePopped );

                cutsceneDisplay.ShowNextSegment ( );

                break;

            case GameState.InGame:
                SoundManager.Instance.Play ( SoundType.OnDialoguePopped );

                dialogueBox.ShowNextDialogue ( );

                break;
        }
    }

    public void OnRestartPressed ( ) => OnRestartPressedAction?.Invoke ( );

    #endregion
}