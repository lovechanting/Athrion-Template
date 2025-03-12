using System;
using System.Collections.Generic;
using System.Text;

namespace Athrion.Utilitites
{
    internal class AttributeBreaker
    {
        [AttributeUsage(AttributeTargets.ReturnValue)]
        public class FloatNaNAttribute : Attribute
        {
            public float Value { get; }

            public FloatNaNAttribute()
            {
                Value = float.NaN;
            }
        }
    }
}
