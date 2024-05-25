
using System;

public class LevenshteinDistance {
    public static int Solve(string A, string B) {
        int ALength = A.Length;
        int BLength = B.Length;

        int[,] mat = new int[ALength + 1, BLength + 1];

        // Return full length if any empty
        if (ALength == 0) return BLength;
        if (BLength == 0) return ALength;

        // Init matrix
        for (int i = 0; i <= ALength; i++) {
            mat[i, 0] = i;
        }
        for (int j = 0; j <= BLength; j++) {
            mat[0, j] = j;
        }


        // rows, columns distances
        for (int i = 1; i <= ALength; i++) {
            for (int j = 1; j <= BLength; j++) {
                int cost = (B[j - 1] == A[i - 1]) ? 0 : 1;

                mat[i, j] = Math.Min(
                    Math.Min(mat[i - 1, j] + 1, mat[i, j - 1] + 1),
                    mat[i - 1, j - 1] + cost
                );
            }
        }
        return mat[ALength, BLength];
    }

}
