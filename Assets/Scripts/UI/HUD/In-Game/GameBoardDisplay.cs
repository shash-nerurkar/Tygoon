using System;
using DG.Tweening;
using UnityEngine;

public class GameBoardDisplay : MonoBehaviour
{
    #region Actions

    public static event Action<Card, int> PlaceCardAction;

    #endregion


    #region Fields

    [ SerializeField ] private Transform [ ] placeableSpaceTransforms;

    #endregion

    
    #region Methods

    private void Awake ( ) 
    {
        Card.PlacementValidityCheckAction += CheckCardPlacementValidity;
    }

    private void OnDestroy ( ) 
    {
        Card.PlacementValidityCheckAction -= CheckCardPlacementValidity;
    }
    
    private void CheckCardPlacementValidity ( Card card ) 
    {
        for (int i = 0; i < placeableSpaceTransforms.Length; i++) 
            if ( Vector3.Distance ( card.transform.position, placeableSpaceTransforms[ i ].position ) < 1.0f ) 
            {
                PlaceCard ( card, placeableSpaceTransforms[ i ], i );

                card.OnPlacementValidityChecked ( true );

                return;
            }

        card.OnPlacementValidityChecked ( false );
    }

    private void PlaceCard ( Card card, Transform placeableSpaceTransform, int rowNumber ) 
    {
        card.transform.SetParent ( placeableSpaceTransform, false );
        card.GetComponent<RectTransform> ( ).SetLocalPositionAndRotation ( Vector3.zero, Quaternion.Euler ( 0, 0, -90 ) );
        card.transform.DOScale ( new Vector3 ( 0.35f, 0.35f, 0.35f), 0.05f );
        card.transform.localPosition = Vector3.zero;

        PlaceCardAction?.Invoke ( card, rowNumber );
    }

    #endregion
}
