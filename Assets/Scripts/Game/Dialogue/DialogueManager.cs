using System;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    #region Actions

    public static event Action<DialogueSequenceData, bool> StartDialogueSequenceAction;

    #endregion


    #region Fields

    [ SerializeField ] private SerializedDictionary<Level, DialogueSequenceData> dialogueSequenceDatas;

    #endregion


    #region Methods

    private void Awake ( ) 
    {
        GameManager.OnLevelStartAction += StartOpeningDialogueSequence;

        BoardManager.OnPlayerWinAction += StartClosingDialogueSequence;
    }
    
    private void OnDestroy ( ) 
    {
        GameManager.OnLevelStartAction -= StartOpeningDialogueSequence;

        BoardManager.OnPlayerWinAction -= StartClosingDialogueSequence;
    }
    
    public void StartOpeningDialogueSequence ( Level level ) 
    {
        GameStateManager.ChangeInGameState ( InGameState.DialogueShowing );

        StartDialogueSequenceAction?.Invoke ( dialogueSequenceDatas.ToDictionary ( ) [ level ], true );
    }
    
    public void StartClosingDialogueSequence ( Level level ) 
    {
        GameStateManager.ChangeInGameState ( InGameState.DialogueShowing );

        StartDialogueSequenceAction?.Invoke ( dialogueSequenceDatas.ToDictionary ( ) [ level ], false );
    }

    #endregion
}
