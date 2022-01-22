using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace compiler.Interpreter
{
    public interface ILastExpressionValue 
    {
        ValueType GetValueType();
    }
    public class LastExpressionValueInt : ILastExpressionValue
    {
        public ValueType ValueType = ValueType.INT;
        public int LastValue;

        public LastExpressionValueInt(int lastValue)
        {
            LastValue = lastValue;
        }

        public ValueType GetValueType()
        {
            return ValueType;
        }
    }

    public class LastExpressionValueDouble : ILastExpressionValue
    {
        public ValueType ValueType = ValueType.DOUBLE;
        public double LastValue;

        public LastExpressionValueDouble(double lastValue)
        {
            LastValue = lastValue;
        }
        public ValueType GetValueType()
        {
            return ValueType;
        }
    }

    public enum ValueType 
    {
        INT,
        DOUBLE
    }
}
