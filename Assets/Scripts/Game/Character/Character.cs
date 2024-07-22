using UnityEngine;
using UnityEngine.UI;

public class Character : MonoBehaviour
{
    #region Fields

    [ SerializeField ] private RectTransform rectTransform;

    [ SerializeField ] private Image portrait;

    #endregion

    
    #region Methods

    public void Initialize ( CharacterData data ) 
    {
        rectTransform.anchoredPosition = data.Position;
        rectTransform.sizeDelta = data.Size;
        portrait.sprite = data.Sprite;
    }

    #endregion
}
