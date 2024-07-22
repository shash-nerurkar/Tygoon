using System;
using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    #region Actions

    public static event Action<CardData [ ]> AddPlayerCardsAction;

    public static event Action ClearAllCardsAction;

    #endregion
    
    
    #region Fields

    [ SerializeField ] private SerializedDictionary<Level, CardData [ ]> levelInitialPlayerCards;

    #endregion


    #region Methods

    private void Awake ( ) 
    {
        BoardManager.GeneratePlayerInitialCardsAction += AddLevelCards;

        GameManager.ClearLevelDataAction += ClearAllCards;
    }

    private void OnDestroy ( ) 
    {
        BoardManager.GeneratePlayerInitialCardsAction -= AddLevelCards;

        GameManager.ClearLevelDataAction -= ClearAllCards;
    }

    private void AddLevelCards ( Level level ) 
    {
        AddPlayerCardsAction?.Invoke ( levelInitialPlayerCards.ToDictionary ( ) [ level ] );
    }

    private void ClearAllCards ( ) 
    {
        ClearAllCardsAction?.Invoke ( );
    }

    #endregion
}
