namespace GalaxyGauntlet.Common
{
    /// <summary>
    /// A functional interpretation of a software version number, allowing for comparisons between versions. 
    /// </summary>
    // Why did I make this?
    public class VersionNumber
    {
        public int? Major { get; private set; } = null!; // Nullability is checked after construction.

        public int? Minor { get; private set; } = null;

        public int? Build { get; private set; } = null;

        public int[] Revisions { get; private set; } = [];

        private readonly string? RawValue = null;


        #region Constructors

        public VersionNumber(string versionNumber)
        {
            if(string.IsNullOrEmpty(versionNumber))
            {
                throw new InvalidVersionNumberException("No version number was provided.");
            }
            RawValue = versionNumber;
            BuildVersionNumber(versionNumber);
        }


        public VersionNumber(params int[] versionNumber)
        {
            string versionString = string.Join('.', versionNumber);
            RawValue = versionString;
            BuildVersionNumber(versionString);
        }

        #endregion


        #region Parse Methods

        public static VersionNumber Parse(string versionNumber)
        {
            try
            {
                VersionNumber result = new(versionNumber);
                return result;
            }
            catch(Exception e)
            {
                throw new FormatException($"The input string '{versionNumber}' was not in the correct format.'", e);
            }
        }


        public static bool TryParse(string versionNumber, out VersionNumber? result)
        {
            try
            {
                result = new(versionNumber);
                return true;
            }
            catch
            {
                result = null;
                return false;
            }
        }

        #endregion


        #region ToString Methods

        public override string ToString()
        {
            if (Major is null)
            {
                throw new InvalidVersionNumberException("Major version number missing");
            }
            string result = $"{Major.Value}.{Minor ?? 0}.{Build ?? 0}";
            if(Revisions.Length > 0)
            {
                result += $".{string.Join('.', Revisions)}";
            }
            return result;
        }


        /// <summary>
        /// Returns the version number exactly as it was provided to the constructor.
        /// </summary>
        public string ToRawString()
        {
            return RawValue ?? ToString();
        }


        public string ToShortString()
        {
            return $"{Major.GetValueOrDefault()}.{Minor ?? 0}.{Build ?? 0}";
        }


        public string ToShortString(int digits)
        {
            digits = Math.Max(digits, 1);
            digits = Math.Min(digits, 100);
            string result = $"{Major.GetValueOrDefault()}";
            if(digits > 1)
            {
                if(Minor is null)
                {
                    return result;
                }
                result += $".{Minor.Value}";
            }
            if(digits > 2)
            {
                if(Build is null)
                {
                    return result;
                }
                result += $".{Build.Value}";
            }
            for(int i = 3; i < Revisions.Length + 3; i++)
            {
                if(i > digits)
                {
                    break;
                }

                result += $".{Revisions[i - 3]}";
            }
            return result;
        }

        #endregion


        #region Object Override Methods

        public override bool Equals(object? obj)
        {
            if(obj is not VersionNumber)
            {
                return false;
            }

            return this == (VersionNumber)obj;
        }


        public override int GetHashCode()
        {
            int revisionHash = Revisions.GetHashCode();
            return HashCode.Combine(Major, Minor, Build, revisionHash);
        }

        #endregion


        #region Comparison Operators

        public static bool operator >(VersionNumber a, VersionNumber b)
        {
            return Compare(a, b) == 1;
        }


        public static bool operator <(VersionNumber a, VersionNumber b)
        {
            return Compare(a, b) == -1;
        }


        public static bool operator ==(VersionNumber a, VersionNumber b)
        {
            return Compare(a, b) == 0;
        }


        public static bool operator >=(VersionNumber a, VersionNumber b)
        {
            return (a > b) || (a == b);
        }


        public static bool operator <=(VersionNumber a, VersionNumber b)
        {
            return (a < b) || (a == b);
        }


        public static bool operator !=(VersionNumber a, VersionNumber b)
        {
            return Compare(a, b) != 0;
        }

        #endregion


        #region Construction Helpers

        private void BuildVersionNumber(string versionNumber)
        {
            string[] numbers = versionNumber.Split('.');

            List<int> revisions = [];
            for (int i = 0; i < numbers.Length; i++)
            {
                string digit = numbers[i];
                switch (i)
                {
                    case 0:
                        if(digit.Length > 1
                            && string.Equals($"{digit[0]}", "v", StringComparison.InvariantCultureIgnoreCase))
                        {
                            digit = digit[1..];
                        }
                        Major = GetNumber(digit, false);
                        break;

                    case 1:
                        Minor = GetNumber(digit);
                        break;

                    case 2:
                        Build = GetNumber(digit);
                        break;

                    default:
                        revisions.Add(GetNumber(digit));
                        break;
                }
            }
            Revisions = [.. revisions];
            if (Major is null)
            {
                throw new InvalidVersionNumberException("Major version number is missing");
            }
        }


        private static int GetNumber(string digit)
        {
            return GetNumber(digit, true);
        }


        private static int GetNumber(string digit, bool allowLetters)
        {
            if(int.TryParse(digit, out int value))
            {
                return value;
            }
            if(false == allowLetters)
            {
                throw new InvalidVersionNumberException($"{digit} is not a valid Major version number. Only digits are allowed.");
            }

            if(digit.Length != 1)
            {
                throw new InvalidVersionDigitException(digit);
            }

            string digitLowercase = digit.ToLowerInvariant();
            string latinLetters = "abcdefghijklmnopqrstuvwxyz";
            string greekLetters = "αβγδεζηθικλμνξοπρστυφχψω";

            int stringIndex = latinLetters.IndexOf(digitLowercase[0]);
            if(stringIndex >= 0)
            {
                return stringIndex - (latinLetters.Length);
            }

            stringIndex = greekLetters.IndexOf(digitLowercase[0]);
            if(stringIndex >= 0)
            {
                return stringIndex - (greekLetters.Length);
            }

            throw new InvalidVersionDigitException(digit);
        }

        #endregion


        #region Comparison Methods

        private static int Compare(VersionNumber a, VersionNumber b)
        {
            // Compare major version number
            int comparison = CompareNumber(a.Major, b.Major);
            if(comparison != 0)
            {
                return comparison;
            }

            comparison = CompareNumber(a.Minor, b.Minor);
            if(comparison != 0)
            {
                return comparison;
            }

            comparison = CompareNumber(a.Build, b.Build);
            if (comparison != 0)
            {
                return comparison;
            }

            for(int i = 0; i < Math.Max(a.Revisions.Length, b.Revisions.Length); i++)
            {
                if(i >= b.Revisions.Length)
                {
                    return 1;
                }

                if(i >= a.Revisions.Length)
                {
                    return -1;
                }

                comparison = CompareNumber(a.Revisions[i], b.Revisions[i]);
                if(comparison != 0)
                {
                    return comparison;
                }
            }

            return 0;
        }


        private static int CompareNumber(int? a, int? b)
        {
            if(a.HasValue != b.HasValue)
            {
                return (b is null) ? 1 : -1;
            }
            if(a is null)
            {
                return 0;
            }

            if(a.Value != b!.Value) // b is guaranteed to be not null due to above checks.
            {
                return (a.Value > b.Value) ? 1 : -1;
            }
            return 0;
        }

        #endregion
    }


    public class InvalidVersionNumberException(string message) : Exception($"Invalid version number: {message}");


    public class InvalidVersionDigitException(string digit) : Exception($"{digit} is not a valid digit. Either a number, a single latin letter, or a single greek letter is allowed.");
}
