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

    private List<Card> _cards = new ( );

    private Card _currentlyDraggedCard;

    private Tweener _highlightCardsTween;

    private Tweener _unHighlightCardsTween;

    private Tweener _hideCardsTween;

    private bool _shouldFadeOverlappingCard;

    #endregion


    #region Methods

    public void CreatePlayerCards ( CardData [ ] cardDatas ) 
    {
        for ( var i = 0; i < cardDatas.Length; i++ ) 
        {
            GameObject cardObject = Instantiate ( cardPrefab, transform );

            Card card = cardObject.GetComponent<Card> ( );
            card.Initialize ( cardDatas [ i ], isEnemyCard: false );

            _cards.Add ( card );
        }
        
        SetHolderState ( );
    }

    public void ClearCard ( Card card, int _, bool isEnemyCard ) 
    {
        if ( !isEnemyCard ) 
            _cards.Remove ( card );
        
        SetHolderState ( );
    }

    public void ClearAllCards ( ) 
    {
        foreach ( Card card in _cards ) 
            Destroy ( card.gameObject );

        _cards.Clear ( );
        
        SetHolderState ( );

        rectTransform.sizeDelta = new Vector2 ( 0, rectTransform.sizeDelta.y );
    }

    public async void SetCards ( ) 
    {
        if ( !_cards.Any ( ) ) 
            return;

        layoutGroup.spacing = 0;

        LayoutRebuilder.ForceRebuildLayoutImmediate ( rectTransform );

        layoutGroup.spacing = -Mathf.Clamp ( rectTransform.rect.width - ( Screen.width - 400 ), 0, Mathf.Infinity ) / _cards.Count;
        
        _shouldFadeOverlappingCard = layoutGroup.spacing < -40;
        
        LayoutRebuilder.ForceRebuildLayoutImmediate ( rectTransform );

        await Task.Delay ( 100 );

        foreach ( var card in _cards ) 
            card.SetCurrentAsBasePosition ( );
    }

    public void DragCard ( Card clickedCard ) 
    {
        _currentlyDraggedCard = clickedCard;

        _currentlyDraggedCard.BeginDrag ( );

        HideCards ( );
        
        FadeInAllCards ( );
    }

    public void StopDraggingCard ( ) 
    {
        _currentlyDraggedCard.EndDrag ( );
        
        _currentlyDraggedCard = null;
    }


    #region Helpers

    private void HighLightCards ( ) => PlayMovementTween ( ref _highlightCardsTween, Constants.CardHolderHighlightedPosition, 0.25f );

    private void UnHighlightCards ( ) => PlayMovementTween ( ref _unHighlightCardsTween, 
                    !_cards.Any ( ) ? Constants.CardHolderDisplayPosition : Constants.CardHolderUnHighlightedPosition, 0.4f );

    private void HideCards ( ) => PlayMovementTween ( ref _hideCardsTween, Constants.CardHolderHiddenPosition, 0.25f );

    private void PlayMovementTween ( ref Tweener tween, Vector3 destinationPosition, float travelTimeInSeconds ) 
    {
        if ( tween.IsActive ( ) ) 
            return;
        
        DestroyTweenIfActive ( _highlightCardsTween );
        DestroyTweenIfActive ( _unHighlightCardsTween );
        DestroyTweenIfActive ( _hideCardsTween );

        tween = rectTransform.DOAnchorPos ( destinationPosition, travelTimeInSeconds );

        return;
        

        static void DestroyTweenIfActive ( Tweener tween ) 
        {
            if ( tween.IsActive ( ) ) 
            {
                tween.Kill ( );
                tween = null;
            }
        }
    }

    private void SetHolderState ( ) 
    {
        bool isCardHolderEmpty = !_cards.Any ( );

        indicatorPanel.SetActive ( !isCardHolderEmpty );
        noCardsLeftPanel.SetActive ( isCardHolderEmpty );
    }

    private void FadeOverlappingCard ( ) 
    {
        if ( !_shouldFadeOverlappingCard ) 
            return;
        
        var topmostCard = InputManager.GetTopmostCard ( );
        var topmostCardIndex = _cards.IndexOf ( topmostCard );
        
        if ( topmostCard != null && _cards.IndexOf ( topmostCard ) >= 0 ) 
            for ( var index = 0; index < _cards.Count; index++ ) 
                _cards [ index ].FadeToggle ( index == topmostCardIndex + 1 );
    }

    private void FadeInAllCards ( ) 
    {
        if ( !_shouldFadeOverlappingCard ) 
            return;
        
        for ( var index = 0; index < _cards.Count; index++ ) 
            _cards [ index ].FadeToggle ( fadeOut: false );
    }

    #endregion


    private void FixedUpdate ( ) 
    {
        if ( _currentlyDraggedCard == null ) 
        {
            if ( InputManager.IsMouseOverCards ( ) ) 
            {
                HighLightCards ( );

                FadeOverlappingCard ( );
            }
            else 
            {
                UnHighlightCards ( );

                FadeInAllCards ( );
            }
        }
    }

    #endregion
}
