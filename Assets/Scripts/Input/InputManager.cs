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

    public static event Action<Card> OnCardDragStartedAction;

    public static event Action OnCardDragEndedAction;

    public static event Action<Card> OnCardZoomStartedAction;

    public static event Action OnCardZoomEndedAction;

    #endregion


    #region Fields

    private InputActionsDefault _inputActionsDefault;

    #endregion


    #region Static

    public static bool IsMouseOverCards ( ) 
    {
        PointerEventData pointerEventData = new ( EventSystem.current ) { position = Input.mousePosition };

        List<RaycastResult> raycastResults = new ( );
        EventSystem.current.RaycastAll ( pointerEventData, raycastResults );
        
        return raycastResults.Any ( ) 
                ? raycastResults.Any ( result => result.gameObject.GetComponent<CardHolder> ( ) )
                : false;
    }

    public static Card GetTopmostCard ( ) 
    {
        PointerEventData pointerEventData = new ( EventSystem.current ) { position = Input.mousePosition };

        List<RaycastResult> raycastResults = new ( );
        EventSystem.current.RaycastAll ( pointerEventData, raycastResults );
        
        return raycastResults.Any ( ) ? raycastResults.First ( ).gameObject.GetComponent<Card> ( ) : null;
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
        Card currentlyHeldCard = null;

        _inputActionsDefault.InGame.ClickCard.started += _ => {
            currentlyHeldCard = GetTopmostCard ( );

            if ( currentlyHeldCard != null ) 
            {
                if ( currentlyHeldCard.IsPlaced ) 
                    OnCardZoomStartedAction?.Invoke ( currentlyHeldCard );
                else 
                    OnCardDragStartedAction?.Invoke ( currentlyHeldCard );
            }
        };

        _inputActionsDefault.InGame.ClickCard.canceled += _ => {
            if ( currentlyHeldCard != null ) 
            {
                if ( currentlyHeldCard.IsPlaced ) 
                    OnCardZoomEndedAction?.Invoke ( );
                else 
                    OnCardDragEndedAction?.Invoke ( );
            }

            currentlyHeldCard = null;
        };
    }

    #endregion
}
