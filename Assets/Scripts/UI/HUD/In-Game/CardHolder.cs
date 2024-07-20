using System.Collections.Generic;
using UnityEngine;

public class CardHolder : MonoBehaviour
{
    #region Fields

    [ SerializeField ] private GameObject cardPrefab;

    private List<Card> _cards;

    private RectTransform _rectTransform;

    #endregion


    #region Methods

    public void CreatePlayerCards ( List<CardData> cardDatas ) 
    {
        _cards = new List<Card> ( );
        
        foreach ( CardData cardData in cardDatas ) 
        {
            GameObject cardObject = Instantiate ( cardPrefab, transform );

            Card card = cardObject.GetComponent<Card> ( );
            card.UpdateData ( cardData );

            _cards.Add ( card );
        }
    }

    // private void FixedUpdate ( ) => InputManager.GetTopmostCard ( )?.transform.SetSiblingIndex ( transform.childCount - 1 );

    #endregion
}
