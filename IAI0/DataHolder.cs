using System;
using System.Collections.Generic;
using System.Text;

namespace NOT_MIT_KAN_0
{
    //The area of triangle as function of coordinate of vertices
    class Formula4
    {
        Random _rnd = new Random();

        private double Function(double x1, double y1, double x2, double y2, double x3, double y3)
        {
            double X1 = Math.Abs(x1 - x2);
            double Y1 = Math.Abs(y1 - y2);
            double X2 = Math.Abs(x1 - x3);
            double Y2 = Math.Abs(y1 - y3);
            double X3 = Math.Abs(x2 - x3);
            double Y3 = Math.Abs(y2 - y3);

            double a = Math.Sqrt(X1 * X1 + Y1 * Y1);
            double b = Math.Sqrt(X2 * X2 + Y2 * Y2);
            double c = Math.Sqrt(X3 * X3 + Y3 * Y3);
            double s = (a + b + c) / 2.0;
            double rtn = Math.Sqrt(s * (s - a) * (s - b) * (s - c));
            return rtn;
        }

        private double GetValue()
        {
            double v = _rnd.Next(10, 1000) / 1000.0 * 100.0;
            return v;
        }

        public double[] GetInput()
        {
            double x1 = GetValue();
            double x2 = GetValue();
            double x3 = GetValue();
            double y1 = GetValue();
            double y2 = GetValue();
            double y3 = GetValue();

            return new double[] { x1, y1, x2, y2, x3, y3 };
        }

        public double GetTarget(double[] input)
        {
            return Function(input[0], input[1], input[2], input[3], input[4], input[5]);
        }

        public (List<double[]> input, List<double> target) GenerateData(int N)
        {
            List<double[]> inputs = new List<double[]>();
            List<double> target = new List<double>();

            int counter = 0;
            while (true)
            {
                double[] x = GetInput();
                double S = Function(x[0], x[1], x[2], x[3], x[4], x[5]);
                if (!Double.IsNaN(S))
                {
                    inputs.Add(new double[] { x[0], x[1], x[2], x[3], x[4], x[5] });
                    target.Add(S);
                    if (++counter >= N) break;
                }
            }

            return (inputs, target);
        }
    }

    //Mike's formula
    class Formula3
    {
        Random _rnd = new Random();

        private double Function(double[] x)
        {
            //y = (1/pi)*(2+2*x3)*(1/3)*(atan(20*exp(x5)*(x1-0.5+x2/6))+pi/2) + (1/pi)*(2+2*x4)*(1/3)*(atan(20*exp(x5)*(x1-0.5-x2/6))+pi/2);
            double pi = 3.14159265359;
            if (5 != x.Length)
            {
                Console.WriteLine("Formala error");
                Environment.Exit(0);
            }
            double y = (1.0 / pi);
            y *= (2.0 + 2.0 * x[2]);
            y *= (1.0 / 3.0);
            y *= Math.Atan(20.0 * Math.Exp(x[4]) * (x[0] - 0.5 + x[1] / 6.0)) + pi / 2.0;

            double z = (1.0 / pi);
            z *= (2.0 + 2.0 * x[3]);
            z *= (1.0 / 3.0);
            z *= Math.Atan(20.0 * Math.Exp(x[4]) * (x[0] - 0.5 - x[1] / 6.0)) + pi / 2.0;

            return y + z;
        }

        public double[] GetInput()
        {
            double[] x = new double[5];
            x[0] = (_rnd.Next() % 100) / 100.0;
            x[1] = (_rnd.Next() % 100) / 100.0;
            x[2] = (_rnd.Next() % 100) / 100.0;
            x[3] = (_rnd.Next() % 100) / 100.0;
            x[4] = (_rnd.Next() % 100) / 100.0;
            return x;
        }

        public double GetTarget(double[] input)
        {
            return Function(input);
        }

        public (List<double[]> input, List<double> target) GenerateData(int N)
        {
            List<double[]> inputs = new List<double[]>();
            List<double> target = new List<double>();

            for (int i = 0; i < N; ++i)
            {
                double[] x = GetInput();
                double t = GetTarget(x);

                inputs.Add(x);
                target.Add(t);
            }

            return (inputs, target);
        }
    }

    class Formula2
    {
        const double pi = 3.141592653589793;
        private Random _rnd = new Random();
        double _xmin = -1.0;
        double _xmax = 1.0;

        private double Function(double x1, double x2, double x3, double x4)
        {
            //example from https://kindxiaoming.github.io/pykan/Examples/Example_2_deep_formula.html
            //torch.exp((torch.sin(torch.pi*(x[:,[0]]**2+x[:,[1]]**2))+torch.sin(torch.pi*(x[:,[2]]**2+x[:,[3]]**2)))/2)
            return Math.Exp((Math.Sin(pi * (x1 * x1 + x2 * x2)) + Math.Sin(pi * (x3 * x3 + x4 * x4))) / 2.0);
        }

        public (List<double[]> input, List<double> target) GenerateData(int N)
        {
            List<double[]> input = new List<double[]>();
            List<double> target = new List<double>();

            for (int i = 0; i < N; ++i)
            {
                double[] arg = GetInput();
                input.Add(arg);
                target.Add(GetTarget(arg));
            }

            return (input, target);
        }

        public double[] GetInput()
        {
            double arg1 = _rnd.Next(10, 1000) / 1000.0 * (_xmax - _xmin) + _xmin;
            double arg2 = _rnd.Next(10, 1000) / 1000.0 * (_xmax - _xmin) + _xmin;
            double arg3 = _rnd.Next(10, 1000) / 1000.0 * (_xmax - _xmin) + _xmin;
            double arg4 = _rnd.Next(10, 1000) / 1000.0 * (_xmax - _xmin) + _xmin;
            return new double[] { arg1, arg2, arg3, arg4 };
        }

        public double GetTarget(double[] input)
        {
            return Function(input[0], input[1], input[2], input[3]);
        }
    }

    class Formula1
    {
        private Random _rnd = new Random();
        double _min = -1.0;
        double _max = 1.0;
        const double pi = 3.141592653589793;

        private double Function(double x, double y)
        {
            return Math.Exp(Math.Sin(pi * x) + y * y);
        }

        public (List<double[]> input, List<double> target) GenerateData(int N)
        {
            List<double[]> input = new List<double[]>();
            List<double> target = new List<double>();

            for (int i = 0; i < N; ++i)
            {
                double[] arg = GetInput();
                input.Add(arg);
                target.Add(GetTarget(input[i]));
            }
            return (input, target);
        }

        public double[] GetInput()
        {
            double arg1 = _rnd.Next(10, 1000) / 1000.0 * (_max - _min) + _min;
            double arg2 = _rnd.Next(10, 1000) / 1000.0 * (_max - _min) + _min;
            return new double[] { arg1, arg2 };
        }

        public double GetTarget(double[] input)
        {
            return Function(input[0], input[1]);
        }
    }
}

