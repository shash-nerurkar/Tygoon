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

    public static event Action<Level> ShowLevelObjectiveAction;

    public static event Action<bool> ShowNoCardsLeftAction;

    public static event Action<CardData, int, bool> CardSpawnAction;

    public static event Action<Card, int, bool> CardMoveAction;

    public static event Action<Card, Card, int, bool> CardAttackAction;

    public static event Action<Card> CardDespawnAction;

    public static event Action OnAllCardsAttackingCompleteAction;

    public static event Action<Level> InitProgressBarAction;

    public static event Action<int> UpdateProgressBarAction;

    public static event Action<Level> OnPlayerWinAction;

    public static event Action<Level> OnPlayerLoseAction;

    #endregion


    #region Fields

    [ SerializeField ] private SerializedDictionary<Level, CardData [ ]> levelInitialEnemyCards;
    private Dictionary<Level, CardData [ ]> LevelInitialEnemyCards => levelInitialEnemyCards.ToDictionary ( );

    private ReadOnlyDictionary<Level, IEnemy> _levelEnemies = new ( dictionary: new Dictionary<Level, IEnemy> ( ) { 
        { Level.GovernmentOffice, new EnemyGovtEmployee ( ) },
        { Level.Bank, new EnemyBankHead ( ) },
        { Level.Building, new EnemyBachelors ( ) },
        { Level.ConstructionSite, new EnemyMob ( ) },
    } );

    private Level _currentLevel;

    private List<CardData> _currentPlayerCardDatas;

    private int _previousProgress = 0;

    private int _currentProgress = 0;

    private Card [ ] _playerPlacedCards;

    private Card [ ] _enemyPlacedCards;

    private Card [ ] _enemyAttackingCards;

    private int _movesSinceNoProgressOrCardsPlaced = 0;

    #endregion


    #region Methods

    private void Awake ( ) 
    {
        GameManager.OnLevelStartAction += OnLevelStart;
        GameManager.StartBattleAction += StartBattle;
        GameManager.ClearLevelDataAction += ClearData;

        CardManager.AddPlayerCardsAction += SetInitialPlayerCards; 

        BoardDisplay.PlaceCardAction += OnPlacedCard;

        Card.RemoveAction += RemoveCard;
        
        _playerPlacedCards = new Card [ 4 ];
        _enemyPlacedCards = new Card [ 4 ];
        _enemyAttackingCards = new Card [ 4 ];
    }

    private void OnDestroy ( ) 
    {
        GameManager.OnLevelStartAction -= OnLevelStart;
        GameManager.StartBattleAction -= StartBattle;
        GameManager.ClearLevelDataAction -= ClearData;

        CardManager.AddPlayerCardsAction -= SetInitialPlayerCards; 

        BoardDisplay.PlaceCardAction -= OnPlacedCard;

        Card.RemoveAction -= RemoveCard;
    }

    private void OnLevelStart ( Level level ) => InitializeBoardAction?.Invoke ( level );

    private void ClearData ( ) 
    {
        _previousProgress = 0;
        _currentProgress = 0;
        _movesSinceNoProgressOrCardsPlaced = 0;

        _currentPlayerCardDatas.Clear ( );

        _playerPlacedCards.Where ( card => card != null ).ToList ( ).ForEach ( card => Destroy ( card.gameObject ) );
        Array.Clear ( _playerPlacedCards, 0, _playerPlacedCards.Length );

        _enemyPlacedCards.Where ( card => card != null ).ToList ( ).ForEach ( card => Destroy ( card.gameObject ) );
        Array.Clear ( _enemyPlacedCards, 0, _enemyPlacedCards.Length );
        
        _enemyAttackingCards.Where ( card => card != null ).ToList ( ).ForEach ( card => Destroy ( card.gameObject ) );
        Array.Clear ( _enemyAttackingCards, 0, _enemyAttackingCards.Length );
    }

    private void SetInitialPlayerCards ( CardData [ ] playerCardDatas ) => _currentPlayerCardDatas = new List<CardData> ( playerCardDatas );

    private async void StartBattle ( Level level ) 
    {
        _currentLevel = level;

        InitProgressBarAction?.Invoke ( _currentLevel );

        GeneratePlayerInitialCardsAction?.Invoke ( _currentLevel );

        ShowLevelObjectiveAction?.Invoke ( _currentLevel );

        _levelEnemies [ _currentLevel ].Init ( cardDatas: LevelInitialEnemyCards [ _currentLevel ] );

        await Task.Delay ( 3000 );

        SpawnEnemyCard ( );
    }
    
    private async void SpawnEnemyCard ( ) 
    {
        GameStateManager.ChangeInGameState ( InGameState.EnemyPlayingCard );

        _previousProgress = _currentProgress;
        
        if ( CanEnemyPlayAnyCards ( ) ) 
        {
            _levelEnemies [ _currentLevel ].PlayCard ( _playerPlacedCards, _enemyPlacedCards, _enemyAttackingCards, out var enemyCardDataToPlay, out var rowNumber );

            await Task.Delay ( UnityEngine.Random.Range ( 500, 1000 ) );

            if ( enemyCardDataToPlay != null ) 
                CardSpawnAction?.Invoke ( enemyCardDataToPlay, rowNumber, true );
            else 
                SetPlayerInputIfPossible ( );
        }
        else 
        {
            ShowNoCardsLeftAction?.Invoke ( true );

            await Task.Delay ( 1500 );
            
            SetPlayerInputIfPossible ( );
        }
    }

    private async void SetPlayerInputIfPossible ( ) 
    {
        GameStateManager.ChangeInGameState ( InGameState.WaitingForPlayerInput );

        if ( !CanPlayerPlayAnyCards ( ) ) 
        {
            ShowNoCardsLeftAction?.Invoke ( false );

            await Task.Delay ( 1500 );

            PlayerCardsAttack ( );
        }
    }

    private async void OnPlacedCard ( Card card, int rowNumber, bool isEnemyCard ) 
    {
        card.OnPlaced ( );

        if ( isEnemyCard ) 
        {
            _enemyPlacedCards [ rowNumber ] = card;

            await Task.Delay ( 500 );
            
            SetPlayerInputIfPossible ( );
        }
        else
        {
            _currentPlayerCardDatas.Remove ( card.Data );

            _playerPlacedCards [ rowNumber ] = card;

            PlayerCardsAttack ( );
        }
    }

    private async void PlayerCardsAttack ( ) 
    {
        GameStateManager.ChangeInGameState ( InGameState.AllCardsAttacking );
        
        for ( var i = 0; i < _playerPlacedCards.Length; i++ ) 
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
        for ( var i = 0; i < _enemyAttackingCards.Length; i++ ) 
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

        OnAllCardsAttackingCompleteAction?.Invoke ( );
        
        if ( _previousProgress.Equals ( _currentProgress ) && !CanPlayerPlayAnyCards ( ) && !CanEnemyPlayAnyCards ( ) ) 
        {
            ++_movesSinceNoProgressOrCardsPlaced;

            if ( _movesSinceNoProgressOrCardsPlaced > 5 ) 
            {
                OnPlayerLoseAction?.Invoke ( _currentLevel );
                return;
            }
        }

        SpawnEnemyCard ( );
    }

    private void EndGame ( ) 
    {
        OnAllCardsAttackingCompleteAction?.Invoke ( );

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

            CardAttackAction?.Invoke ( card, _enemyAttackingCards [ rowNumber ], rowNumber, isEnemyCard );

            await Task.Delay ( 1150 );

            if ( _enemyAttackingCards [ rowNumber ] != null ) 
                _enemyAttackingCards [ rowNumber ].UpdateHealth ( card.Damage );
            else 
                UpdateProgress ( card.Damage );
            
            await Task.Delay ( 500 );
        }
        else 
        {
            if ( isAttacking ) 
            {
                if ( card.Damage == 0 ) 
                    return;

                CardAttackAction?.Invoke ( card, _playerPlacedCards [ rowNumber ], rowNumber, isEnemyCard );

                await Task.Delay ( 1150 );
                
                if ( _playerPlacedCards [ rowNumber ] != null ) 
                    _playerPlacedCards [ rowNumber ].UpdateHealth ( card.Damage );
                else 
                    UpdateProgress ( -card.Damage );
            
                await Task.Delay ( 500 );
            }
            else 
            {
                if ( _enemyAttackingCards [ rowNumber ] == null ) 
                {
                    _enemyAttackingCards [ rowNumber ] = _enemyPlacedCards [ rowNumber ];
                    _enemyPlacedCards [ rowNumber ] = null;

                    CardMoveAction?.Invoke ( card, rowNumber, true );
                    
                    await Task.Delay ( 500 );
                    
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
        }
        else if ( _enemyAttackingCards.Contains ( card ) ) 
        {
            var cardIndex = Array.IndexOf ( _enemyAttackingCards, card );
            _enemyAttackingCards [ cardIndex ] = null;
        }
        else if ( _enemyPlacedCards.Contains ( card ) ) 
        {
            var cardIndex = Array.IndexOf ( _enemyPlacedCards, card );
            _enemyPlacedCards [ cardIndex ] = null;
        }
        else 
            return;

        CardDespawnAction?.Invoke ( card );
    }

    private void UpdateProgress ( int valueToAdd ) 
    {
        _currentProgress += valueToAdd;
    
        UpdateProgressBarAction?.Invoke ( _currentProgress );
    }
    
    private bool CanPlayerPlayAnyCards ( ) => _currentPlayerCardDatas.Any ( ) && _playerPlacedCards.Any ( card => card == null );
 
    private bool CanEnemyPlayAnyCards ( ) => _levelEnemies [ _currentLevel ].CardDatas.Any ( ) && _enemyPlacedCards.Any ( card => card == null );

    private bool HasProgressReachedTarget ( ) => Mathf.Abs ( _currentProgress ) >= Constants.MaxProgress;

    #endregion


    #endregion
}
