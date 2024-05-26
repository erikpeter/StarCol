
using System;

namespace DV
{
	public class DVector
	{
		public double X { get; set; }
		public double Y { get; set; }
		public double Z { get; set; }

		public DVector Normalized => (1 / Length) * this;

		public double Length => Math.Sqrt(X * X + Y * Y + Z * Z);


		public DVector()
		{
			X = 0;
			Y = 0;
			Z = 0;
		}

		public DVector(double x, double y, double z)
		{
			X = x;
			Y = y;
			Z = z;
		}

		public static DVector Cross(DVector a, DVector b)
		{
			double newx = a.Y * b.Z - a.Z * b.Y;
			double newy = a.Z * b.X - a.X * b.Z;
			double newz = a.X * b.Y - a.Y * b.X;
			return new DVector(newx, newy, newz);
		}



		public static DVector operator +(DVector a, DVector b)
		{
			double newx = a.X + b.X;
			double newy = a.Y + b.Y;
			double newz = a.Z + b.Z;
			return new DVector(newx, newy, newz);
		}

		public static DVector operator -(DVector a, DVector b)
		{
			double newx = a.X - b.X;
			double newy = a.Y - b.Y;
			double newz = a.Z - b.Z;
			return new DVector(newx, newy, newz);
		}

		public static double operator *(DVector a, DVector b)
		{
			return a.X * b.X + a.Y * b.Y + a.Z * b.Z;
		}

		public static DVector operator *(double a, DVector b)
		{
			double newx = a * b.X;
			double newy = a * b.Y;
			double newz = a * b.Z;
			return new DVector(newx, newy, newz);
		}

		public static DVector operator *(DVector b, double a)
		{
			return a * b;
		}

		public override string ToString()
		{
			string s = "X: " + this.X + " Y:" + this.Y + " Z:" + this.Z;
			return s;
		}



	}
}
