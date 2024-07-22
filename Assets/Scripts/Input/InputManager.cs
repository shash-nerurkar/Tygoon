using System;
using System.Collections.Generic;
using System.Linq;
using InputCustom;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager : MonoBehaviour
{
    #region Actions

    public static event Action OnGoNextPressedAction;

    #endregion


    #region Fields

    private InputActionsDefault _inputActionsDefault;

    private Card _currentlyHeldCard;

    #endregion


    #region Static

    public static Card GetTopmostCard ( ) 
    {
        PointerEventData pointerEventData = new ( EventSystem.current ) { position = Input.mousePosition };

        List<RaycastResult> raycastResults = new ( );
        EventSystem.current.RaycastAll ( pointerEventData, raycastResults );
        
        return raycastResults.Any ( ) ? raycastResults.First ( ).gameObject.GetComponent<Card> ( ) : null;
    }

    public static bool IsMouseOverCards ( ) 
    {
        PointerEventData pointerEventData = new ( EventSystem.current ) { position = Input.mousePosition };

        List<RaycastResult> raycastResults = new ( );
        EventSystem.current.RaycastAll ( pointerEventData, raycastResults );
        
        return raycastResults.Any ( ) 
                ? raycastResults.Any ( result => result.gameObject.GetComponent<CardHolder> ( ) )
                : false;
    }

    #endregion    

    
    #region Private Methods
    
    private void Awake ( ) 
    {
        GameStateManager.OnGameStateChangeAction += OnGameStateChange;
        GameStateManager.OnInGameStateChangeAction += OnInGameStateChange; 
        
        _inputActionsDefault = new InputActionsDefault();
        
        SetupGlobalActions ( );
        SetupInGameActions ( );
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
                _inputActionsDefault.Global.Disable ( );
                _inputActionsDefault.InGame.Disable ( );

                break;

            case GameState.Cutscene:
                _inputActionsDefault.Global.Enable ( );
                _inputActionsDefault.InGame.Disable ( );

                break;

            case GameState.InGame:
                _inputActionsDefault.Global.Enable ( );

                break;
        }
    }

    private void OnInGameStateChange ( InGameState state ) 
    {
        switch ( state ) 
        {
            case InGameState.None:
                _inputActionsDefault.InGame.Disable ( );

                break;

            case InGameState.DialogueShowing:
                _inputActionsDefault.InGame.Disable ( );

                break;

            case InGameState.EnemyPlayingCard:
                _inputActionsDefault.InGame.Disable ( );

                break;

            case InGameState.WaitingForPlayerInput:
                _inputActionsDefault.InGame.Enable ( );

                break;

            case InGameState.AllCardsAttacking:
                _inputActionsDefault.InGame.Disable ( );

                break;
        }
    }

    private void SetupGlobalActions ( ) 
    {
        _inputActionsDefault.Global.GoNext.started += _ => OnGoNextPressedAction?.Invoke ( );
    }

    private void SetupInGameActions ( ) 
    {
        _inputActionsDefault.InGame.DragCard.started += _ => {
            _currentlyHeldCard = GetTopmostCard ( );

            _currentlyHeldCard?.OnDragBegin ( );
        };

        _inputActionsDefault.InGame.DragCard.canceled += _ => {
            _currentlyHeldCard?.OnDragEnd ( );
            
            _currentlyHeldCard = null;
        };
    }

    #endregion
}
