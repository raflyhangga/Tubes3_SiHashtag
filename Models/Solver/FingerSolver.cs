using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;

public abstract class FingerSolver{

    /// <summary>
    /// Return the solution depending on the algorithm in FingerSolution
    /// </summary>
    /// <param name="sjInput">Input from user</param>
    /// <returns>FingerSolution</returns>
    public FingerSolution Solve(SidikJari sjInput){
        FingerSolution sol = new FingerSolution().StartTimer();
        ProcessCalculation(sjInput, AllSidikJari, ref sol);
        if(sol.SidikJari != null) sol.PersentaseKecocokan = 1;
        else SolveWithLevenstheinDistance(sjInput, AllSidikJari, ref sol);
        sol.Biodata = FindBiodata(sol.SidikJari);
        return sol.StopTimer();
    }


    // Case not found
    void SolveWithLevenstheinDistance(SidikJari sj, List<SidikJari> listSj, ref FingerSolution sol){
        double percentage = 0;
        int smallest = int.MaxValue;
        for(int i = 0; i < listSj.Count; i++) {
            int res = LevenshteinDistance.Solve(sj.Ascii, listSj[i].Ascii);
            if(smallest > res) {
                smallest = res;
                sol.SidikJari = listSj[i];
                percentage = ((double)res) / double.Max(sj.Ascii.Length, listSj[i].Ascii.Length);
            }
        }
        sol.PersentaseKecocokan = 1-percentage;
    }

    /// <summary>
    /// // Populate solution in FingerSolution
    /// </summary>
    /// <param name="sj">Input from user</param>
    /// <param name="listSj">List to search in database</param>
    /// <param name="sol">Solution itself</param>
    protected abstract void ProcessCalculation(SidikJari sj, List<SidikJari> listSj, ref FingerSolution sol);
    
    /// <summary>
    /// return all sidik jari
    /// </summary>
    /// <returns></returns>
    private static List<SidikJari> cachedSidikJariList = null;
    private static List<SidikJari> AllSidikJari => cachedSidikJariList ?? (cachedSidikJariList = SidikJari.GetAll()); 
    public static void Initialize() => cachedSidikJariList = SidikJari.GetAll(); 

    /// <summary>
    /// Find user data with regex because of bahasa alay stuff
    /// </summary>
    /// <param name="sj">The SidikJari found on database with ProcessCalculation</param>
    /// <returns></returns>
    private Biodata FindBiodata(SidikJari sj) {
        List<Biodata> biodataList = Biodata.GetAll();
        Biodata result = null;
        double _smallestDistance = double.MaxValue;
        foreach(Biodata biodata in biodataList){

            string bioName = biodata.Nama.FormatToAlay();
            string sjName = sj.Nama.FormatToAlay();
            
            // pure string compute
            double pureLevenshteinDistance = LevenshteinDistance.Solve(biodata.Nama, sj.Nama);
            double pureNormalized = pureLevenshteinDistance/(3*double.Max(biodata.Nama.Length, sj.Nama.Length));

            // corrupted + pure string compute
            double corruptedLevenshteinDistance = LevenshteinDistance.Solve(bioName, sjName);
            double corruptedNormalized = (corruptedLevenshteinDistance/(3*double.Max(bioName.Length, sjName.Length)) + pureNormalized) / 2;

            // result
            double minResult = double.Min(pureLevenshteinDistance, corruptedNormalized);
            if(_smallestDistance > minResult){
                _smallestDistance = minResult;
                result = biodata;
                if(_smallestDistance == 0){
                    return result;
                }
            }

        }
        return result;
    }
}