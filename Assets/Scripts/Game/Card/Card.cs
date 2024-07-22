using System;
using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
        #region Actions

        public static event Action<Card> PlacementValidityCheckAction;

        public static event Action<Card> RemoveAction;

        #endregion
        
        
        #region Fields

        [ SerializeField ] private Image image;

        [ SerializeField ] private TextMeshProUGUI titleLabel;

        [ SerializeField ] private TextMeshProUGUI damageLabel;

        [ SerializeField ] private Image damageIcon;

        [ SerializeField ] private TextMeshProUGUI healthLabel;

        [ SerializeField ] private Image healthIcon;

        [ SerializeField ] private TextMeshProUGUI descriptionLabel;

        private Vector3 _basePosition;

        private IEnumerator _dragCoroutine;

        private bool _isDragging;

        private bool _isPlaced;

        private bool _isEnemyCard;
        
        public int Damage { get; private set; }

        public int Health { get; private set; }

        #endregion


        #region Methods

        
        #region Public

        public void UpdateData ( CardData data, bool isEnemyCard ) 
        {
            titleLabel.text = data.Title;
            descriptionLabel.text = data.Description;
            damageLabel.text = data.Damage.ToString ( );
            healthLabel.text = data.Health.ToString ( );

            Damage = data.Damage;
            Health = data.Health;

            _isEnemyCard = isEnemyCard;

            image.color = _isEnemyCard ? Constants.EnemyCardColor : Constants.PlayerCardColor;
        }

        public void SetCurrentAsBasePosition ( ) => _basePosition = transform.localPosition;

        public void SetOpacity ( float a ) 
        {
            image.color = new Color ( image.color.r, image.color.g, image.color.b, a );

            titleLabel.color = new Color ( titleLabel.color.r, titleLabel.color.g, titleLabel.color.b, ( int ) a );

            damageLabel.color = new Color ( damageLabel.color.r, damageLabel.color.g, damageLabel.color.b, ( int ) a );
            damageIcon.color = new Color ( damageIcon.color.r, damageIcon.color.g, damageIcon.color.b, a );

            healthLabel.color = new Color ( healthLabel.color.r, healthLabel.color.g, healthLabel.color.b, ( int ) a );
            healthIcon.color = new Color ( healthIcon.color.r, healthIcon.color.g, healthIcon.color.b, a );

            descriptionLabel.color = new Color ( descriptionLabel.color.r, descriptionLabel.color.g, descriptionLabel.color.b, ( int ) a );
        }

        public void Place ( ) => _isPlaced = true;

        public void UpdateHealth ( int damageDealt ) 
        {
            Health = Mathf.Clamp ( Health - damageDealt, 0, int.MaxValue );
            healthLabel.text = Health.ToString ( );

            if ( Health == 0 ) 
                RemoveAction?.Invoke ( this );
        }

        public void UpdateDamage ( int damageReduced ) 
        {
            Damage = Mathf.Clamp ( Damage - damageReduced, 0, int.MaxValue );
            damageLabel.text = Damage.ToString ( );
        }
        
        public void OnPlacementValidityChecked ( bool isPlacementValid ) 
        {
            if ( !isPlacementValid )
                MoveBackToBasePosition ( );
        }

        public void OnDragBegin ( ) 
        {
            if ( _isPlaced ) 
            {
                transform.DOScale ( Constants.HighLightPlacedCardScale, 0.05f );
                transform.DORotate ( new Vector3 ( 0, 0, 0 ), 0.05f );
            }
            else 
            {
                if ( _isDragging ) return;
                _isDragging = true;

                transform.DOScale ( Constants.HeldCardScale, 0.05f );
                
                _dragCoroutine = Drag ( );
                StartCoroutine ( _dragCoroutine );
            }
        }

        public void OnDragEnd ( ) 
        {
            if ( _isPlaced ) 
            {
                transform.DOScale ( Constants.UnhighLightPlacedCardScale, 0.05f );
                transform.DORotate ( new Vector3 ( 0, 0, _isEnemyCard ? 90 : -90 ), 0.05f );
            }
            else 
            {
                if ( !_isDragging ) return;
                _isDragging = false;
                
                if ( _dragCoroutine != null ) 
                    StopCoroutine ( _dragCoroutine );
                
                PlacementValidityCheckAction?.Invoke ( this );
            }
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
            transform.DOLocalMove ( _basePosition, 0.05f );
            transform.DOScale ( Constants.InDeckCardScale, 0.05f );
        }

        #endregion
}
