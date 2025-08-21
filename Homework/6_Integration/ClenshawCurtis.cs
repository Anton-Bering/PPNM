using System;
using static System.Math;
using static Integration.Adaptive;

namespace Integration {

  public static class ClenshawCurtis {

    public static Result IntegrateCC(Func<double,double> f,double a,double b,
                                     double acc=1e-6,double eps=1e-6)
    {
      Func<double,double> g = theta =>{
        double c = (a+b)/2 + (b-a)/2*Cos(theta);
        return f(c)*Sin(theta)*(b-a)/2;
      };
      return Integrate(g,0,PI,acc,eps);
    }

    /* --- infinite limits --------------------------------------------------- */
    public static Result IntegrateInfinite(Func<double,double> f,
                                           double a,double b,
                                           double acc=1e-6,double eps=1e-6)
    {
      if(double.IsNegativeInfinity(a) && double.IsPositiveInfinity(b))
        return IntegrateCC(t => f(Tan(t)) / (Cos(t)*Cos(t)), -PI/2, PI/2, acc, eps);
      if(double.IsNegativeInfinity(a))
        return IntegrateCC(t=>f(b - (1-t)/t)/t/t, 0, 1, acc,eps);
      if(double.IsPositiveInfinity(b))
        return IntegrateCC(t=>f(a + (1-t)/t)/t/t, 0, 1, acc,eps);
      throw new ArgumentException("At least one limit must be infinite.");
    }
  }
}
