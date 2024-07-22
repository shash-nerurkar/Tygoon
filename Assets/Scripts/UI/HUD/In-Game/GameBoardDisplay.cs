using System;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class GameBoardDisplay : MonoBehaviour
{
    #region Actions

    public static event Action<Card, int, bool> PlaceCardAction;

    #endregion


    #region Fields

    [ SerializeField ] private GameObject cardPrefab;

    [ SerializeField ] private Image boardImage;

    [ SerializeField ] private Transform [ ] placeableSpaceTransforms;

    [ SerializeField ] private Transform [ ] enemyPlaceableSpaceTransforms;

    [ SerializeField ] private Transform [ ] enemyAttackingSpaceTransforms;

    [ Header ( "Data" ) ]
    [ SerializeField ] private SerializedDictionary<Level, Sprite> levelGameBoardSprites;

    #endregion

    
    #region Methods

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

    public void SpawnEnemyCard ( CardData cardData, int rowNumber ) 
    {
        GameObject cardObject = Instantiate ( cardPrefab, transform );

        Card card = cardObject.GetComponent<Card> ( );
        card.UpdateData ( cardData, isEnemyCard: true );

        PlaceCard ( card, enemyPlaceableSpaceTransforms [ rowNumber ], rowNumber, isEnemyCard: true, isMoving: false );
    }
    
    public void MoveEnemyCard ( Card card, int rowNumber ) 
    {
        card.transform.DOMove ( enemyAttackingSpaceTransforms [ rowNumber ].position, 0.5f )
            .OnComplete ( ( ) =>
            {
                PlaceCard ( card, enemyAttackingSpaceTransforms [ rowNumber ], rowNumber, isEnemyCard: true, isMoving: true );
            } );
    }

    public void PlayCardAttackAnimation ( Card card, int rowNumber, bool isEnemyCard ) 
    {
        var spaceToAttack = isEnemyCard ? placeableSpaceTransforms [ rowNumber ] : enemyAttackingSpaceTransforms [ rowNumber ];
        card.transform.DOMove ( spaceToAttack.position, 0.15f )
            .OnComplete ( ( ) =>
            {
                 card.transform.DOLocalMove ( Vector3.zero, 0.3f );
            } );
    }

    private async void PlaceCard ( Card card, Transform spaceTransform, int rowNumber, bool isEnemyCard, bool isMoving ) 
    {
        if ( !isMoving ) 
            PlaceCardAction?.Invoke ( card, rowNumber, isEnemyCard );
        
        card.transform.SetParent ( spaceTransform, false );

        var spaceRectTransform = spaceTransform.GetComponent<RectTransform> ( );
        var cardRectTransform = card.GetComponent<RectTransform> ( );
        var cardAspectRatio = cardRectTransform.sizeDelta.y / cardRectTransform.sizeDelta.x;
        var cardNewWidth = spaceRectTransform.rect.height - 20;
        
        cardRectTransform.rotation = Quaternion.Euler ( 0, 0, isEnemyCard ? 90 : -90 );
        cardRectTransform.localScale = Constants.PlacedCardScale;
        cardRectTransform.sizeDelta = new Vector3 ( cardNewWidth, cardNewWidth * cardAspectRatio );

        if ( !isMoving ) 
            await Task.Delay ( 100 );

        cardRectTransform.anchoredPosition = Vector3.zero;
        cardRectTransform.localPosition = Vector3.zero;
    }

    #endregion
}
