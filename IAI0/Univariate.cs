using System;
using System.Collections.Generic;
using System.Text;

namespace NOT_MIT_KAN_0
{
    internal class Univariate
    {
        private int _points;
        private Basis _basis = null;
        private double[] _coefficients = null;
        private double _xmin;
        private double _xmax;
        private double _ymin;
        private double _ymax;
        private double _deltax;
        private Random _rnd = new Random();
        private double[] _vlast = null;

        public Univariate(double xmin, double xmax, double ymin, double ymax, Basis basis)
        {
            _basis = basis;
            _points = basis.nPoints();
            _xmin = xmin;
            _xmax = xmax;
            SetLimits();
            _ymin = ymin;
            _ymax = ymax;
            _coefficients = new double[_points];
            for (int i = 0; i < _points; i++)
            {
                _coefficients[i] = _rnd.Next(10, 1000) / 1000.0 * (_ymax - _ymin) + _ymax;
                _coefficients[i] /= _points;
            }
        }

        public Univariate(Univariate obj)
        {
            _basis = obj._basis;
            _points = obj._points;
            _xmin = obj._xmin;
            _xmax = obj._xmax;
            _deltax = (_xmax - _xmin) / (_points - 1);
            _ymin = obj._ymin;
            _ymax = obj._ymax;
            _coefficients = new double[_points];
            for (int i = 0; i < _points; i++)
            {
                _coefficients[i] = obj._coefficients[i];
            }
        }

        private void SetLimits()
        {
            double range = _xmax - _xmin;
            _xmin -= 0.01 * range;
            _xmax += 0.01 * range;
            _deltax = (_xmax - _xmin) / (_points - 1);
        }

        private void FitDefinition(double x)
        {
            if (x < _xmin)
            {
                _xmin = x;
                SetLimits();
            }
            if (x > _xmax)
            {
                _xmax = x;
                SetLimits();
            }
        }

        public (int k, double relative) GetSplineIndexAndOffset(double x)
        {
            double offset = (x - _xmin) / _deltax;
            int k = (int)(offset);
            double relative = offset - k;
            return (k, relative);
        }

        public double GetDerrivative(double x)
        {
            FitDefinition(x);
            (int k, double relative) = GetSplineIndexAndOffset(x);
            double[] derivative = _basis.GetAllDerivatives(k, relative);
            double v = 0.0;
            for (int i = 0; i < _points; i++)
            {
                v += derivative[i] * _coefficients[i];
            }
            return v / _deltax;
        }

        public double GetFunctionUsingInput(double x)
        {
            FitDefinition(x);
            (int k, double relative) = GetSplineIndexAndOffset(x);
            _vlast = _basis.GetAllValues(k, relative);
            double f = 0.0;
            for (int i = 0; i < _points; i++)
            {
                f += _vlast[i] * _coefficients[i];
            }
            return f;
        }

        public void UpdateUsingMemory(double delta, double mu)
        {
            for (int i = 0; i < _points; ++i)
            {
                _coefficients[i] += delta * mu * _vlast[i];
            }
        }

        public void UpdateUsingInput(double x, double delta, double mu)
        {
            FitDefinition(x);
            (int k, double relative) = GetSplineIndexAndOffset(x);
            _vlast = _basis.GetAllValues(k, relative);
            for (int i = 0; i < _points; ++i)
            {
                _coefficients[i] += delta * mu * _vlast[i];
            }
        }

        //Functions returning data for plotting pictures
        public double[] GetFunctionPoints(int N)
        {
            double delta = (_xmax - _xmin) / (N - 1);
            List<double> points = new List<double>();
            for (int i = 0; i < N; ++i)
            {
                double x = _xmin + delta * i;
                if (N - 1 == i)
                {
                    x -= (_xmax - _xmin) * 0.001;  //we don't want x = _xmax
                }
                points.Add(GetFunctionUsingInput(x));
            }
            return points.ToArray();
        }

        public (double xmin, double xmax) GetLimits()
        {
            return (_xmin, _xmax);
        }
    }
}
