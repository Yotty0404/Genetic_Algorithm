/*
 * Copyright (c) 2011 Hiroaki,Komori
 * http://komozo.blogspot.com/2011/04/rgbcielab.html
 */

using System;
using System.Drawing;

namespace Genetic_Algorithm
{
    //
    //　CIELAB構造体
    //
    struct CIELAB
    {
        private const double Xn = 0.950456;
        private const double Yn = 1.0;
        private const double Zn = 1.088754;

        private double l;
        private double a;
        private double b;

        public double L
        {
            get { return l; }
            set { l = value; }
        }

        public double A
        {
            get { return a; }
            set { a = value; }
        }

        public double B
        {
            get { return b; }
            set { b = value; }
        }


        public CIELAB(Double _L, Double _A, Double _B)
        {
            l = _L;
            a = _A;
            b = _B;
        }

        public CIELAB(Color rgb)
        {
            CIELAB _lab = CIELAB.Parse(rgb);
            l = _lab.L;
            a = _lab.A;
            b = _lab.B;
        }

        public CIELAB(CIEXYZ xyz)
        {
            CIELAB _lab = CIELAB.Parse(xyz);
            l = _lab.L;
            a = _lab.A;
            b = _lab.B;
        }

        ///<summary>
        /// Colorへ変換
        ///</summary>
        ///<param name="" hsv""="">
        //////<returns></returns>
        public Color ToColor()
        {
            return ToCIEXyz().ToColor();
        }

        ///<summary>
        /// CIEXyzへ変換
        ///</summary>
        ///<param name="" hsv""="">
        //////<returns></returns>
        public CIEXYZ ToCIEXyz()
        {
            double _delta = 6.0 / 29.0;
            double _fy = (l + 16.0) / 116.0;
            double _fx = _fy + a / 500.0;
            double _fz = _fy - b / 200.0;

            double _x, _y, _z;

            if (_fx > _delta)
            {
                _x = Xn * Math.Pow(_fx, 3);
            }
            else
            {
                _x = (_fx - 16.0 / 116.0) * 3 * Math.Pow(_delta, 2);
            }

            if (_fy > _delta)
            {
                _y = Yn * Math.Pow(_fy, 3);
            }
            else
            {
                _y = (_fy - 16.0 / 116.0) * 3 * Math.Pow(_delta, 2);
            }

            if (_fz > _delta)
            {
                _z = Zn * Math.Pow(_fz, 3);
            }
            else
            {
                _z = (_fz - 16.0 / 116.0) * 3 * Math.Pow(_delta, 2);
            }

            return new CIEXYZ(_x, _y, _z);
        }

        ///<summary>
        /// CIEXYZからCIELabへ変換
        ///</summary>
        public static CIELAB Parse(CIEXYZ xyz)
        {
            double _l = 116.0 * Fxyz(xyz.Y / Yn) - 16;
            double _a = 500.0 * (Fxyz(xyz.X / Xn) - Fxyz(xyz.Y / Yn));
            double _b = 200.0 * (Fxyz(xyz.Y / Yn) - Fxyz(xyz.Z / Zn));

            CIELAB _result = new CIELAB(_l, _a, _b);

            return _result;
        }

        ///<summary>
        /// XYZ to L*a*b* transformation function.
        ///</summary>
        private static double Fxyz(double t)
        {
            double _result;
            if (t > 0.008856)
            {
                _result = Math.Pow(t, (1.0 / 3.0));
            }
            else
            {
                _result = 7.787 * t + 16.0 / 116.0;
            }

            return _result;
        }

        ///<summary>
        /// CIEXYZからColorへ変換
        ///</summary>
        ///<param name="" col""="">
        //////<returns></returns>
        public static CIELAB Parse(Color col)
        {
            CIEXYZ _xyz = CIEXYZ.Parse(col);
            return CIELAB.Parse(_xyz);
        }

        ///<summary>
        ///文字列に変換
        ///</summary>
        ///<returns></returns>
        public override string ToString()
        {
            string _result = string.Format("CIELab [L*={0}, a*={1}, b*={2}", l.ToString("F"), a.ToString("F"), b.ToString("F"));
            return _result;
        }
    }

    //
    //　CIEXYZ構造体
    //
    struct CIEXYZ
    {
        private double x;
        private double y;
        private double z;

        public double X
        {
            get { return x; }
            set { x = value; }
        }

        public double Y
        {
            get { return y; }
            set { y = value; }
        }

        public double Z
        {
            get { return z; }
            set { z = value; }
        }


        public CIEXYZ(Double _X, Double _Y, Double _Z)
        {
            x = _X;
            y = _Y;
            z = _Z;
        }

        public CIEXYZ(Color rgb)
        {
            CIEXYZ _xyz = Parse(rgb);

            x = _xyz.X;
            y = _xyz.Y;
            z = _xyz.Z;
        }

        ///<summary>
        /// CIELabからCIEXyzへ変換
        ///</summary>
        ///<param name="" lab""="">
        public CIEXYZ(CIELAB Lab)
        {
            CIEXYZ _xyz = Lab.ToCIEXyz();
            x = _xyz.X;
            y = _xyz.Y;
            z = _xyz.Z;
        }

        ///<summary>
        /// ColorからXYZへ変換
        ///</summary>
        ///<param name="" col""="">
        ///<returns></returns>
        public static CIEXYZ Parse(Color col)
        {
            double _RLinear = col.R / 255d;
            double _GLinear = col.G / 255d;
            double _BLinear = col.B / 255d;

            double _r, _g, _b, _x, _y, _z;

            if (_RLinear > 0.04045)
            {
                _r = Math.Pow((_RLinear + 0.055) / (1 + 0.055), 2.2);
            }
            else
            {
                _r = (_RLinear / 12.92);
            }

            if (_GLinear > 0.04045)
            {
                _g = Math.Pow((_GLinear + 0.055) / (1 + 0.055), 2.2);
            }
            else
            {
                _g = (_GLinear / 12.92);
            }

            if (_BLinear > 0.04045)
            {
                _b = Math.Pow((_BLinear + 0.055) / (1 + 0.055), 2.2);
            }
            else
            {
                _b = (_BLinear / 12.92);
            }

            _x = _r * 0.4124 + _g * 0.3576 + _b * 0.1805;
            _y = _r * 0.2126 + _g * 0.7152 + _b * 0.0722;
            _z = _r * 0.0193 + _g * 0.1192 + _b * 0.9505;
            CIEXYZ _result = new CIEXYZ(_x, _y, _z);

            return _result;
        }

        ///<summary>
        /// Colorへ変換
        ///</summary>
        ///<param name="" xyz""="">
        ///<returns></returns>
        public Color ToColor()
        {
            double _r = 3.240479 * x - 1.53715 * y - 0.498535 * z;
            double _g = -0.969256 * x + 1.875991 * y + 0.041556 * z;
            double _b = 0.055648 * x - 0.204043 * y + 1.057311 * z;

            Color _result = Color.FromArgb(FromLinear(_r), FromLinear(_g), FromLinear(_b));
            return _result;
        }

        private static int FromLinear(double v)
        {
            int _result;

            if (v <= 0.0031308)
            {
                _result = (int)(Clamp(v * 12.92) * 255.0 + 0.5);
            }
            else
            {
                _result = (int)(Clamp(1.055 * Math.Pow(v, (1.0 / 2.4)) - 0.055) * 255.0 + 0.5);
            }

            return _result;
        }

        private static double Clamp(double v)
        {
            double _result = v;

            if (v < 0.0)
            {
                _result = 0.0;
            }
            else if (v > 1.0)
            {
                _result = 1.0;
            }

            return _result;
        }

        public CIELAB ToCIELab()
        {
            CIELAB _result = new CIELAB(new CIEXYZ(x, y, z));
            return _result;
        }

        ///<summary>
        ///文字列に変換
        ///</summary>
        ///<returns></returns>
        public override string ToString()
        {
            string _result = string.Format("CIEXyz [X={0}, Y={1}, Z={2}", x.ToString("F"), y.ToString("F"), z.ToString("F"));
            return _result;
        }
    }
}
