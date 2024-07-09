using System;
using System.Collections.Generic;
using System.Text;

namespace NOT_MIT_KAN_0
{
    internal class Spline
    {
        private double a, b, c, d;

        public double GetValue(double x)
        {
            return a + b * x + c * x * x + d * x * x * x;
        }

        public double GetDerivative(double x)
        {
            return b + 2.0 * c * x + 3.0 * d * x * x;
        }

        public Spline(double A, double B, double C, double D)
        {
            this.a = A; this.b = B; this.c = C; this.d = D;
        }
    }

    internal class BasisFunction
    {
        public List<Spline> splines = new List<Spline>();

        public void AddSpline(double A, double B, double C, double D)
        {
            splines.Add(new Spline(A, B, C, D));
        }

        public double GetDerivative(int spline, double relativeDistance)
        {
            return splines[spline].GetDerivative(relativeDistance);
        }

        public double GetValue(int spline, double relativeDistance)
        {
            return splines[spline].GetValue(relativeDistance);
        }
    }

    internal class Basis
    {
        private int _points = 0;
        List<BasisFunction> _basisFunctions = new List<BasisFunction>();

        public Basis(int Points)
        {
            _points = Points;

            double[] h = new double[_points - 1];
            for (int i = 0; i < h.Length; i++)
            {
                h[i] = 1.0;
            }

            AlgebraHelper ah = new AlgebraHelper();
            double[][] M = ah.GenerateTriDiagonal(_points, h);
            double[][] R = ah.MatInverseQR(M);

            for (int i = 0; i < _points; ++i)
            {
                double[] e = new double[_points];
                for (int j = 0; j < _points; ++j)
                {
                    e[j] = 0.0;
                }
                e[i] = 1.0;

                (double[] a, double[] b, double[] c, double[] d) = ah.MakeSplines(R, e, h);

                BasisFunction basisFunction = new BasisFunction();
                for (int j = 0; j < a.Length; ++j)
                {
                    basisFunction.AddSpline(a[j], b[j], c[j], d[j]);
                }
                _basisFunctions.Add(basisFunction);
            }
        }

        public int nPoints()
        {
            return _points;
        }

        public double[] GetAllValues(int k, double relative)
        {
            double[] values = new double[_points];
            for (int i = 0; i < _points; i++)
            {
                values[i] = _basisFunctions[i].GetValue(k, relative);
            }
            return values;
        }

        public double[] GetAllDerivatives(int k, double relative)
        {
            double[] derivative = new double[_points];
            for (int i = 0; i < _points; i++)
            {
                derivative[i] = _basisFunctions[i].GetDerivative(k, relative);
            }
            return derivative;
        }
    }
}
