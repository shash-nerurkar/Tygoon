using System.Collections.Generic;

public interface IEnemy
{
    public void UpdateData ( bool isInitial, CardData [ ] cardDatas, List<int> placeableRows );

    public void PlayCard ( out CardData cardData, out int rowNumber );
}
