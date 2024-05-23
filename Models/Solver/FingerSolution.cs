using System.Diagnostics;

public class FingerSolution{
    Biodata _biodata;
    public Biodata Biodata => _biodata;

    double _persentaseKecocokan;
    public double PersentaseKecocokan => _persentaseKecocokan;

    Stopwatch _stopWatch = new Stopwatch();
    public long ExecutionTime => _stopWatch.ElapsedMilliseconds;

    public FingerSolution SetBiodata(Biodata biodata){
        _biodata = biodata;
        return this;
    }

    public FingerSolution SetPersentaseKecocokan(double persentaseKecocokan){
        _persentaseKecocokan = persentaseKecocokan;
        return this;
    }

    public FingerSolution StartTimer(){
        _stopWatch.Start();
        return this;
    }

    public FingerSolution StopTimer(){
        _stopWatch.Stop();
        return this;
    }
    
}