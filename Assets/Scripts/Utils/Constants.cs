using System;
using UnityEngine;

public static class Constants 
{
    #region In Game Progress

    public static readonly int MaxProgress = 9;

    #endregion


    #region Card

    public static readonly Color EnemyCardColor = new ( 1f, 0.8f, 0.8f );

    public static readonly Color PlayerCardColor = new ( 0.8f, 1f, 0.8f );

    public static readonly Vector3 HighLightPlacedCardScale = new ( 1.5f, 1.5f, 1.5f );

    public static readonly Vector3 UnhighLightPlacedCardScale = new ( 1f, 1f, 1f );

    public static readonly Vector3 PlacedCardScale = new ( 1f, 1f, 1f );

    public static readonly Vector3 HeldCardScale = new ( 0.75f, 0.75f, 0.75f );

    public static readonly Vector3 InDeckCardScale = new ( 1f, 1f, 1f );

    public static readonly Vector3 CardHolderHidePosition = new ( 0f, -400f );

    public static readonly Vector3 CardHolderShowPosition = new ( 0f, 0f );

    #endregion
}


[ Serializable ]
public enum GameState 
{
    MainMenu,
    Cutscene,
    InGame
}


[ Serializable ]
public enum InGameState 
{
    None,
    DialogueShowing,
    EnemyPlayingCard,
    WaitingForPlayerInput,
    AllCardsAttacking
}


[ Serializable ]
public enum Cutscene 
{
    Pilot,
    GoingToBank,
    GoingToBuilding,
    GoingToConstructionSite,
    Outro
}


[ Serializable ]
public enum Level 
{
    GovernmentOffice,
    Bank,
    Building,
    ConstructionSite
}


[ Serializable ]
public enum InGameCharacter 
{
    Player,
    GovtEmployeeOne,
    GovtEmployeeTwo,
    BankHead,
    TenantOne,
    TenantTwo,
    TenantThree,
    RandomWoman,
    RandomMan,
    RandomKids,
    RandomMan2
}