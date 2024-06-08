using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
        else SolveWithLevenstheinDistance(new SidikJari(sjInput.BerkasCitra, sjInput.Nama), AllSidikJari, ref sol);
        
        sol.Biodata = FindBiodata(sol.SidikJari);
        return sol.StopTimer();
    }


    // Case not found
    void SolveWithLevenstheinDistance(SidikJari sj, List<SidikJari> listSj, ref FingerSolution sol){
        double percentage = 0;
        int smallest = int.MaxValue;
        // for(int i = 0; i < listSj.Count; i++) {
        //     int res = LevenshteinDistance.Solve(sj.Ascii, listSj[i].Ascii);
        //     if(smallest > res) {
        //         smallest = res;
        //         sol.SidikJari = listSj[i];
        //         percentage = ((double)res) / double.Max(sj.Ascii.Length, listSj[i].Ascii.Length);
        //     }

        //     // Debug
        //     if(++i % 1000 == 0) Console.WriteLine("LevenshteinDistance: " + i + " / " + listSj.Count);
        // }

        // Paralel version
        int unsafeCounter = 0;
        int[] distances = new int[listSj.Count];
        Parallel.For(0, listSj.Count, new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount }, i => {
            distances[i] = LevenshteinDistance.Solve(sj.Ascii, listSj[i].Ascii);
            if(++unsafeCounter % 1000 == 0) Console.WriteLine("LevenshteinDistance: " + unsafeCounter + " / " + listSj.Count);
        });

        for(int i = 0; i < listSj.Count; i++) {
            if(distances[i] < smallest) {
                smallest = distances[i];
                sol.SidikJari = listSj[i];
                // percentage = ((double)distances[i]) / double.Max(sj.Ascii.Length, listSj[i].Ascii.Length); // case all pixel
                double bigger = double.Max(sj.Ascii.Length, listSj[i].Ascii.Length);
                double smaller = double.Min(sj.Ascii.Length, listSj[i].Ascii.Length);
                percentage = Mathf.InverseLerp(bigger - smaller, bigger, smallest);
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

            string bioName = biodata.Nama.ToCompareAlay();
            string sjName = sj.Nama.ToCompareAlay();
            
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