using System;
using System.Collections.Generic;

public class BMSolver : FingerSolver{
    protected override void ProcessCalculation(SidikJari sj, List<SidikJari> listSj, ref FingerSolution sol){
        SidikJari result = null;
        for(int i = 0; i < listSj.Count; i++) {
            int res = BMSearch(sj.Ascii, listSj[i].Ascii);
            if(res != -1) {
                result = listSj[i];
                break;
            }
        }
        sol.SidikJari = result;
    }
 
    // last Occurence table preprocessing
    static void LastOccurenceCalculation(string pat, int[] lastOc)
    {
        int size = pat.Length;
        for (int i = 0; i < 256; i++)
            lastOc[i] = -1;
        for (int i = 0; i < size; i++)
            lastOc[(int)pat[i]] = i;
    }

    static int BMSearch(string pat, string txt)
    {
        int n = txt.Length;
        int m = pat.Length;

        // preprocessing pattern
        int[] lastOc = new int[256];
        LastOccurenceCalculation(pat, lastOc);

        int i = m - 1;
        int j = m - 1;
        while (i <= n - 1) {

            if(txt[i] == pat[j]){
                if(j == 0){
                    return i;
                }
                else{
                    i--;
                    j--;
                }
            } else{
                // lookup to last occurence table
                i = i + m - int.Min(j, 1 + lastOc[(int)txt[i]]); 
                j = m - 1;
            }
        }
        return -1;
    }

    public override string ToString() => "BM";

}