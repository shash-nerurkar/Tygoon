using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class CardHolder : MonoBehaviour
{
    #region Fields

    [ SerializeField ] private GameObject cardPrefab;

    [ SerializeField ] private RectTransform rectTransform;

    [ SerializeField ] private HorizontalLayoutGroup layoutGroup;

    public static List<Card> Cards = new ( );

    #endregion


    #region Methods

    public void CreatePlayerCards ( CardData [ ] cardDatas ) 
    {
        for ( var i = 0; i < cardDatas.Length; i++ ) 
        {
            GameObject cardObject = Instantiate ( cardPrefab, transform );

            Card card = cardObject.GetComponent<Card> ( );
            card.UpdateData ( cardDatas [ i ], isEnemyCard: false );

            Cards.Add ( card );
        }
    }

    public void ClearCard ( Card card, int _, bool isEnemyCard ) 
    {
        if ( !isEnemyCard ) 
            Cards.Remove ( card );
    }

    public void ClearAllCards ( ) 
    {
        foreach ( Card card in Cards ) 
            Destroy ( card.gameObject );

        Cards.Clear ( );

        rectTransform.sizeDelta = new Vector2 ( 0, rectTransform.sizeDelta.y );
        layoutGroup.spacing = 0;
    }

    public async void SetCards ( ) 
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate ( rectTransform );

        var maxHolderWidth = Screen.width - 400;
        if ( rectTransform.rect.width > maxHolderWidth ) 
        {
            var overflowAmount = rectTransform.rect.width - maxHolderWidth;
            layoutGroup.spacing = -overflowAmount/Cards.Count;
        }
        
        LayoutRebuilder.ForceRebuildLayoutImmediate ( rectTransform );

        await Task.Delay ( 100 );

        foreach ( var card in Cards ) 
            card.SetCurrentAsBasePosition ( );
    }

    private void FixedUpdate ( ) 
    {
        // var topmostCardIndex = _cards.IndexOf ( InputManager.GetTopmostCard ( ) );

        if ( InputManager.IsMouseOverCards ( ) ) 
        {
            rectTransform.anchoredPosition = Constants.CardHolderShowPosition;

            // if ( topmostCardIndex >= 0 ) 
            //     for ( var index = 0; index < _cards.Count; index++ ) 
            //         _cards [ index ].SetOpacity ( index == topmostCardIndex + 1 ? 0.4f : 1f );
        }
        else 
        {
            rectTransform.anchoredPosition = Constants.CardHolderHidePosition;

            // for ( var index = 0; index < _cards.Count; index++ ) 
            //     _cards [ index ].SetOpacity ( 1f );
        }
    }

    #endregion
}
