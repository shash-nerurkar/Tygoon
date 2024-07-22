using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    #region Actions

    public static event Action<Level> InitializeBoardAction;

    public static event Action<Level> GeneratePlayerInitialCardsAction;

    public static event Action<CardData, int> SpawnEnemyCardAction;

    public static event Action<Card, int, bool> CardAttackAction;

    public static event Action<Card, int> MoveEnemyCardAction;

    public static event Action<Card, int, bool, bool> RemoveCardAction;

    public static event Action<Level> InitProgressBarAction;

    public static event Action<int> UpdateProgressBarAction;

    public static event Action<Level> OnPlayerWinAction;

    public static event Action<Level> OnPlayerLoseAction;

    #endregion


    #region Fields

    [ SerializeField ] private SerializedDictionary<Level, CardData [ ]> levelInitialEnemyCards;
    private Dictionary<Level, CardData [ ]> LevelInitialEnemyCards => levelInitialEnemyCards.ToDictionary ( );

    private ReadOnlyDictionary<Level, IEnemy> _levelEnemies = new ( 
        dictionary: new Dictionary<Level, IEnemy> ( ) 
        { 
            { Level.GovernmentOffice, new EnemyGovtEmployee ( ) },
            { Level.Bank, new EnemyBankHead ( ) },
            { Level.Building, new EnemyBachelors ( ) },
            { Level.ConstructionSite, new EnemyMob ( ) },
        } 
    );

    Level _currentLevel;

    int _currentProgress = 0;

    private Card [ ] _playerPlacedCards;
    private Card [ ] _enemyPlacedCards;
    private Card [ ] _enemyAttackingCards;

    #endregion


    #region Methods

    private void Awake ( ) 
    {
        GameManager.OnLevelStartAction += OnLevelStart;
        GameManager.StartBattleAction += StartBattle;
        GameManager.ClearLevelDataAction += ClearData;

        GameBoardDisplay.PlaceCardAction += OnPlacedCard;

        Card.RemoveAction += RemoveCard;
        
        _playerPlacedCards = new Card [ 4 ];
        _enemyPlacedCards = new Card [ 4 ];
        _enemyAttackingCards = new Card [ 4 ];
    }

    private void OnDestroy ( ) 
    {
        GameManager.OnLevelStartAction += OnLevelStart;
        GameManager.StartBattleAction -= StartBattle;
        GameManager.ClearLevelDataAction -= ClearData;

        GameBoardDisplay.PlaceCardAction -= OnPlacedCard;

        Card.RemoveAction -= RemoveCard;
    }

    private void OnLevelStart ( Level level ) 
    {
        InitializeBoardAction?.Invoke ( level );
    }

    private void ClearData ( ) 
    {
        _currentProgress = 0;

        foreach ( var card in _playerPlacedCards ) 
            if ( card != null ) Destroy ( card.gameObject );
        Array.Clear ( _playerPlacedCards, 0, _playerPlacedCards.Length );

        foreach ( var card in _enemyPlacedCards ) 
            if ( card != null ) Destroy ( card.gameObject );
        Array.Clear ( _enemyPlacedCards, 0, _enemyPlacedCards.Length );

        foreach ( var card in _enemyAttackingCards ) 
            if ( card != null ) Destroy ( card.gameObject );
        Array.Clear ( _enemyAttackingCards, 0, _enemyAttackingCards.Length );
    }

    private void StartBattle ( Level level ) 
    {
        _currentLevel = level;

        InitProgressBarAction?.Invoke ( _currentLevel );

        GeneratePlayerInitialCardsAction?.Invoke ( _currentLevel );

        _levelEnemies [ _currentLevel ].UpdateData ( isInitial: true, cardDatas: LevelInitialEnemyCards [ _currentLevel ], new List<int> ( ) { 0, 1, 2, 3 } );

        SpawnEnemyCard ( );
    }
    
    private void SpawnEnemyCard ( ) 
    {
        GameStateManager.ChangeInGameState ( InGameState.EnemyPlayingCard );

        var enemyPlaceableRows = new List<int> ( );
        for ( var i = 0; i < 4; i++ ) 
            if ( _enemyPlacedCards [ i ] == null ) 
                enemyPlaceableRows.Add ( i );
        _levelEnemies [ _currentLevel ].UpdateData ( isInitial: false, cardDatas: LevelInitialEnemyCards [ _currentLevel ], enemyPlaceableRows );

        _levelEnemies [ _currentLevel ].PlayCard ( out var enemyCardDataToPlay, out var rowNumber );

        if ( enemyCardDataToPlay != null ) 
            SpawnEnemyCardAction?.Invoke ( enemyCardDataToPlay, rowNumber );
        else 
            SetPlayerInputIfPossible ( );
    }

    private void SetPlayerInputIfPossible ( ) 
    {
        foreach ( var card in _playerPlacedCards ) 
            if ( card == null ) 
                if ( CardHolder.Cards.Any ( ) ) 
                {
                    GameStateManager.ChangeInGameState ( InGameState.WaitingForPlayerInput );
                    return;
                }
        
        PlayerCardsAttack ( );
    }

    private async void OnPlacedCard ( Card card, int rowNumber, bool isEnemyCard ) 
    {
        card.Place ( );

        if ( isEnemyCard ) 
        {
            _enemyPlacedCards [ rowNumber ] = card;

            await Task.Delay ( 1000 );
            
            SetPlayerInputIfPossible ( );
        }
        else
        {
            _playerPlacedCards [ rowNumber ] = card;

            await Task.Delay ( 1000 );

            PlayerCardsAttack ( );
        }
    }

    private async void PlayerCardsAttack ( ) 
    {
        GameStateManager.ChangeInGameState ( InGameState.AllCardsAttacking );
        
        for ( var i = 0; i < 4; i++ ) 
        {
            if ( _playerPlacedCards [ i ] == null ) 
                continue;

            await PerformCardAction ( _playerPlacedCards [ i ], i, false, true );
            
            if ( HasProgressReachedTarget ( ) ) 
            {
                EndGame ( );
                return;
            }
        }

        EnemyCardsAttack ( );
    }

    private async void EnemyCardsAttack ( ) 
    {
        for ( var i = 0; i < 4; i++ ) 
        {
            if ( _enemyAttackingCards [ i ] != null ) 
            {
                await PerformCardAction ( _enemyAttackingCards [ i ], i, true, true );
                
                if ( HasProgressReachedTarget ( ) ) 
                {
                    EndGame ( );
                    return;
                }
            }

            if ( _enemyPlacedCards [ i ] != null ) 
            {
                await PerformCardAction ( _enemyPlacedCards [ i ], i, true, false );
                
                if ( HasProgressReachedTarget ( ) ) 
                {
                    EndGame ( );
                    return;
                }
            }
        }

        SpawnEnemyCard ( );
    }

    private void EndGame ( ) 
    {
        GameStateManager.ChangeInGameState ( InGameState.None );

        if ( _currentProgress > 0 ) 
            OnPlayerWinAction?.Invoke ( _currentLevel );
        else 
            OnPlayerLoseAction?.Invoke ( _currentLevel );
    }


    #region Helpers

    private async Task PerformCardAction ( Card card, int rowNumber, bool isEnemyCard, bool isAttacking ) 
    {
        if ( !isEnemyCard ) 
        {
            if ( card.Damage == 0 ) 
                return;

            CardAttackAction?.Invoke ( card, rowNumber, isEnemyCard );

            if ( _enemyAttackingCards [ rowNumber ] != null ) 
                _enemyAttackingCards [ rowNumber ].UpdateHealth ( card.Damage );
            else 
                UpdateProgress ( card.Damage );
            
            await Task.Delay ( 1000 );
        }
        else 
        {
            if ( isAttacking ) 
            {
                if ( card.Damage == 0 ) 
                    return; 

                CardAttackAction?.Invoke ( card, rowNumber, isEnemyCard );
                
                if ( _playerPlacedCards [ rowNumber ] != null ) 
                    _playerPlacedCards [ rowNumber ].UpdateHealth ( card.Damage );
                else 
                    UpdateProgress ( -card.Damage );
            
                await Task.Delay ( 1000 );
            }
            else 
            {
                if ( _enemyAttackingCards [ rowNumber ] == null ) 
                {
                    _enemyAttackingCards [ rowNumber ] = _enemyPlacedCards [ rowNumber ];
                    _enemyPlacedCards [ rowNumber ] = null;
                    MoveEnemyCardAction?.Invoke ( card, rowNumber );
                    
                    await Task.Delay ( 1000 );
                    
                    await PerformCardAction ( card, rowNumber, isEnemyCard, true );
                }
            }
        }
    }
    
    private void RemoveCard ( Card card ) 
    {
        if ( _playerPlacedCards.Contains ( card ) ) 
        {
            var cardIndex = Array.IndexOf ( _playerPlacedCards, card );
            _playerPlacedCards [ cardIndex ] = null;

            RemoveCardAction?.Invoke ( card, cardIndex, false, true );
        }
        else if ( _enemyAttackingCards.Contains ( card ) ) 
        {
            var cardIndex = Array.IndexOf ( _enemyAttackingCards, card );
            _enemyAttackingCards [ cardIndex ] = null;

            RemoveCardAction?.Invoke ( card, cardIndex, true, true );
        }
        else if ( _enemyPlacedCards.Contains ( card ) ) 
        {
            var cardIndex = Array.IndexOf ( _enemyPlacedCards, card );
            _enemyPlacedCards [ cardIndex ] = null;

            RemoveCardAction?.Invoke ( card, cardIndex, true, false );
        }

        Destroy ( card.gameObject );
    }

    private void UpdateProgress ( int valueToAdd ) 
    {
        _currentProgress += valueToAdd;
    
        UpdateProgressBarAction?.Invoke ( _currentProgress );
    }
    
    private bool HasProgressReachedTarget ( ) => Mathf.Abs ( _currentProgress ) >= Constants.MaxProgress;

    #endregion


    #endregion
}
