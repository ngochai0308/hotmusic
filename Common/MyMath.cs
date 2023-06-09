namespace HotMusic.Common
{
    public class MyMath
    {
        public static int Sum(int a, int b)
        {
            return a + b;
        }
        public static int Sub(int a, int b)
        {
            return a - b;
        }

        public static int Mul(int a, int b)
        {
            return a * b;
        }
        public static double Div(int a, int b)
        {
            if (b == 0)
            {
                return double.MaxValue;
            }

            return (double)a / b;
        }
    }
}
