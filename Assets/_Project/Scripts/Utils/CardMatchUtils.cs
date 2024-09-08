using System.Collections.Generic;
using UnityEngine;

namespace CardMatch.Utils
{
    public static class CardMatchUtils
    {
        public static void Shuffle<T>(this List<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = Random.Range(0, n + 1);
                (list[k], list[n]) = (list[n], list[k]);
            }
        }
        
        public static (int rows, int columns) GetMostSquareLayout(int totalCards, int maxColumns)
        {
            int bestRows = 1;
            int bestColumns = totalCards;
            double bestAspectRatio = double.MaxValue;

            for (int columns = 1; columns <= maxColumns; columns++)
            {
                if (totalCards % columns == 0)
                {
                    int rows = totalCards / columns;
                    double aspectRatio = Mathf.Abs(rows / (float)columns - 1);

                    if (aspectRatio < bestAspectRatio)
                    {
                        bestAspectRatio = aspectRatio;
                        bestRows = rows;
                        bestColumns = columns;
                    }
                }
            }

            return (bestRows, bestColumns);
        }
    }
}