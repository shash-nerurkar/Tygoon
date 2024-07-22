using System;
using System.Collections.ObjectModel;
using UnityEngine;


[ CreateAssetMenu ( fileName = "DialogueSequenceData", menuName = "Game/Dialogue Sequence Data", order = 1 ) ]
public class DialogueSequenceData : ScriptableObject 
{
    #region Serialized Fields
    
    [ SerializeField ] private DialogueData [ ] openingDialogues;
    
    [ SerializeField ] private DialogueData [ ] closingDialoguesOnWin;
    
    [ SerializeField ] private DialogueData [ ] closingDialoguesOnLose;
    
    #endregion

    
    #region Public Fields
    
    public ReadOnlyCollection<DialogueData> OpeningDialogues => Array.AsReadOnly ( openingDialogues );
    
    public ReadOnlyCollection<DialogueData> ClosingDialoguesOnWin => Array.AsReadOnly ( closingDialoguesOnWin );
    
    public ReadOnlyCollection<DialogueData> ClosingDialoguesOnLose => Array.AsReadOnly ( closingDialoguesOnLose );
    
    #endregion
}
