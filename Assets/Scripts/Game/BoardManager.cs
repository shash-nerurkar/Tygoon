using UnityEngine;

public class BoardManager : MonoBehaviour
{
    #region Methods

    private void Awake ( ) 
    {
        GameBoardDisplay.PlaceCardAction += PlaceCard;
    }

    private void OnDestroy ( ) 
    {
        GameBoardDisplay.PlaceCardAction -= PlaceCard;
    }
    
    private void PlaceCard ( Card card, int rowNumber ) 
    {
        
    }

    #endregion
}
