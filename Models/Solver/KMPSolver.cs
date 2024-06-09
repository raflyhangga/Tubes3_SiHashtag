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



    int KMPSearch(string pat, string txt) {
        int M = pat.Length;
        int N = txt.Length;
 
        // longest prefix suffix values
        int[] lps = new int[M];
        int j = 0;
 
  
        computeLPSArray(pat, M, lps);
 
        int i = 0;
        while (i < N) {
            if (pat[j] == txt[i]) {
                j++;
                i++;
            }
            if (j == M) {
                return i-j;
            }
 
            // mismatch after j matches
            else if (i < N && pat[j] != txt[i]) {
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
        lps[0] = 0; 

        while (i < M) {
            if (pat[i] == pat[len]) {
                len++;
                lps[i] = len;
                i++;
            }
            else {
                if (len != 0) {
                    len = lps[len - 1];
                }
                else {
                    lps[i] = len;
                    i++;
                }
            }
        }
    }

    public override string ToString() => "KMP";
}