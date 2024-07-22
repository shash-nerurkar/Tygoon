using System;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    #region Actions

    public static event Action<InGameCharacter [ ]> GenerateLevelCharactersAction;

    public static event Action DestroyLevelCharactersAction;

    #endregion


    #region Fields

    [ SerializeField ] private SerializedDictionary<Level, InGameCharacter [ ]> levelCharacters;

    #endregion


    #region Methods

    private void Awake ( ) 
    {
        GameManager.OnLevelStartAction += OnLevelStart;
        GameManager.ClearLevelDataAction += ClearLevelData;
    }

    private void OnDestroy ( ) 
    {
        GameManager.OnLevelStartAction -= OnLevelStart;
        GameManager.ClearLevelDataAction -= ClearLevelData;
    }

    public void OnLevelStart ( Level level ) 
    {
        GenerateLevelCharactersAction?.Invoke ( levelCharacters.ToDictionary ( ) [ level ] );
    }

    public void ClearLevelData ( ) 
    {
        DestroyLevelCharactersAction?.Invoke ( );
    }

    #endregion
}
