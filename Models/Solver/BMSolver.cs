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
 
    // The preprocessing function for Boyer Moore's
    // bad character heuristic
    static void BadCharHeuristic(string pat, int[] badchar)
    {
        int size = pat.Length;
        int i;
 
        // Initialize all occurrences as -1
        for (i = 0; i < 256; i++)
            badchar[i] = -1;
 
        // Fill the actual value of last occurrence
        // of a character
        for (i = 0; i < size; i++)
            badchar[(int)pat[i]] = i;
    }
 
    /* A pattern searching function that uses Bad
    Character Heuristic of Boyer Moore Algorithm */
    static int BMSearch(string txt, string pat)
    {
        int m = pat.Length;
        int n = txt.Length;
 
        int[] badchar = new int[256];
 
        /* Fill the bad character array by calling
            the preprocessing function badCharHeuristic()
            for given pattern */
        BadCharHeuristic(pat, badchar);
 
        int s = 0; // s is shift of the pattern with
                   // respect to text
        while (s <= (n - m)) {
            int j = m - 1;
 
            /* Keep reducing index j of pattern while
                characters of pattern and text are
                matching at this shift s */
            while (j >= 0 && pat[j] == txt[s + j])
                j--;
 
            /* If the pattern is present at current
                shift, then index j will become -1 after
                the above loop */
            if (j < 0) {
                return s;
 
                /* Shift the pattern so that the next
                    character in text aligns with the last
                    occurrence of it in pattern.
                    The condition s+m < n is necessary for
                    the case when pattern occurs at the end
                    of text */
            }
 
            else
                /* Shift the pattern so that the bad
                   character in text aligns with the last
                   occurrence of it in pattern. The max
                   function is used to make sure that we get
                   a positive shift. We may get a negative
                   shift if the last occurrence of bad
                   character in pattern is on the right side
                   of the current character. */
                s += (1 > j - badchar[txt[s+j]])? 1 :  j - badchar[txt[s+j]];
        }
        return -1;
    }

    public override string ToString() => "BM";

}