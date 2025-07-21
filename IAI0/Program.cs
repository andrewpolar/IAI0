//NOT_MIT_KAN v.0
//Concept: Andrew Polar and Mike Poluektov
//Developer Andrew Polar

// License
// If the end user somehow manages to make billions of US dollars using this code,
// and happens to meet the developer begging for change outside a McDonald's,
// he or she is under no obligation to buy the developer a sandwich.

// Symmetry Clause
// Likewise, if the developer becomes rich and famous by publishing this code,
// and meets an unfortunate end user who went bankrupt using it,
// the developer is also under no obligation to buy the end user a sandwich.

//Publications:
//https://www.sciencedirect.com/science/article/abs/pii/S0016003220301149
//https://www.sciencedirect.com/science/article/abs/pii/S0952197620303742
//https://arxiv.org/abs/2305.08194

//Some toy datasets are taken from MIT benchmark:
//https://kindxiaoming.github.io/pykan/

//Formula3 is designed by Mike Poluektov.
//Formula4 is epistemic uncertainty test: the area of triangles given by vertices 
//Formula1 from MIT example

using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;

namespace NOT_MIT_KAN_0
{
    class Model
    {
        public List<KANAddend> listKM = null;
        public double error = 0.0;
        public int nEpochs = -1;

        public void Instantiate(double[] xmin, double[] xmax,
            double targetMin, double targetMax, int M, Basis inner, Basis outer, 
            double muInner, double muOuter)
        {
            listKM = new List<KANAddend>();
            double zmin = targetMin / M;
            double zmax = targetMax / M;
            for (int i = 0; i < M; ++i)
            {
                listKM.Add(new KANAddend(xmin, xmax, zmin, zmax, inner, outer, muInner, muOuter));
            }
        }
    }

    internal class Program
    {
        private static List<double[]> inputs = null;
        private static List<double> target = null;
        private static double[] xmin = null;
        private static double[] xmax = null;
        private static double targetMin;
        private static double targetMax;

        private static (double[] xmin, double[] xmax, double targetMins, double targetMax)
        FindMinMax(List<double[]> inputs, List<double> target)
        {
            int size = inputs[0].Length;
            double[] xmin = new double[size];
            double[] xmax = new double[size];

            for (int i = 0; i < size; ++i)
            {
                xmin[i] = double.MaxValue;
                xmax[i] = double.MinValue;
            }

            for (int i = 0; i < inputs.Count; ++i)
            {
                for (int j = 0; j < inputs[i].Length; ++j)
                {
                    if (inputs[i][j] < xmin[j]) xmin[j] = inputs[i][j];
                    if (inputs[i][j] > xmax[j]) xmax[j] = inputs[i][j];
                }

            }

            double targetMin = double.MaxValue;
            double targetMax = double.MinValue;
            for (int j = 0; j < target.Count; ++j)
            {
                if (target[j] < targetMin) targetMin = target[j];
                if (target[j] > targetMax) targetMax = target[j];
            }

            return (xmin, xmax, targetMin, targetMax);
        }

        static void DoWork(Object obj)
        {
            Model m1 = (Model)obj;
            for (int epoch = 0; epoch < m1.nEpochs; ++epoch)
            {
                m1.error = 0.0;
                for (int i = 0; i < inputs.Count; ++i)
                {
                    double residual = target[i];
                    foreach (KANAddend modelPL in m1.listKM)
                    {
                        residual -= modelPL.ComputeUsingInput(inputs[i]);
                    }
                    foreach (KANAddend modelPL in m1.listKM)
                    {
                        modelPL.UpdateUsingMemory(residual);
                        //modelPL.UpdateUsingInput(inputs[k], residual);
                    }
                    m1.error += residual * residual;
                }
                m1.error /= inputs.Count;
                m1.error = Math.Sqrt(m1.error);
                Console.WriteLine("Training step {0}, relative RMSE {1:0.0000}", epoch, m1.error);
            }
        }

        private static int SelectTheBest(Model[] mm)
        {
            int nMin = 0;
            double min = mm[0].error;
            for (int i = 0; i < mm.Length; ++i)
            {
                if (mm[i].error < min)
                {
                    min = mm[i].error;
                    nMin = i;
                }
            }
            return nMin;
        }

        static void Main(string[] args)
        {
            Formula1 f = new Formula1();
            (inputs, target) = f.GenerateData(1000);

            DateTime start = DateTime.Now;
            (xmin, xmax, targetMin, targetMax) = FindMinMax(inputs, target);

            //Settings for the training
            //these settings are only good for Formula1
            //for other data the choices must be customized
            int M = 1;   //addends
            int TH = 8;  //threads
            Basis inner = new Basis(8);
            Basis outer = new Basis(16);
            double muInner = 0.2;
            double muOuter = 0.1;
            int Epochs = 400;
            int marginStart = 60;
            int marginEnd = 60;
            double sensitivity = 0.0;
            //end setting part

            Model[] m = new Model[TH];
            for (int i = 0; i < TH; ++i)
            {
                m[i] = new Model();
                m[i].nEpochs = 16;
                m[i].Instantiate(xmin, xmax, targetMin, targetMax, M, inner, outer, muInner, muOuter);
            }

            //Concurrent threads for all models
            List<Thread> threads = new List<Thread>();
            for (int i = 0; i < TH; ++i)
            {
                threads.Add(new Thread(new ParameterizedThreadStart(DoWork)));
            }

            for (int i = 0; i < TH; ++i)
            {
                threads[i].Start(m[i]);
            }

            for (int i = 0; i < TH; ++i)
            {
                threads[i].Join();
            }

            Console.WriteLine("\n--- end of the first block ---\n");

            //Selection the best and making copies
            int n = SelectTheBest(m);

            int end = Epochs - marginEnd;
            double[] residualError = new double[inputs.Count];
            for (int step = 0; step < Epochs; ++step)
            {
                double error2 = 0.0;
                double cnt = 0;
                for (int i = 0; i < inputs.Count; ++i)
                {
                    if (step >= marginStart && Epochs < end && residualError[i] < sensitivity) continue;
                    double model = 0.0;
                    foreach (KANAddend addend in m[n].listKM)
                    {
                        model += addend.ComputeUsingInput(inputs[i]);
                    }
                    double diff = target[i] - model;

                    foreach (KANAddend addend in m[n].listKM)
                    {
                        addend.UpdateUsingMemory(diff);
                        //addend.UpdateUsingInput(inputs[i], diff);
                    }
                    error2 += diff * diff;
                    residualError[i] = Math.Abs(diff);
                    ++cnt;
                }
                error2 /= cnt;
                error2 = Math.Sqrt(error2);
                if (0 == step % 25)
                {
                    Console.WriteLine("Training step {0}, RMSE {1:0.0000}", step, error2);
                }
            }
            DateTime endt = DateTime.Now;
            TimeSpan duration = endt - start;
            double time = duration.Minutes * 60.0 + duration.Seconds + duration.Milliseconds / 1000.0;
            Console.WriteLine("Time for building representation {0:####.00} seconds", time);

            double error = 0.0;
            int NTests = 1000;
            for (int i = 0; i < NTests; ++i)
            {
                double[] test_input = f.GetInput();
                double test_target = f.GetTarget(test_input);
                double model = 0.0;
                foreach (KANAddend addend in m[n].listKM)
                {
                    model += addend.ComputeUsingInput(test_input);
                }
                error += (test_target - model) * (test_target - model);
            }
            error /= NTests;
            error = Math.Sqrt(error);
            Console.WriteLine("\nRMSE for unseen data {0:0.0000}", error);

            Plotter plotter = new Plotter();
            plotter.PlotFunctions(m[n].listKM, "Charts");
        }
    }
}
