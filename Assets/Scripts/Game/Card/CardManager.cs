using System;
using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    #region Actions

    public static event Action<List<CardData>> AddPlayerCardsAction;

    #endregion
    
    
    #region Fields

    [ SerializeField ] private SerializedDictionary<Level, List<CardData>> levelInitialPlayerCards;

    #endregion


    #region Methods

    private void Awake ( ) 
    {
        GameManager.OnLevelStartAction += AddLevelCards;
    }

    private void OnDestroy ( ) 
    {
        GameManager.OnLevelStartAction -= AddLevelCards;
    }

    private void AddLevelCards ( Level level ) 
    {
        AddPlayerCardsAction?.Invoke ( levelInitialPlayerCards.ToDictionary ( ) [ level ] );
    }

    #endregion
}
