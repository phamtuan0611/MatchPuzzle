using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardLayOut : MonoBehaviour
{
    public LayOutRow[] allRows;

    public Gem[,] GetLayOut()
    {
        Gem[,] theLayOut = new Gem[allRows[0].gemsInRow.Length, allRows.Length];

        for (int y = 0; y < allRows.Length; y++)
        {
            for (int x = 0; x < allRows[y].gemsInRow.Length; x++)
            {
                if (x < theLayOut.GetLength(0))
                {
                    if (allRows[y].gemsInRow[x] != null)
                    {
                        theLayOut[x, allRows.Length - 1 - y] = allRows[y].gemsInRow[x];
                    }
                }
            }
        }

        return theLayOut;
    }
}

[System.Serializable]
public class LayOutRow 
{
    public Gem[] gemsInRow;

}

