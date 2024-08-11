using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public interface IEnemy
{
    public void Init ( CardData [ ] cardDatas );

    public void PlayCard ( Card [ ] playerPlacedCards, Card [ ] placedCards, Card [ ] attackingCards, out CardData cardData, out int rowNumber );


    #region Static

    public static List<int> GetEmptyRows ( Card [ ] cards ) {
        List<int> rows = new ( );

        rows.AddRange ( cards.Select ( ( card, index ) => ( card, index ) )
            .Where ( x => x.card == null )
            .Select ( x => x.index ) );

        return rows;
    }
      
    public static int GetRandomRow ( Dictionary<int, int> rowsToWeight = null ) 
    {
        if ( rowsToWeight == null ) 
            return -1;

        var randomValue = Random.Range ( 0, rowsToWeight.Values.Sum ( ) );
        var cursor = 0;
        foreach ( var rowToWeight in rowsToWeight ) 
        {
            cursor += rowToWeight.Value;
        
            if ( cursor >= randomValue )
                return rowToWeight.Key;
        }

        return -1;
    }

    #endregion
}
