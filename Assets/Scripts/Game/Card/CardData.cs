using UnityEngine;

[ CreateAssetMenu ( fileName = "CardData", menuName = "Game/Card Data", order = 1 ) ]
public class CardData : ScriptableObject
{
    #region Serialized Fields
    
    [ SerializeField ] private int damage;
    
    [ SerializeField ] private int health;
    
    [ SerializeField ] private string description;
    
    #endregion

    
    #region Public Fields
    
    public int Damage => damage;
    
    public int Health => health;
    
    public string Description => description;
    
    #endregion
}
