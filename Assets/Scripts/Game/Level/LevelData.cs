using System;
using System.Collections.ObjectModel;
using UnityEngine;


[ CreateAssetMenu ( fileName = "LevelData", menuName = "Game/Level Data", order = 1 ) ]
public class LevelData : ScriptableObject 
{
    #region Serialized Fields

    [ SerializeField ] private Level level;
    
    [ SerializeField ] private DialogueData [ ] openingDialogues;
    
    [ SerializeField ] private DialogueData [ ] closingDialoguesOnWin;
    
    [ SerializeField ] private DialogueData [ ] closingDialoguesOnLose;
    
    #endregion

    
    #region Public Fields
    
    public Level Level => level;
    
    public ReadOnlyCollection<DialogueData> OpeningDialogues => Array.AsReadOnly ( openingDialogues );
    
    public ReadOnlyCollection<DialogueData> ClosingDialoguesOnWin => Array.AsReadOnly ( closingDialoguesOnWin );
    
    public ReadOnlyCollection<DialogueData> ClosingDialoguesOnLose => Array.AsReadOnly ( closingDialoguesOnLose );
    
    #endregion
}
