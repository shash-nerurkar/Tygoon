using System;
using System.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BoardDisplay : MonoBehaviour
{
    #region Actions

    public static event Action<Card, int, bool> PlaceCardAction;

    public static event Action<bool> PlayCharacterDamageAnimationAction;

    #endregion


    #region Fields

    
    #region Serialized

    [ SerializeField ] private Image boardImage;

    [ SerializeField ] private RectTransform rectTransform;

    [ Header ( "In-Game State Display" ) ]
    [ SerializeField ] private Image inGameStatePanelImage;

    [ SerializeField ] private TextMeshProUGUI inGameStateLabel;

    [ Header ( "Playable Spaces" ) ]
    [ SerializeField ] private GameObject spaceColumns;

    [ SerializeField ] private Transform [ ] placeableSpaceTransforms;

    [ SerializeField ] private Transform [ ] enemyAttackingSpaceTransforms;

    [ SerializeField ] private Transform [ ] enemyPlaceableSpaceTransforms;

    [ Header ( "Indicators" ) ]
    [ SerializeField ] private RectTransform indicatorContainerTransform;
    [ SerializeField ] private BoardSpaceIndicator currentSpaceIndicator;
    
    [ Header ( "Prefabs" ) ]
    [ SerializeField ] private GameObject cardPrefab;
    [ SerializeField ] private GameObject damageIndicatorPrefab;

    [ Header ( "Data" ) ]
    [ SerializeField ] private SerializedDictionary<Level, Sprite> levelGameBoardSprites;
    [ SerializeField ] private SerializedDictionary<Level, string> levelObjectiveTexts;

    #endregion


    private Card _currentlyZoomedCard;

    #endregion

    
    #region Methods

    public void ShowBattleElements ( bool toggleFlags ) 
    {
        // inGameStatePanelImage.gameObject.SetActive ( toggleFlags );
        spaceColumns.SetActive ( toggleFlags );
        indicatorContainerTransform.gameObject.SetActive ( toggleFlags );
    }

    public void OnInGameStateChanged ( InGameState state ) 
    {
        switch ( state ) 
        {
            case InGameState.DialogueShowing:
                IndicateInGameState ( "...", Constants.NeutralCardColor, 0f );

                break;

            case InGameState.EnemyPlayingCard:
                IndicateInGameState ( "THEIR MOVE", Constants.EnemyCardColor );

                break;

            case InGameState.WaitingForPlayerInput:
                IndicateInGameState ( "YOUR MOVE", Constants.PlayerCardColor );

                break;

            case InGameState.AllCardsAttacking:
                IndicateInGameState ( "CARDS ARE IN ACTION", Constants.NeutralCardColor );

                break;
        }
    }
    
    public void ShowLevelObjective ( Level level ) 
    {
        IndicateInGameState ( levelObjectiveTexts.ToDictionary ( ) [ level ], Constants.NeutralCardColor, scaleFactor: 1.2f, indicationTime: 2f );
    }

    public void OnLevelStart ( Level level ) 
    {
        var sprite = levelGameBoardSprites.ToDictionary ( ) [ level ];
        
        boardImage.enabled = sprite != null;
        boardImage.sprite = sprite;
    }

    public void CheckCardPlacementValidity ( Card card ) 
    {
        for ( var i = 0; i < placeableSpaceTransforms.Length; i++ ) 
            if ( Vector3.Distance ( card.transform.position, placeableSpaceTransforms [ i ].position ) < 1.0f && 
                    placeableSpaceTransforms [ i ].childCount == 1 ) 
            {
                PlaceCard ( card, placeableSpaceTransforms[ i ], i, isEnemyCard: false, isMoving: false );

                card.OnPlacementValidityChecked ( true );

                return;
            }

        card.OnPlacementValidityChecked ( false );
    }

    public async void CardSpawn ( CardData cardData, int rowNumber, bool isEnemyCard ) 
    {
        if ( isEnemyCard ) 
        {
            GameObject cardObject = Instantiate ( cardPrefab, enemyPlaceableSpaceTransforms [ rowNumber ].position, Quaternion.identity, transform );

            Card card = cardObject.GetComponent<Card> ( );
            card.Initialize ( cardData, isEnemyCard: true );

            await Task.Delay ( 1000 );

            PlaceCard ( card, enemyPlaceableSpaceTransforms [ rowNumber ], rowNumber, isEnemyCard: true, isMoving: false );
        }
        else 
        {}
    }
    
    public void CardMove ( Card card, int rowNumber, bool isEnemyCard ) 
    {
        if ( isEnemyCard ) 
        {
            IndicateSpace ( true, enemyPlaceableSpaceTransforms [ rowNumber ] );

            card.transform.DOMove ( enemyAttackingSpaceTransforms [ rowNumber ].position, 0.5f )
                .OnComplete ( ( ) =>
                {
                    PlaceCard ( card, enemyAttackingSpaceTransforms [ rowNumber ], rowNumber, isEnemyCard: true, isMoving: true );
                } );
        }
        else 
        {}
    }

    public void CardAttack ( Card card, Card targetCard, int rowNumber, bool isEnemyCard ) 
    {
        var spaceToAttack = isEnemyCard ? placeableSpaceTransforms [ rowNumber ] : enemyAttackingSpaceTransforms [ rowNumber ];
     
        IndicateSpace ( true, isEnemyCard ? enemyAttackingSpaceTransforms [ rowNumber ] : placeableSpaceTransforms [ rowNumber ] );

        card.transform.DOMove ( spaceToAttack.position, 0.15f ) 
            .SetDelay ( 1f ) 
            .OnComplete ( ( ) =>
            {
                SoundManager.Instance.Play ( SoundType.OnCardAttack );
                
                if ( targetCard != null ) 
                {
                    targetCard.transform.DOShakePosition ( 0.3f, strength: new Vector3 ( 10, 10 ), vibrato: 10, randomness: 90 );

                    IndicateDamageDealt ( card.Damage.ToString ( ), spaceToAttack.position, Constants.CardDamagedColor );
                }
                else 
                {
                    PlayCharacterDamageAnimationAction.Invoke ( isEnemyCard );

                    var corners = new Vector3 [ 4 ];
                    rectTransform.GetWorldCorners ( corners );
                    IndicateDamageDealt ( ( isEnemyCard ? "-" : "+" ) + card.Damage.ToString ( ), 
                                            new Vector3 ( rectTransform.position.x, corners [ 1 ].y + 0.5f ),
                                            isEnemyCard ? Constants.PlayerDamagedColor : Constants.EnemyDamagedColor );
                }

                card.transform.DOLocalMove ( Vector3.zero, 0.3f );
            } );
    }

    public void CardDespawn ( Card card ) 
    {
        card.Despawn ( );
    }

    public void OnAllCardsAttackingComplete ( ) 
    {
        IndicateSpace ( false );
    }

    public void ZoomCard ( Card clickedCard ) 
    {
        _currentlyZoomedCard = clickedCard;

        clickedCard.BeginZoom ( transform );
    }

    public void StopZoomingCard ( ) 
    {
        _currentlyZoomedCard.EndZoom ( );
        
        _currentlyZoomedCard = null;
    }


    #region Helpers

    private void PlaceCard ( Card card, Transform spaceTransform, int rowNumber, bool isEnemyCard, bool isMoving ) 
    {
        if ( !isMoving ) 
            PlaceCardAction?.Invoke ( card, rowNumber, isEnemyCard );
        
        card.Place ( spaceTransform.GetComponent<RectTransform> ( ), isEnemyCard, isMoving );
    }

    private void IndicateInGameState ( string labelText, Color color, float scaleFactor = 1.3f, float indicationTime = 1f ) 
    {
        inGameStateLabel.text = labelText;
        inGameStateLabel.color = color;
                
        inGameStatePanelImage.color = color;

        inGameStateLabel.transform.DOScale ( scaleFactor, 0.2f ) 
            .OnComplete ( ( ) => inGameStateLabel.transform.DOScale ( 1f, 0.35f ).SetDelay ( indicationTime ) );
    }

    private void IndicateSpace ( bool show, Transform space = null ) 
    {
        currentSpaceIndicator.gameObject.SetActive ( show );

        if ( show ) 
        {
            var spaceRectTransform = space.GetComponent<RectTransform> ( );
            currentSpaceIndicator.Initialize ( spaceRectTransform.sizeDelta + new Vector2 ( 5, 5 ), spaceRectTransform.position );
        }
    }

    private void IndicateDamageDealt ( string damage, Vector3 position, Color color ) 
    {
        var damageIndicatorObject = Instantiate ( damageIndicatorPrefab, position, Quaternion.identity, indicatorContainerTransform );

        var damageIndicator = damageIndicatorObject.GetComponent<DamageIndicator> ( );
        damageIndicator.Initialize ( damage, color );
    }

    #endregion


    #endregion
}
