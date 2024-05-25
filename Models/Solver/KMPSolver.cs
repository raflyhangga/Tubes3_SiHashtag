using System;
using System.Collections.Generic;

public class KMPSolver : FingerSolver{
    protected override void ProcessCalculation(SidikJari sj, List<SidikJari> listSj, ref FingerSolution sol){
        SidikJari result = null;
        for(int i = 0; i < listSj.Count; i++) {
            int res = KMPSearch(sj.Ascii, listSj[i].Ascii);
            if(res != -1) {
                result = listSj[i];
                break;
            }
        }

        // Case not found
        if(result == null) {
            //// TODO: LevenshteinDistance

            double percentage = 0;
            for(int i = 0; i < listSj.Count; i++) {
                LevenshteinDistance.Solve("", "");
            }
            //// The above is unfinished yet
            sol.PersentaseKecocokan = percentage;
        } else {
            sol.PersentaseKecocokan = 1;
        }

        sol.SidikJari = result;
    }



    int KMPSearch(string pat, string txt) {
        int M = pat.Length;
        int N = txt.Length;
 
        // create lps[] that will hold the longest
        // prefix suffix values for pattern
        int[] lps = new int[M];
        int j = 0; // index for pat[]
 
        // Preprocess the pattern (calculate lps[]
        // array)
        computeLPSArray(pat, M, lps);
 
        int i = 0; // index for txt[]
        while (i < N) {
            if (pat[j] == txt[i]) {
                j++;
                i++;
            }
            if (j == M) {
                // Console.Write("Found pattern " + "at index " + (i - j));
                return i-j;
                // j = lps[j - 1];
            }
 
            // mismatch after j matches
            else if (i < N && pat[j] != txt[i]) {
                // Do not match lps[0..lps[j-1]] characters,
                // they will match anyway
                if (j != 0)
                    j = lps[j - 1];
                else
                    i = i + 1;
            }
        }

        // Case not found
        return -1;
    }
 
    void computeLPSArray(string pat, int M, int[] lps) {
        // length of the previous longest prefix suffix
        int len = 0;
        int i = 1;
        lps[0] = 0; // lps[0] is always 0
 
        // the loop calculates lps[i] for i = 1 to M-1
        while (i < M) {
            if (pat[i] == pat[len]) {
                len++;
                lps[i] = len;
                i++;
            }
            // (pat[i] != pat[len])
            else {
                // This is tricky. Consider the example.
                // AAACAAAA and i = 7. The idea is similar
                // to search step.
                if (len != 0) {
                    len = lps[len - 1];
 
                    // Also, note that we do not increment
                    // i here
                }
                // if (len == 0)
                else {
                    lps[i] = len;
                    i++;
                }
            }
        }
    }
}