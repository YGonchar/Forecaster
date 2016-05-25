using System;

namespace Forecaster.Utils
{
    internal struct WindDirection
    {
        private readonly string _name;
        private readonly Func<double, bool> _hitTest;

        public WindDirection(string name, Func<double, bool> hitTest)
        {
            _name = name;
            _hitTest = hitTest;
        }

        public bool IsHit(double value)
        {
            return _hitTest(value);
        }

        public override string ToString()
        {
            return _name;
        }
    }
}