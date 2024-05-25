using System;
using System.Collections.Generic;

public abstract class FingerSolver{

    /// <summary>
    /// Return the solution depending on the algorithm in FingerSolution
    /// </summary>
    /// <param name="sj">Input from user</param>
    /// <returns>FingerSolution</returns>
    public FingerSolution Solve(SidikJari sj){
        FingerSolution sol = new FingerSolution().StartTimer();
        ProcessCalculation(sj, GetAllSidikJari(), ref sol);
        Biodata biodata = FindBiodata(sol.SidikJari);
        sol.Biodata = biodata;
        return sol.StopTimer();
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
    private List<SidikJari> GetAllSidikJari(){
        return SidikJari.GetAll();;
    }

    /// <summary>
    /// Find user data with regex because of bahasa alay stuff
    /// </summary>
    /// <param name="sj">The SidikJari found on database with ProcessCalculation</param>
    /// <returns></returns>
    private Biodata FindBiodata(SidikJari sj) {
        //// TODO: regex bahasa alay
        return null;
    }
}