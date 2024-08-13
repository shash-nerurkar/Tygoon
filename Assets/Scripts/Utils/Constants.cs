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

    public static readonly Color NeutralCardColor = new ( 1f, 1f, 1f );

    public static readonly Vector3 HighLightPlacedCardScale = new ( 1.5f, 1.5f, 1.5f );

    public static readonly Vector3 UnhighLightPlacedCardScale = new ( 1f, 1f, 1f );

    public static readonly Vector3 PlacedCardScale = new ( 1f, 1f, 1f );

    public static readonly Vector3 HeldCardScale = new ( 0.75f, 0.75f, 0.75f );

    public static readonly Vector3 InDeckCardScale = new ( 1f, 1f, 1f );

    #endregion


    #region Card Holder

    public static readonly Vector3 CardHolderHighlightedPosition = new ( 0f, 0f );

    public static readonly Vector3 CardHolderDisplayPosition = new ( 0f, -300f );

    public static readonly Vector3 CardHolderUnHighlightedPosition = new ( 0f, -525f );

    public static readonly Vector3 CardHolderHiddenPosition = new ( 0f, -600f );

    #endregion


    #region Transition

    public static readonly Color TransitionFadeInColor = new ( 0f, 0f, 0f, 1f );

    public static readonly Color TransitionFadeOutColor = new ( 0f, 0f, 0f, 0f );

    #endregion

    
    #region Board Display

    public static readonly Color CardDamagedColor = new ( 1f, 0.5f, 0f );

    public static readonly Color PlayerDamagedColor = new ( 1f, 0f, 0f );

    public static readonly Color EnemyDamagedColor = new ( 0f, 1f, 0f );

    #endregion
    

    #region Progress Bar

    public static readonly Color ProgressBarNegativeColor = new ( 1f, 0f, 0f );

    public static readonly Color ProgressBarPositiveColor = new ( 0f, 1f, 0f );

    public static readonly Color ProgressBarNeutralColor = new ( 1f, 1f, 1f );

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


[ Serializable ]
public enum SoundType
{
    OnUIClicked,
    OnDialoguePopped,
    OnCutscenePopped,
    OnCardPlaced,
    OnCardAttack
}


[ Serializable ]
public enum MusicType
{
    MainMenu,
    Fight
}