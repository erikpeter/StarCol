using System;
using static System.Math;
using DV;

namespace OrbitCalculations
{
    public class Orbit
    {
        const double AU = 1.496e+11; // Astronimical unit;

        const double G = 6.674e-11; // Gravitational Constant;
        public DVector Position0 { get; private set; } // Position at time 0;
        public DVector Velocity0 { get; private set; } // Velocity vector at time 0;

        public double Mass { get; private set; } // Central mass kg;
        public double Mu => G * Mass;

        private double v0;
        private double r0;

        public double? PeriodSecond { get; private set; }

        public double? PeriodDay => PeriodSecond / (60 * 60 * 24.0);

        public double? PeriodYear => PeriodSecond / (365 * 60 * 60 * 24.0);

        public double Alpha { get; private set; }

        private double A => 1 / Alpha;

        public Orbit()
        {
            //default constructor uses earth like conditions;
            Position0 = new DVector(-1.0 * AU, 0.0, 0.0);
            Velocity0 = new DVector(0, -1.0*29784.8, 0000.0);
            Mass = 1.989e+30;
            CalculateParameters();
        }

        public Orbit(DVector pos0, DVector vel0)
        {
            Position0 = pos0;
            Velocity0 = vel0;
            Mass = 1.989e+30;
            CalculateParameters();
        }

        public Orbit(DVector pos0, DVector vel0, double mass)
        {
            Position0 = pos0;
            Velocity0 = vel0;
            Mass = mass;
            CalculateParameters();
        }

        private void CalculateParameters()
        {
            r0 = Position0.Length;
            v0 = Velocity0.Length;
            Alpha = (2 * Mu / r0 - v0 * v0) / Mu;
            if (Alpha > 0) // calculate period only in elliptic case;
            {
                PeriodSecond = 2 * PI * Sqrt(Pow(A, 3) / Mu);
            }
            else PeriodSecond = null;
        }

        private static double CalculateS(double z)
        {

            double S = 0.0;

            if (Abs(z) < 1e-10)
            {
                double factorial = 1.0;
                for (int i = 0; i <= 5; i++)
                {
                    factorial *= (2 * i + 2.0) * (2 * i + 3.0);
                    S += Pow(-1 * z, i) / factorial;
                }
            }
            else if (z > 0)
            {
                S = (Sqrt(z) - Sin(Sqrt(z))) / Sqrt(Pow(z, 3));
            }
            else if (z < 0)
            {
                S = (Sinh(Sqrt(-1.0*z)) - Sqrt(-1.0 * z)) / Sqrt(Pow(-1.0 * z, 3));
            }
            return (S);
        }

        private static double CalculateC(double z)
        {
            double C = 0.0;
            if (Abs(z) < 1e-10)
            {
                double factorial = 1.0;
                for (int i = 0; i <= 5; i++)
                {
                    factorial *= (2 * i + 1.0) * (2 * i + 2.0);
                    C += Pow(-1 * z, i) / factorial;
                }

            }
            else if (z > 0)
            {
                C = (1.0 - Cos(Sqrt(z))) / z;
            }
            else if (z < 0)
            {
                C = (1.0 - Cosh(Sqrt(-1.0 * z))) / z; ;
            }
            return (C);
        }

        private double TimeOfFlight(double x)
        {
            double z = Alpha * x * x;
            double C = CalculateC(z);
            double S = CalculateS(z);

            double muTime = Pow(x, 3) * S + (Position0 * Velocity0) * Pow(x, 2) * C / Sqrt(Mu) + r0 * x * (1 - z * S);
            double time = muTime / Sqrt(Mu);

            return time;
        }

        private double DTimeOfFlight(double x)
        {
            double z = Alpha * x * x;
            double C = CalculateC(z);
            double S = CalculateS(z);

            double Mudt = x * x * C + (Position0 * Velocity0) * x * (1 - z * S) / Math.Sqrt(Mu) + r0 * (1 - z * C);
            double dt = Mudt / Sqrt(Mu);

            return dt;
        }

        private double InitValueX(double t)
        {
            double x0 = 0.0;
            if (Alpha > 0) // Elliptical Case
            {
                x0 = Alpha * Sqrt(Mu) * t;
            }
            else
            {
                double logval = Log(-2 * Mu * t * Alpha / (Position0 * Velocity0) + Sign(t) * Sqrt(-Mu * A) * (1 - Alpha * r0));
                x0 = Sign(t) * Sqrt(-1.0 * A) * logval;
            }
            return x0;
        }

        private double SolveForX(double t)
        {
            double x_current = InitValueX(t);
            double currentt;
            for (int i = 0; i <= 100; i++)
            {
                double dt = DTimeOfFlight(x_current);
                currentt = TimeOfFlight(x_current);
                double step_size = (t - currentt) / dt;
                x_current += step_size;
                if (Abs(step_size) < 1.0e-10) break;
            }
            //Console.WriteLine("Solved: " + x_current);
            return x_current;
        }

        private double Calculatef(double x)
        {
            double z = Alpha * x * x;
            double C = CalculateC(z);
            //Console.WriteLine("x: " + x);
            //Console.WriteLine("C: " + C);
            double f = 1.0 - C * Pow(x, 2) / r0;
            //Console.WriteLine(f);
            return f;
        }

        private double Calculateg(double x, double time)
        {
            double z = Alpha * x * x;
            double S = CalculateS(z);
            double s = time - Pow(x, 3) * S / Sqrt(Mu);
            return s;
        }

        public DVector GetPosition(double time)
        {
            // in elliptic case, constrain time value for numerical stability;
            if (PeriodSecond is not null) time %= (double)PeriodSecond;

            double x = SolveForX(time);
            double f = Calculatef(x);
            double g = Calculateg(x, time);
            DVector newpos = f * Position0 + g * Velocity0;
            //Console.WriteLine("Position: " + newpos);
            //Console.WriteLine("Distance Sun: " + newpos.Length);
            return newpos;
        }

        public DVector GetPositionDay(double day)
        {
            return GetPosition(day * 24.0 * 60.0 * 60.0);
        }

        private DVector GetPositionFromX(double x, double time)
        {
            double f = Calculatef(x);
            double g = Calculateg(x, time);
            DVector newpos = f * Position0 + g * Velocity0;
            return newpos;
        }

        private double CalculateDf(double x, double time)
        {
            double r = GetPositionFromX(x, time).Length;
            double z = Alpha * x * x;
            double S = CalculateS(z);
            double df = Sqrt(Mu) * x * (z * S - 1.0) / (r0 * r);
            return df;
        }

        private double CalculateDg(double x, double time)
        {
            double r = GetPositionFromX(x, time).Length;
            double z = Alpha * x * x;
            double C = CalculateC(z);
            double ds = 1 - x * x * C / r;
            return ds;
        }

        public DVector GetVelocity(double time)
        {
            double x = SolveForX(time);
            double df = CalculateDf(x, time);
            double dg = CalculateDg(x, time);
            DVector newvel = df * Position0 + dg * Velocity0;
            return newvel;
        }

        public DVector GetVelocityDay(double day)
        {
            return GetVelocity(day * 24.0 * 60.0 * 60.0);
        }

        public DVector[] GetStateVector(double time)
        {
            DVector[] stateVec = new DVector[2];
            stateVec[0] = GetPosition(time);
            stateVec[1] = GetVelocity(time);
            return stateVec;
        }

        public DVector[] GetStateVectorDay(double day)
        {
            DVector[] stateVec = new DVector[2];
            double time = day * 24.0 * 60.0 * 60.0;
            stateVec[0] = GetPosition(time);
            stateVec[1] = GetVelocity(time);
            return stateVec;
        }


    }
}
