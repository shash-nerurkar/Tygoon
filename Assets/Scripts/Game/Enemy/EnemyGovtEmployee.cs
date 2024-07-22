using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;

public class EnemyGovtEmployee : IEnemy
{
    #region Fields

    private List<CardData> _cardDatas;

    private List<int> _placeableRows;

    #endregion


    #region Methods

    public void UpdateData ( bool isInitial, CardData[] cardDatas, List<int> placeableRows ) 
    {
        if ( isInitial ) 
        {
            _cardDatas = cardDatas.ToList ( );
        }
        else 
        {
            if ( !_cardDatas.Any ( ) ) 
                _cardDatas = cardDatas.ToList ( );
        }

        _placeableRows = placeableRows;
    }

    public void PlayCard ( out CardData cardData, out int rowNumber ) 
    {
        if ( _placeableRows == null ) 
        {
            cardData = null;
            rowNumber = -1;
        }
        else 
        {
            rowNumber = _placeableRows [ Random.Range ( 0, _placeableRows.Count ) ];
            
            var cardIndex = Random.Range ( 0, _cardDatas.Count );
            cardData = _cardDatas [ cardIndex ];
            _cardDatas.RemoveAt ( cardIndex );
        }
    }

    #endregion
}
