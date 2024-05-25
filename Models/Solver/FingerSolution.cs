using System.Diagnostics;

public class FingerSolution{

    public Biodata Biodata {
        get => _biodata;
        set => _biodata = value;
    }

    public SidikJari SidikJari {
        get => _sidikJari;
        set => _sidikJari = value;
    }

    public double PersentaseKecocokan {
        get => _persentaseKecocokan;
        set => _persentaseKecocokan = value;
    }

    Biodata _biodata;
    SidikJari _sidikJari;
    double _persentaseKecocokan;

    Stopwatch _stopWatch = new Stopwatch();
    public long ExecutionTime => _stopWatch.ElapsedMilliseconds;

    public FingerSolution StartTimer(){
        _stopWatch.Start();
        return this;
    }

    public FingerSolution StopTimer(){
        _stopWatch.Stop();
        return this;
    }
    
}