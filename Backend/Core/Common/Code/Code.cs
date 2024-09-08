using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;

namespace Core.Common.Code
{
    public class Codes<T> : Stack<Code<T>>
        where T : Enum;

    public class Code<T>
        where T : Enum
    {
        private static ReadOnlySpan<char> allowedChar =>
            "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789".ToCharArray();
        public string Value { get; init; } = String.Empty;
        public T Type { get; init; }
        public DateTime? ValidTo { get; init; }
        public bool Used { get; set; } = false;
        public bool OneTime { get; set; } = false;

        public Code() { }

        public Code(T type, bool oneTime = false)
        {
            Type = type;
            Value = RandomNumberGenerator.GetString(allowedChar, 8);
            OneTime = oneTime;
        }

        public Code(T type, DateTime validTo, bool oneTime = false)
        {
            Type = type;
            Value = RandomNumberGenerator.GetString(allowedChar, 8);
            ValidTo = validTo;
            OneTime = oneTime;
        }

        /// <param name="type"></param>
        /// <param name="duration">Code duration in seconds</param>
        public Code(T type, uint duration, bool oneTime = false)
        {
            Type = type;
            Value = RandomNumberGenerator.GetString(allowedChar, 8);
            ValidTo = DateTime.Now.AddSeconds(duration);
            OneTime = oneTime;
        }

        public bool Check(string value, T type)
        {
            if (value != Value || !Type.Equals(type))
            {
                return false;
            }

            if (ValidTo != null && ValidTo < DateTime.Now)
            {
                return false;
            }

            if (OneTime && Used)
            {
                return false;
            }

            return true;
        }
    }
}
