using System;
using System.Collections.Generic;
using System.Text;

namespace NOT_MIT_KAN_0
{
    class KANAddend
    {
        private double _muInner;
        private double _muOuter;
        private double _lastInnerValue;

        private Urysohn _u = null;
        private Univariate _univariate = null;
        private Random _rnd = new Random();

        public KANAddend(double[] xmin, double[] xmax, double targetMin, double targetMax,
            Basis innerBasis, Basis outerBasis, double mu_inner, double mu_outer)
        {
            _muInner = mu_inner;
            _muOuter = mu_outer;

            _u = new Urysohn(xmin, xmax, targetMin, targetMax, innerBasis);
            _univariate = new Univariate(targetMin, targetMax, targetMin, targetMax, outerBasis);
        }

        public KANAddend(KANAddend obj)
        {
            _muInner = obj._muInner;
            _muOuter = obj._muOuter;
            _lastInnerValue = obj._lastInnerValue;
            _univariate = new Univariate(obj._univariate);
            _u = new Urysohn(obj._u);
        }

        public void UpdateUsingMemory(double diff)
        {
            double derrivative = _univariate.GetDerrivative(_lastInnerValue);
            _u.UpdateUsingMemory(diff * derrivative, _muInner);
            _univariate.UpdateUsingMemory(diff, _muOuter);
        }

        public void UpdateUsingInput(double[] inputs, double diff)
        {
            double value = _u.GetValueUsingInput(inputs);
            double derrivative = _univariate.GetDerrivative(value);
            _u.UpdateUsingInput(diff * derrivative, inputs, _muInner);
            _univariate.UpdateUsingInput(value, diff, _muOuter);
        }

        public double ComputeUsingInput(double[] inputs)
        {
            _lastInnerValue = _u.GetValueUsingInput(inputs);
            return _univariate.GetFunctionUsingInput(_lastInnerValue);
        }

        //Functions used for plotting only
        public double[] AddendUnivariateFunction(int N)
        {
            return _univariate.GetFunctionPoints(N);
        }

        public (double xmin, double xmax) AdendUnivariateLimist()
        {
            return _univariate.GetLimits();
        }

        public int GetUSize(int k)
        {
            return _u.GetUSize();
        }

        public double[] GetUListFunctionPoints(int k, int m, int N)
        {
            return _u.GetFunctionPoints(m, N);
        }

        public (double xmin, double xmax) GetUListLimits(int k, int m)
        {
            return _u.GetLimits(m);
        }
    }
}