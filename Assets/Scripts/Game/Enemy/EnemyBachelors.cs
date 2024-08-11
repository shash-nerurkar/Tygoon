using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;

public class EnemyBachelors : IEnemy
{
    #region Fields

    private List<CardData> _cardDatas;

    #endregion


    #region Methods

    public void Init ( CardData [ ] cardDatas ) => _cardDatas = cardDatas.ToList ( );

    public void PlayCard ( Card [ ] playerPlacedCards, Card [ ] placedCards, Card [ ] attackingCards, out CardData cardData, out int rowNumber ) 
    {
        var emptyPlacedRows = IEnemy.GetEmptyRows ( placedCards ).OrderBy ( x => Random.value ).ToList ( );

        if ( !emptyPlacedRows.Any ( ) || !_cardDatas.Any ( ) )
        {
            cardData = null;
            rowNumber = -1;
            return;
        }

        var emptyRowsToWeight = new Dictionary<int, int> ( );
        foreach ( int row in emptyPlacedRows ) 
            if ( playerPlacedCards [ row ] == null && attackingCards [ row ] == null ) 
                emptyRowsToWeight.Add ( row, 7 );
            else if ( playerPlacedCards [ row ] != null && attackingCards [ row ] == null ) 
                emptyRowsToWeight.Add ( row, 5 );
            else if ( playerPlacedCards [ row ] != null && attackingCards [ row ] != null ) 
                emptyRowsToWeight.Add ( row, 2 );
            else if ( playerPlacedCards [ row ] == null && attackingCards [ row ] != null ) 
                emptyRowsToWeight.Add ( row, 1 );
        
        rowNumber = IEnemy.GetRandomRow ( emptyRowsToWeight );
            
        var cardIndex = Random.Range ( 0, _cardDatas.Count );
        cardData = _cardDatas [ cardIndex ];
        _cardDatas.RemoveAt ( cardIndex );
    }

    #endregion
}
