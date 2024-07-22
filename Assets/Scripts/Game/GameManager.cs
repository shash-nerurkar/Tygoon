using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Actions

    public static event Action<Cutscene> OnCutsceneStartAction;

    public static event Action<Level> OnLevelStartAction;

    public static event Action<Level> StartBattleAction;

    public static event Action ClearLevelDataAction;

    #endregion


    #region Fields

    private Cutscene _currentCutscene;

    private Level _currentLevel;

    #endregion
  

    #region Methods

    private void Awake ( ) 
    {
        MainMenu.OnStartGameButtonPressedAction += StartNewGame;

        CutsceneDisplay.OnSequenceCompleteAction += OnCutsceneSequenceComplete;

        DialogueBox.OnSequenceCompleteAction += OnDialogueSequenceComplete;

        BoardManager.OnPlayerLoseAction += OnPlayerLose;
    }

    private void OnDestroy ( ) 
    {
        MainMenu.OnStartGameButtonPressedAction -= StartNewGame;

        CutsceneDisplay.OnSequenceCompleteAction -= OnCutsceneSequenceComplete;

        DialogueBox.OnSequenceCompleteAction -= OnDialogueSequenceComplete;

        BoardManager.OnPlayerLoseAction -= OnPlayerLose;
    }

    private void Start ( ) => StartMainMenu ( );

    private void StartNewGame ( ) => StartCutscene ( Cutscene.Pilot );

    private void OnCutsceneSequenceComplete ( ) 
    {
        switch ( _currentCutscene ) 
        {
            case Cutscene.Pilot:
                StartLevel ( Level.GovernmentOffice );

                break;

            case Cutscene.GoingToBank:
                StartLevel ( Level.Bank );

                break;

            case Cutscene.GoingToBuilding:
                StartLevel ( Level.Building );

                break;

            case Cutscene.GoingToConstructionSite:
                StartLevel ( Level.ConstructionSite );

                break;

            case Cutscene.Outro:
                StartMainMenu ( );
                break;
        }
    }

    private void OnDialogueSequenceComplete ( bool isOpeningSequence ) 
    {
        if ( isOpeningSequence ) 
            StartBattleAction?.Invoke ( _currentLevel );
        else 
        {
            ClearLevelDataAction?.Invoke ( );

            switch ( _currentLevel ) 
            {
                case Level.GovernmentOffice:
                    StartCutscene ( Cutscene.GoingToBank );

                    break;

                case Level.Bank:
                    StartCutscene ( Cutscene.GoingToBuilding );

                    break;

                case Level.Building:
                    StartCutscene ( Cutscene.GoingToConstructionSite );

                    break;

                case Level.ConstructionSite:
                    StartCutscene ( Cutscene.Outro );

                    break;
            }
        }
    }

    private void OnPlayerLose ( Level level ) 
    {
        ClearLevelDataAction?.Invoke ( );

        StartLevel ( level );
    }

    private void StartMainMenu ( ) 
    {
        GameStateManager.ChangeGameState ( GameState.MainMenu );
    }

    private void StartCutscene ( Cutscene cutscene ) 
    {
        _currentCutscene = cutscene;

        GameStateManager.ChangeGameState ( GameState.Cutscene );

        OnCutsceneStartAction?.Invoke ( cutscene );
    }

    private void StartLevel ( Level level ) 
    {
        _currentLevel = level;

        GameStateManager.ChangeGameState ( GameState.InGame );

        OnLevelStartAction?.Invoke ( level );
    }

    #endregion
}