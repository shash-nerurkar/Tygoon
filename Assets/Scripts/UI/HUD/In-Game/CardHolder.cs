using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class CardHolder : MonoBehaviour
{
    #region Fields

    [ SerializeField ] private GameObject cardPrefab;

    [ SerializeField ] private GameObject indicatorPanel;

    [ SerializeField ] private GameObject noCardsLeftPanel;

    [ SerializeField ] private RectTransform rectTransform;

    [ SerializeField ] private HorizontalLayoutGroup layoutGroup;

    public static List<Card> Cards = new ( );

    private Card _currentlyDraggedCard;

    private Tweener _highlightCardsTween;

    private Tweener _unHighlightCardsTween;

    private Tweener _hideCardsTween;

    #endregion


    #region Methods

    public void CreatePlayerCards ( CardData [ ] cardDatas ) 
    {
        for ( var i = 0; i < cardDatas.Length; i++ ) 
        {
            GameObject cardObject = Instantiate ( cardPrefab, transform );

            Card card = cardObject.GetComponent<Card> ( );
            card.Initialize ( cardDatas [ i ], isEnemyCard: false );

            Cards.Add ( card );
        }
        
        SetHolderState ( );
    }

    public void ClearCard ( Card card, int _, bool isEnemyCard ) 
    {
        if ( !isEnemyCard ) 
            Cards.Remove ( card );
        
        SetHolderState ( );
    }

    public void ClearAllCards ( ) 
    {
        foreach ( Card card in Cards ) 
            Destroy ( card.gameObject );

        Cards.Clear ( );
        
        SetHolderState ( );

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

    public void DragCard ( Card clickedCard ) 
    {
        _currentlyDraggedCard = clickedCard;

        clickedCard.BeginDrag ( );

        HideCards ( );
    }

    public void StopDraggingCard ( ) 
    {
        _currentlyDraggedCard.EndDrag ( );
        
        _currentlyDraggedCard = null;
    }


    #region Helpers

    private void HighLightCards ( ) => PlayMovementTween ( _highlightCardsTween, Constants.CardHolderHighlightedPosition, 0.25f );

    private void UnHighlightCards ( ) => PlayMovementTween ( _unHighlightCardsTween, Constants.CardHolderUnHighlightedPosition, 0.4f );

    private void HideCards ( ) => PlayMovementTween ( _hideCardsTween, Constants.CardHolderHiddenPosition, 0.25f );

    private void PlayMovementTween ( Tweener tween, Vector3 destinationPosition, float travelTimeInSeconds ) 
    {
        if ( tween != null && tween.IsActive ( ) ) 
            return;
        
        DestroyTweenIfActive ( _highlightCardsTween );
        DestroyTweenIfActive ( _unHighlightCardsTween );
        DestroyTweenIfActive ( _hideCardsTween );

        tween = rectTransform.DOAnchorPos ( destinationPosition, travelTimeInSeconds )
            .OnComplete ( ( ) => DestroyTweenIfActive ( tween ) );

        return;
        

        static void DestroyTweenIfActive ( Tweener tween ) 
        {
            if ( tween != null && tween.IsActive ( ) ) 
            {
                tween.Kill ( );
                tween = null;
            }
        }
    }

    private void SetHolderState ( ) 
    {
        bool isCardHolderEmpty = !Cards.Any ( );

        indicatorPanel.SetActive ( !isCardHolderEmpty );
        noCardsLeftPanel.SetActive ( isCardHolderEmpty );
    }

    #endregion


    private void FixedUpdate ( ) 
    {
        if ( _currentlyDraggedCard == null || _currentlyDraggedCard.IsPlaced )
            if ( InputManager.IsMouseOverCards ( ) ) 
                HighLightCards ( );
            else 
                UnHighlightCards ( );
    }

    #endregion
}
