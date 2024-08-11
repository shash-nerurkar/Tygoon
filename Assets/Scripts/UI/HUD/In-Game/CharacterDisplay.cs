using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class CharacterDisplay : MonoBehaviour
{
    #region Fields

    [ SerializeField ] private GameObject characterPrefab;

    [ SerializeField ] private Transform leftContainerTransform;

    [ SerializeField ] private Transform rightContainerTransform;

    [ Header ( "Data" ) ]
    [ SerializeField ] private SerializedDictionary<InGameCharacter, CharacterData> characterDatas;

    private readonly List<Character> _characters = new ( );

    #endregion


    #region Methods

    public void AddCharacters ( InGameCharacter [ ] characterTypes ) 
    {
        foreach ( var characterType in characterTypes ) 
        {
            var characterObject = Instantiate ( characterPrefab, characterType.Equals ( InGameCharacter.Player ) ? leftContainerTransform : rightContainerTransform );

            var character = characterObject.GetComponent<Character> ( );
            character.Initialize ( characterDatas.ToDictionary ( ) [ characterType ] );

            _characters.Add ( character );
        }
    }

    public void ClearAllCharacters ( ) 
    {
        foreach ( var character in _characters ) 
            Destroy ( character.gameObject );
        
        _characters.Clear ( );
    }

    public void PlayCharacterDamageAnimation ( bool isPlayerDamaged ) 
    {
        if ( isPlayerDamaged ) 
            leftContainerTransform.DOShakePosition ( 1f, strength: new Vector3 ( 20, 20 ), vibrato: 10, randomness: 90, fadeOut: true );
        else 
            rightContainerTransform.DOShakePosition ( 1f, strength: new Vector3 ( 20, 20 ), vibrato: 10, randomness: 90, fadeOut: true );
    }

    #endregion
}


[ Serializable ]
public class CharacterData 
{
    [ SerializeField ] private Vector3 position;
    [ SerializeField ] private Vector3 size;
    [ SerializeField ] private Sprite sprite;
    [ SerializeField ] private string name;
    [ SerializeField ] private bool isOnPlayerSide;

    public Vector3 Position => position;
    public Vector3 Size => size;
    public Sprite Sprite => sprite;
    public string Name => name;
    public bool IsOnPlayerSide => isOnPlayerSide;
}
