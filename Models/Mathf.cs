public static class Mathf
{
    public static double InverseLerp(double a, double b, double value)
    => (value - a) / (b - a);
}