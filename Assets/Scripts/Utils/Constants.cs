using System;

public static class Constants 
{
    
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
    ShowCards,
    PlaceCard,
    ShowDialogue
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
    RandomKid
}