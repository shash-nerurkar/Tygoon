using DG.Tweening;
using UnityEngine;

public class BoardSpaceIndicator : MonoBehaviour
{
    #region Fields

    [ SerializeField ] private RectTransform rectTransform;

    #endregion


    #region Methods

    private void Start ( ) => transform.DOScale ( 1.1f, 0.5f ).SetLoops ( -1, LoopType.Yoyo ).SetEase ( Ease.InOutSine );

    public void Initialize ( Vector3 sizeDelta, Vector3 position ) 
    {
        rectTransform.sizeDelta = sizeDelta;
        rectTransform.position = position;
    }

    #endregion
}
