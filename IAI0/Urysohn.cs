using System;
using System.Collections.Generic;
using System.Text;

namespace NOT_MIT_KAN_0
{
    class Urysohn
    {
        private List<Univariate> _univariateList = new List<Univariate>();

        public Urysohn(double[] xmin, double[] xmax, double targetMin, double targetMax, Basis basis)
        {
            int nPoints = xmin.Length;
            double ymin = targetMin / nPoints;
            double ymax = targetMax / nPoints;
            for (int i = 0; i < nPoints; ++i)
            {
                Univariate univariate = new Univariate(xmin[i], xmax[i], ymin, ymax, basis);
                _univariateList.Add(univariate);
            }
        }

        public Urysohn(Urysohn obj)
        {
            _univariateList.Clear();
            foreach (Univariate uni in obj._univariateList)
            {
                _univariateList.Add(new Univariate(uni));
            }
        }

        public double GetDerrivative(int layer, double x)
        {
            return _univariateList[layer].GetDerrivative(x);
        }

        public void UpdateUsingMemory(double delta, double mu)
        {
            foreach (Univariate uni in _univariateList)
            {
                uni.UpdateUsingMemory(delta, mu);
            }
        }

        public void UpdateUsingInput(double delta, double[] inputs, double mu)
        {
            int i = 0;
            foreach (Univariate uni in _univariateList)
            {
                uni.UpdateUsingInput(inputs[i++], delta, mu);
            }
        }

        public double GetValueUsingInput(double[] inputs)
        {
            double f = 0.0;
            int i = 0;
            foreach (Univariate uni in _univariateList)
            {
                f += uni.GetFunctionUsingInput(inputs[i++]);
            }
            return f;
        }

        //Functions returning data for plotting pictures
        public double[] GetFunctionPoints(int k, int N)
        {
            if (k > _univariateList.Count - 1) return null;
            return _univariateList[k].GetFunctionPoints(N);
        }

        public (double xmin, double xmax) GetLimits(int k)
        {
            return _univariateList[k].GetLimits();
        }

        public int GetUSize()
        {
            return _univariateList.Count;
        }
    }
}
