using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.IO;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;

namespace ClassLibrary1
{
    public class SplinesData
    {
        public MeasuredData Md { get; set; }
        public SplineParameters Sp { get; set; }
        public double? Integral { get; set; } = null;
        public double? Der_l { get; set; } = null;
        public double? Der_r { get; set; } = null;
        public SplinesData(MeasuredData md, SplineParameters sp)
        {
            Md = md; Sp = sp;
        }
        public SplinesData()
        {
            Md = new(); Sp = new();
        }

        [DllImport("\\..\\..\\..\\..\\x64\\Debug\\Splain.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern double Interpolate(int length, double[] points, double[] func, double[] res, double[] der, int gridlen, double[] grid, double[] limits, double[] integrals);
        public double[] Splain(ref double a, ref double[] Int) 
        {
            double[] splain = new double[3 * Sp.N];
            a = Interpolate(Md.N, Md.Grid, Md.Measured, splain, new double[] { Sp.Deriative_l, Sp.Deriative_r }, Sp.N, new double[] { Md.Start, Md.End }, new double[] { Md.Int_Start, Md.Int_End }, Int);
            Integral = Int[0];
            Der_l = splain[1];
            Der_r = splain[^2];
            return splain;

        }
        public string IntInfo
        {
            get
            {
                if (Integral == null || Der_l == null || Der_r == null)
                    return "Интеграл не посчитан";
                return $"Интеграл посчитан на отрезке [{Md.Int_Start}; {Md.Int_End}]\nЗначение: {Integral}\nПроизводные на границах:\n\tСлева {Der_l}\n\tСправа {Der_r}";

            }
        }

        public ObservableCollection<string>? Str
        {
            get
            {
                if (!Md.Iszeros)
                {
                    Md._str = new();
                    for (int i = 0; i < Md.N; i++) Md._str.Add($"x[{i + 1}]: {Md.Grid[i]:f8}\t\ty[{i + 1}]: {Md.Measured[i]:f8}");
                    return Md._str;
                }
                return null;
            }
            set
            {
                Md._str = value;
            }
        }

    }
}
