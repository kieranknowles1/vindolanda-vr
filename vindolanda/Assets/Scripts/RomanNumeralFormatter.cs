using System;
using System.ComponentModel;
using UnityEngine.Localization.SmartFormat.Core.Extensions;

[DisplayName("Roman Numeral Formatter")]
public class RomanNumeralFormatter : FormatterBase
{
    public override string[] DefaultNames => new string[] { "roman" };

    struct Digit
    {
        public readonly string name;
        public readonly int value;
        public Digit(string name, int value) { this.name = name; this.value = value; }

        public static Digit GetNumeral(int value)
        {
            return value switch
            {
                >= 1000 => new("M", 1000),
                >= 900 => new("CM", 900),
                >= 500 => new("D", 500),
                >= 400 => new("CD", 400),
                >= 100 => new("C", 100),
                >= 90 => new("XC", 90),
                >= 50 => new("L", 50),
                >= 40 => new("XL", 40),
                >= 10 => new("X", 10),
                >= 9 => new("IX", 9),
                >= 5 => new("V", 5),
                >= 4 => new("IV", 4),
                >= 1 => new("I", 1),
                _ => throw new Exception("Negative numbers are not supported")
            };
        }
    }


    public override bool TryEvaluateFormat(IFormattingInfo formattingInfo)
    {
        if (formattingInfo.CurrentValue is not int number) return false;

        while (number > 0)
        {
            var num = Digit.GetNumeral(number);
            formattingInfo.Write(num.name);
            number -= num.value;
        }
        return true;
    }
}
