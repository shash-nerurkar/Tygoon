using System;
using System.Collections;
using System.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class Card : MonoBehaviour
{
        #region Fields

        [ SerializeField ] private TextMeshProUGUI damageLabel;

        [ SerializeField ] private TextMeshProUGUI description;

        [ SerializeField ] private TextMeshProUGUI damageLabel2;

        private Vector3 _basePosition;

        private IEnumerator _dragCoroutine;

        private bool _isDragging;

        #endregion


        #region Actions

        public static event Action<Card> PlacementValidityCheckAction;

        #endregion
        
        
        #region Methods

        private void Awake ( ) 
        {
            _basePosition = transform.position;
        }

        
        #region Public

        public void UpdateData ( CardData data ) 
        {
            damageLabel.text = data.Damage.ToString ( );
            description.text = data.Description;
            damageLabel2.text = data.Damage.ToString ( );
        }
        
        public void OnPlacementValidityChecked ( bool isPlacementValid ) 
        {
            if ( !isPlacementValid )
                MoveBackToBasePosition ( );
        }

        public void OnDragBegin ( ) 
        {
            if ( _isDragging ) return;
            _isDragging = true;

            transform.DOScale ( new Vector3 ( 0.75f, 0.75f, 0.75f), 0.05f );
            
            _dragCoroutine = Drag ( );
            StartCoroutine ( _dragCoroutine );
        }

        public void OnDragEnd ( ) 
        {
            if ( !_isDragging ) return;
            _isDragging = false;
            
            if ( _dragCoroutine != null ) 
                StopCoroutine ( _dragCoroutine );
            
            PlacementValidityCheckAction?.Invoke ( this );
        }
        
        #endregion

        
        private IEnumerator Drag ( ) 
        {
            while ( true ) 
            {
                transform.DOMove ( ( Vector2 ) Camera.main.ScreenToWorldPoint ( Input.mousePosition ), 0.05f );
                
                yield return null;
            }
        }

        private void MoveBackToBasePosition ( ) 
        {
            transform.DOMove ( _basePosition, 0.05f );
            transform.DOScale ( new Vector3 ( 1f, 1f, 1f), 0.05f );
        }

        #endregion
}
