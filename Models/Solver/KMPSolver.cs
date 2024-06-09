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
        sol.SidikJari = result;
    }



    int KMPSearch(string pattern, string txt) {
        // longest prefix suffix values
        int[] lps = new int[pattern.Length];
        int j = 0;
 
        computeLPSArray(pattern, lps);
 
        int i = 0;
        while (i < txt.Length) {
            if (pattern[j] == txt[i]) {
                j++;
                i++;
            }
            if (j == pattern.Length) {
                return i-j;
            }
 
            // mismatch after j matches
            else if (i < txt.Length && pattern[j] != txt[i]) {
                if (j != 0) j = lps[j - 1];
                else i = i + 1;
            }
        }

        // Case not found
        return -1;
    }
 
    void computeLPSArray(string pattern, int[] lps) {
        // length of the previous longest prefix suffix
        int len = 0;
        int i = 1;
        lps[0] = 0; 

        while (i < pattern.Length) {
            if (pattern[i] == pattern[len]) {
                len++;
                lps[i] = len;
                i++;
            }
            else {
                if (len != 0) len = lps[len - 1];
                else {
                    lps[i] = len;
                    i++;
                }
            }
        }
    }

    public override string ToString() => "KMP";
}