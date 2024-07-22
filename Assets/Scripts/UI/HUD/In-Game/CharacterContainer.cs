using System;
using System.Collections.Generic;
using UnityEngine;

public class CharacterContainer : MonoBehaviour
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

    #endregion
}


[ Serializable ]
public class CharacterData 
{
    [ SerializeField ] private Vector3 position;
    [ SerializeField ] private Vector3 size;
    [ SerializeField ] private Sprite sprite;

    public Vector3 Position => position;
    public Vector3 Size => size;
    public Sprite Sprite => sprite;
}
