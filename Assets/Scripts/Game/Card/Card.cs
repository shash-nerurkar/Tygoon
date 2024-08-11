using System;
using System.Collections;
using System.Threading.Tasks;
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


        #region Serialized

        [ SerializeField ] private TextMeshProUGUI titleLabel;

        [ SerializeField ] private TextMeshProUGUI descriptionLabel;

        [ SerializeField ] private TextMeshProUGUI healthLabel;

        [ SerializeField ] private Image healthIcon;

        [ SerializeField ] private TextMeshProUGUI damageLabel;

        [ SerializeField ] private Image damageIcon;

        [ Header ( "Self" ) ]
        [ SerializeField ] private Image image;

        [ SerializeField ] private RectTransform rectTransform;

        [ SerializeField ] private LayoutElement layoutElement;

        #endregion


        #region Public
        
        public int Damage { get; private set; }

        public int Health { get; private set; }

        public bool IsDragging { get; private set; }

        public bool IsPlaced { get; private set; }

        public bool IsEnemyCard { get; private set; }

        #endregion


        private Vector3 _basePosition;

        private IEnumerator _dragCoroutine;

        private Transform _boardParent;

        #endregion


        #region Methods

        
        #region Public

        public void Initialize ( CardData data, bool isEnemyCard ) 
        {
            titleLabel.text = data.Title;
            descriptionLabel.text = data.Description;
            damageLabel.text = data.Damage.ToString ( );
            healthLabel.text = data.Health.ToString ( );

            Damage = data.Damage;
            Health = data.Health;

            IsEnemyCard = isEnemyCard;

            if ( IsEnemyCard ) 
            {
                image.color = Constants.EnemyCardColor;

                layoutElement.enabled = true;
                layoutElement.ignoreLayout = true;
            }
            else 
            {
                image.color = Constants.PlayerCardColor;
            }
        }

        public void SetCurrentAsBasePosition ( ) 
        {
            _basePosition = transform.localPosition;
        }

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

        public void BeginDrag ( ) 
        {
            if ( IsDragging ) 
                return;
            IsDragging = true;

            transform.DOScale ( Constants.HeldCardScale, 0.05f );
            
            _dragCoroutine = Drag ( );
            StartCoroutine ( _dragCoroutine );
        }

        public void EndDrag ( ) 
        {
            if ( !IsDragging ) 
                return;
            IsDragging = false;
            
            if ( _dragCoroutine != null ) 
                StopCoroutine ( _dragCoroutine );
            
            PlacementValidityCheckAction?.Invoke ( this );
        }
       
        public async void Place ( RectTransform spaceRectTransform, bool isEnemyCard, bool isMoving ) 
        {
            transform.SetParent ( spaceRectTransform, true );

            layoutElement.enabled = true;
            layoutElement.ignoreLayout = true;

            rectTransform.DOScale ( Constants.PlacedCardScale, 0.3f );

            var cardNewWidth = spaceRectTransform.rect.height - 20;
            rectTransform.DOSizeDelta ( new Vector3 ( cardNewWidth, cardNewWidth * ( rectTransform.sizeDelta.y / rectTransform.sizeDelta.x ) ), 0.3f );

            rectTransform.DORotate ( new Vector3 ( 0, 0, isEnemyCard ? 90 : -90 ), 0.3f );


            if ( !isMoving ) 
            {
                SoundManager.Instance.Play ( SoundType.OnCardPlaced );
                await Task.Delay ( 100 );
            }

            rectTransform.anchoredPosition = Vector3.zero;
            rectTransform.localPosition = Vector3.zero;
        }

        public void OnPlaced ( ) 
        {
            IsPlaced = true;
        }
 
        public void BeginZoom ( Transform newParent ) 
        {
            var originalPosition = transform.position;
            _boardParent = transform.parent;
            transform.SetParent ( newParent, true );
            transform.position = originalPosition;
            
            transform.DOScale ( Constants.HighLightPlacedCardScale, 0.05f );
            transform.DORotate ( new Vector3 ( 0, 0, 0 ), 0.05f );
        }

        public void EndZoom ( ) 
        {
            var originalPosition = transform.position;
            transform.SetParent ( _boardParent, true );
            _boardParent = null;
            transform.position = originalPosition;

            transform.DOScale ( Constants.UnhighLightPlacedCardScale, 0.05f );
            transform.DORotate ( new Vector3 ( 0, 0, IsEnemyCard ? 90 : -90 ), 0.05f );
        }
        
        public void Despawn ( ) 
        {
            var despawnAnimSpeed = 0.5f;

            titleLabel.DOColor ( new Color ( 0, 0, 0, 0 ), despawnAnimSpeed );

            damageLabel.DOColor ( new Color ( 0, 0, 0, 0 ), despawnAnimSpeed );
            damageIcon.DOColor ( new Color ( 0, 0, 0, 0 ), despawnAnimSpeed );

            healthLabel.DOColor ( new Color ( 0, 0, 0, 0 ), despawnAnimSpeed );
            healthIcon.DOColor ( new Color ( 0, 0, 0, 0 ), despawnAnimSpeed );

            descriptionLabel.DOColor ( new Color ( 0, 0, 0, 0 ), despawnAnimSpeed );

            image.DOColor ( new Color ( 0, 0, 0, 0 ), despawnAnimSpeed * 2f )
                .OnComplete ( ( ) => { 
                    Destroy ( gameObject );
                } );
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
