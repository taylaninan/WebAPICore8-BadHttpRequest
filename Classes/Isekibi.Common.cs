using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Web;
using MimeKit;
using MailKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit.Encodings;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;

namespace Isekibi.Common
{
    ///////////////////////////////////////////////////////////////////////////
    // Conversion functions
    ///////////////////////////////////////////////////////////////////////////
    #region CONVERTX...
    public static class ConvertX
    {
        private static string ReplaceSplitters(string prmValue)
        {
            prmValue = prmValue.Replace(';', ',');
            prmValue = prmValue.Replace(':', ',');
            prmValue = prmValue.Replace('.', ',');
            prmValue = prmValue.Replace('-', ',');
            prmValue = prmValue.Replace('_', ',');

            return prmValue;
        }

        private static string PreProcess(string prmValue, string prmPreProcess)
        {
            prmPreProcess = ReplaceSplitters(prmPreProcess);
            string[] strPreProcessValues = prmPreProcess.Split(',');

            for (int Counter = 0; Counter < strPreProcessValues.Length; Counter++)
            {
                string strPreProcessValue = strPreProcessValues[Counter].Trim().ToLower();

                if (!String.IsNullOrEmpty(strPreProcessValue) && !String.IsNullOrEmpty(prmValue))
                {
                    if (strPreProcessValue == "trim")
                    {
                        prmValue = prmValue.Trim();
                    }

                    if (strPreProcessValue == "lower")
                    {
                        prmValue = prmValue.ToLower();
                    }

                    if (strPreProcessValue == "upper")
                    {
                        prmValue = prmValue.ToUpper();
                    }

                    if (strPreProcessValue == "space")
                    {
                        prmValue = prmValue.Replace(" ", "");
                    }

                    if (strPreProcessValue == "whitespace")
                    {
                        while (prmValue.Contains("  "))
                        {
                            prmValue = prmValue.Replace("  ", " ");
                        }
                    }

                    if (strPreProcessValue == "striptags")
                    {
                        prmValue = HtmlX.StripTags(prmValue);
                    }
                }
            }

            return prmValue;
        }

        public static string ToString(string prmValue, uint prmLength, string prmValidValues, string prmPreProcess, string prmDefault)
        {
            // Pre Process
            prmValue = PreProcess(prmValue, prmPreProcess);

            // Check Length
            if (prmLength != 0)
            {
                if (!String.IsNullOrEmpty(prmValue))
                {
                    if (prmValue.Length != prmLength)
                    {
                        prmValue = prmDefault;
                    }
                }
                else
                {
                    prmValue = prmDefault;
                }
            }

            // Check Valid Values
            bool ValidFound = false;
            int CheckCount = 0;

            prmValidValues = ReplaceSplitters(prmValidValues);
            string[] strValidValues = prmValidValues.Split(',');

            foreach (string strValidValue in strValidValues)
            {
                if (!String.IsNullOrEmpty(strValidValue))
                {
                    CheckCount++;

                    if (strValidValue == prmValue)
                    {
                        ValidFound = true;
                    }
                }
            }

            if (CheckCount > 0 && !ValidFound)
            {
                prmValue = prmDefault;
            }

            // Return result;
            return prmValue;
        }

        public static DateX ToDateX(string prmValue, uint prmLength, string prmValidValues, string prmPreProcess, DateX prmDefault)
        {
            DateX Result;

            // Pre Process
            prmValue = PreProcess(prmValue, prmPreProcess);

            // Check Length
            if (prmLength != 0)
            {
                if (!String.IsNullOrEmpty(prmValue))
                {
                    if (prmValue.Length != prmLength)
                    {
                        prmValue = prmDefault.ToString();
                    }
                }
                else
                {
                    prmValue = prmDefault.ToString();
                }
            }

            // Try to Parse
            bool Converted = DateX.TryParse(prmValue, out Result);
            if (!Converted)
            {
                Result = prmDefault;
            }

            // Check for valid values
            bool ValidFound = false;
            int CheckCount = 0;

            prmValidValues = ReplaceSplitters(prmValidValues);
            string[] strValidValues = prmValidValues.Split(',');

            foreach (string strValidValue in strValidValues)
            {
                if (!String.IsNullOrEmpty(strValidValue))
                {
                    CheckCount++;

                    if (strValidValue == Result.ToString())
                    {
                        ValidFound = true;
                    }
                }
            }

            if (CheckCount > 0 && !ValidFound)
            {
                Result = prmDefault;
            }

            return Result;
        }

        public static bool ToBoolean(string prmValue)
        {
            bool Result;

            prmValue = ToString(prmValue, 0, "1,0,on,off,true,false", "trim,space,striptags,lower", "");

            if (String.IsNullOrEmpty(prmValue))
            {
                Result = false;
            }
            else
            {
                Result = (prmValue == "1" || prmValue == "on" || prmValue == "true");
            }

            // Return result;
            return Result;
        }

        public static byte ToByte(string prmValue, uint prmLength, string prmValidValues, string prmPreProcess, byte prmDefault)
        {
            byte Result;

            // Pre Process
            prmValue = PreProcess(prmValue, prmPreProcess);

            // Check Length
            if (prmLength != 0)
            {
                if (!String.IsNullOrEmpty(prmValue))
                {
                    if (prmValue.Length != prmLength)
                    {
                        prmValue = prmDefault.ToString();
                    }
                }
                else
                {
                    prmValue = prmDefault.ToString();
                }
            }

            // Try to Parse
            bool Converted = Byte.TryParse(prmValue, out Result);
            if (!Converted)
            {
                Result = prmDefault;
            }

            // Check Valid Values
            byte ValidValue;
            bool ValidFound = false;
            int CheckCount = 0;

            prmValidValues = ReplaceSplitters(prmValidValues);
            string[] strValidValues = prmValidValues.Split(',');

            foreach (string strValidValue in strValidValues)
            {
                if (!String.IsNullOrEmpty(strValidValue))
                {
                    Converted = Byte.TryParse(strValidValue, out ValidValue);
                    if (Converted)
                    {
                        CheckCount++;

                        if (ValidValue == Result)
                        {
                            ValidFound = true;
                        }
                    }
                }
            }

            if (CheckCount > 0 && !ValidFound)
            {
                Result = prmDefault;
            }

            // Return result;
            return Result;
        }

        public static sbyte ToSByte(string prmValue, uint prmLength, string prmValidValues, string prmPreProcess, sbyte prmDefault)
        {
            sbyte Result;

            // Pre Process
            prmValue = PreProcess(prmValue, prmPreProcess);

            // Check Length
            if (prmLength != 0)
            {
                if (!String.IsNullOrEmpty(prmValue))
                {
                    if (prmValue.Length != prmLength)
                    {
                        prmValue = prmDefault.ToString();
                    }
                }
                else
                {
                    prmValue = prmDefault.ToString();
                }
            }

            // Try to Parse
            bool Converted = SByte.TryParse(prmValue, out Result);
            if (!Converted)
            {
                Result = prmDefault;
            }

            // Check Valid Values
            sbyte ValidValue;
            bool ValidFound = false;
            int CheckCount = 0;

            prmValidValues = ReplaceSplitters(prmValidValues);
            string[] strValidValues = prmValidValues.Split(',');

            foreach (string strValidValue in strValidValues)
            {
                if (!String.IsNullOrEmpty(strValidValue))
                {
                    Converted = SByte.TryParse(strValidValue, out ValidValue);
                    if (Converted)
                    {
                        CheckCount++;

                        if (ValidValue == Result)
                        {
                            ValidFound = true;
                        }
                    }
                }
            }

            if (CheckCount > 0 && !ValidFound)
            {
                Result = prmDefault;
            }

            // Return result;
            return Result;
        }

        public static ushort ToUShort(string prmValue, uint prmLength, string prmValidValues, string prmPreProcess, ushort prmDefault)
        {
            ushort Result;

            // Pre Process
            prmValue = PreProcess(prmValue, prmPreProcess);

            // Check Length
            if (prmLength != 0)
            {
                if (!String.IsNullOrEmpty(prmValue))
                {
                    if (prmValue.Length != prmLength)
                    {
                        prmValue = prmDefault.ToString();
                    }
                }
                else
                {
                    prmValue = prmDefault.ToString();
                }
            }

            // Try to Parse
            bool Converted = ushort.TryParse(prmValue, out Result);
            if (!Converted)
            {
                Result = prmDefault;
            }

            // Check Valid Values
            ushort ValidValue;
            bool ValidFound = false;
            int CheckCount = 0;

            prmValidValues = ReplaceSplitters(prmValidValues);
            string[] strValidValues = prmValidValues.Split(',');

            foreach (string strValidValue in strValidValues)
            {
                if (!String.IsNullOrEmpty(strValidValue))
                {
                    Converted = ushort.TryParse(strValidValue, out ValidValue);
                    if (Converted)
                    {
                        CheckCount++;

                        if (ValidValue == Result)
                        {
                            ValidFound = true;
                        }
                    }
                }
            }

            if (CheckCount > 0 && !ValidFound)
            {
                Result = prmDefault;
            }

            // Return result;
            return Result;
        }

        public static short ToShort(string prmValue, uint prmLength, string prmValidValues, string prmPreProcess, short prmDefault)
        {
            short Result;

            // Pre Process
            prmValue = PreProcess(prmValue, prmPreProcess);

            // Check Length
            if (prmLength != 0)
            {
                if (!String.IsNullOrEmpty(prmValue))
                {
                    if (prmValue.Length != prmLength)
                    {
                        prmValue = prmDefault.ToString();
                    }
                }
                else
                {
                    prmValue = prmDefault.ToString();
                }
            }

            // Try to Parse
            bool Converted = short.TryParse(prmValue, out Result);
            if (!Converted)
            {
                Result = prmDefault;
            }

            // Check Valid Values
            short ValidValue;
            bool ValidFound = false;
            int CheckCount = 0;

            prmValidValues = ReplaceSplitters(prmValidValues);
            string[] strValidValues = prmValidValues.Split(',');

            foreach (string strValidValue in strValidValues)
            {
                if (!String.IsNullOrEmpty(strValidValue))
                {
                    Converted = short.TryParse(strValidValue, out ValidValue);
                    if (Converted)
                    {
                        CheckCount++;

                        if (ValidValue == Result)
                        {
                            ValidFound = true;
                        }
                    }
                }
            }

            if (CheckCount > 0 && !ValidFound)
            {
                Result = prmDefault;
            }

            // Return result;
            return Result;
        }

        public static uint ToUInt(string prmValue, uint prmLength, string prmValidValues, string prmPreProcess, uint prmDefault)
        {
            uint Result;

            // Pre Process
            prmValue = PreProcess(prmValue, prmPreProcess);

            // Check Length
            if (prmLength != 0)
            {
                if (!String.IsNullOrEmpty(prmValue))
                {
                    if (prmValue.Length != prmLength)
                    {
                        prmValue = prmDefault.ToString();
                    }
                }
                else
                {
                    prmValue = prmDefault.ToString();
                }
            }

            // Try to Parse
            bool Converted = uint.TryParse(prmValue, out Result);
            if (!Converted)
            {
                Result = prmDefault;
            }

            // Check Valid Values
            uint ValidValue;
            bool ValidFound = false;
            int CheckCount = 0;

            prmValidValues = ReplaceSplitters(prmValidValues);
            string[] strValidValues = prmValidValues.Split(',');

            foreach (string strValidValue in strValidValues)
            {
                if (!String.IsNullOrEmpty(strValidValue))
                {
                    Converted = uint.TryParse(strValidValue, out ValidValue);
                    if (Converted)
                    {
                        CheckCount++;

                        if (ValidValue == Result)
                        {
                            ValidFound = true;
                        }
                    }
                }
            }

            if (CheckCount > 0 && !ValidFound)
            {
                Result = prmDefault;
            }

            // Return result;
            return Result;
        }

        public static int ToInt(string prmValue, uint prmLength, string prmValidValues, string prmPreProcess, int prmDefault)
        {
            int Result;

            // Pre Process
            prmValue = PreProcess(prmValue, prmPreProcess);

            // Check Length
            if (prmLength != 0)
            {
                if (!String.IsNullOrEmpty(prmValue))
                {
                    if (prmValue.Length != prmLength)
                    {
                        prmValue = prmDefault.ToString();
                    }
                }
                else
                {
                    prmValue = prmDefault.ToString();
                }
            }

            // Try to Parse
            bool Converted = int.TryParse(prmValue, out Result);
            if (!Converted)
            {
                Result = prmDefault;
            }

            // Check Valid Values
            int ValidValue;
            bool ValidFound = false;
            int CheckCount = 0;

            prmValidValues = ReplaceSplitters(prmValidValues);
            string[] strValidValues = prmValidValues.Split(',');

            foreach (string strValidValue in strValidValues)
            {
                if (!String.IsNullOrEmpty(strValidValue))
                {
                    Converted = int.TryParse(strValidValue, out ValidValue);
                    if (Converted)
                    {
                        CheckCount++;

                        if (ValidValue == Result)
                        {
                            ValidFound = true;
                        }
                    }
                }
            }

            if (CheckCount > 0 && !ValidFound)
            {
                Result = prmDefault;
            }

            // Return result;
            return Result;
        }

        public static ulong ToULong(string prmValue, uint prmLength, string prmValidValues, string prmPreProcess, ulong prmDefault)
        {
            ulong Result;

            // Pre Process
            prmValue = PreProcess(prmValue, prmPreProcess);

            // Check Length
            if (prmLength != 0)
            {
                if (!String.IsNullOrEmpty(prmValue))
                {
                    if (prmValue.Length != prmLength)
                    {
                        prmValue = prmDefault.ToString();
                    }
                }
                else
                {
                    prmValue = prmDefault.ToString();
                }
            }

            // Try to Parse
            bool Converted = ulong.TryParse(prmValue, out Result);
            if (!Converted)
            {
                Result = prmDefault;
            }

            // Check Valid Values
            ulong ValidValue;
            bool ValidFound = false;
            int CheckCount = 0;

            prmValidValues = ReplaceSplitters(prmValidValues);
            string[] strValidValues = prmValidValues.Split(',');

            foreach (string strValidValue in strValidValues)
            {
                if (!String.IsNullOrEmpty(strValidValue))
                {
                    Converted = ulong.TryParse(strValidValue, out ValidValue);
                    if (Converted)
                    {
                        CheckCount++;

                        if (ValidValue == Result)
                        {
                            ValidFound = true;
                        }
                    }
                }
            }

            if (CheckCount > 0 && !ValidFound)
            {
                Result = prmDefault;
            }

            // Return result;
            return Result;
        }

        public static long ToLong(string prmValue, uint prmLength, string prmValidValues, string prmPreProcess, long prmDefault)
        {
            long Result;

            // Pre Process
            prmValue = PreProcess(prmValue, prmPreProcess);

            // Check Length
            if (prmLength != 0)
            {
                if (!String.IsNullOrEmpty(prmValue))
                {
                    if (prmValue.Length != prmLength)
                    {
                        prmValue = prmDefault.ToString();
                    }
                }
                else
                {
                    prmValue = prmDefault.ToString();
                }
            }

            // Try to Parse
            bool Converted = long.TryParse(prmValue, out Result);
            if (!Converted)
            {
                Result = prmDefault;
            }

            // Check Valid Values
            long ValidValue;
            bool ValidFound = false;
            int CheckCount = 0;

            prmValidValues = ReplaceSplitters(prmValidValues);
            string[] strValidValues = prmValidValues.Split(',');

            foreach (string strValidValue in strValidValues)
            {
                if (!String.IsNullOrEmpty(strValidValue))
                {
                    Converted = long.TryParse(strValidValue, out ValidValue);
                    if (Converted)
                    {
                        CheckCount++;

                        if (ValidValue == Result)
                        {
                            ValidFound = true;
                        }
                    }
                }
            }

            if (CheckCount > 0 && !ValidFound)
            {
                Result = prmDefault;
            }

            // Return result;
            return Result;
        }

        public static float ToFloat(string prmValue, uint prmLength, string prmValidValues, string prmPreProcess, float prmDefault)
        {
            float Result;

            // Pre Process
            prmValue = PreProcess(prmValue, prmPreProcess);

            // Check Length
            if (prmLength != 0)
            {
                if (!String.IsNullOrEmpty(prmValue))
                {
                    if (prmValue.Length != prmLength)
                    {
                        prmValue = prmDefault.ToString();
                    }
                }
                else
                {
                    prmValue = prmDefault.ToString();
                }
            }

            // Try to Parse
            bool Converted = float.TryParse(prmValue, out Result);
            if (!Converted)
            {
                Result = prmDefault;
            }

            // Check Valid Values
            float ValidValue;
            bool ValidFound = false;
            int CheckCount = 0;

            prmValidValues = ReplaceSplitters(prmValidValues);
            string[] strValidValues = prmValidValues.Split(',');

            foreach (string strValidValue in strValidValues)
            {
                if (!String.IsNullOrEmpty(strValidValue))
                {
                    Converted = float.TryParse(strValidValue, out ValidValue);
                    if (Converted)
                    {
                        CheckCount++;

                        if (ValidValue == Result)
                        {
                            ValidFound = true;
                        }
                    }
                }
            }

            if (CheckCount > 0 && !ValidFound)
            {
                Result = prmDefault;
            }

            // Return result;
            return Result;
        }

        public static double ToDouble(string prmValue, uint prmLength, string prmValidValues, string prmPreProcess, double prmDefault)
        {
            double Result;

            // Pre Process
            prmValue = PreProcess(prmValue, prmPreProcess);

            // Check Length
            if (prmLength != 0)
            {
                if (!String.IsNullOrEmpty(prmValue))
                {
                    if (prmValue.Length != prmLength)
                    {
                        prmValue = prmDefault.ToString();
                    }
                }
                else
                {
                    prmValue = prmDefault.ToString();
                }
            }

            // Try to Parse
            bool Converted = double.TryParse(prmValue, out Result);
            if (!Converted)
            {
                Result = prmDefault;
            }

            // Check Valid Values
            double ValidValue;
            bool ValidFound = false;
            int CheckCount = 0;

            prmValidValues = ReplaceSplitters(prmValidValues);
            string[] strValidValues = prmValidValues.Split(',');

            foreach (string strValidValue in strValidValues)
            {
                if (!String.IsNullOrEmpty(strValidValue))
                {
                    Converted = double.TryParse(strValidValue, out ValidValue);
                    if (Converted)
                    {
                        CheckCount++;

                        if (ValidValue == Result)
                        {
                            ValidFound = true;
                        }
                    }
                }
            }

            if (CheckCount > 0 && !ValidFound)
            {
                Result = prmDefault;
            }

            // Return result;
            return Result;
        }

        public static decimal ToDecimal(string prmValue, uint prmLength, string prmValidValues, string prmPreProcess, decimal prmDefault)
        {
            decimal Result;

            // Pre Process
            prmValue = PreProcess(prmValue, prmPreProcess);

            // Check Length
            if (prmLength != 0)
            {
                if (!String.IsNullOrEmpty(prmValue))
                {
                    if (prmValue.Length != prmLength)
                    {
                        prmValue = prmDefault.ToString();
                    }
                }
                else
                {
                    prmValue = prmDefault.ToString();
                }
            }

            // Try to Parse
            bool converted = decimal.TryParse(prmValue, out Result);
            if (!converted)
            {
                Result = prmDefault;
            }

            // Check Valid Values
            decimal ValidValue;
            bool ValidFound = false;
            int CheckCount = 0;

            prmValidValues = ReplaceSplitters(prmValidValues);
            string[] strValidValues = prmValidValues.Split(',');

            foreach (string strValidValue in strValidValues)
            {
                if (!String.IsNullOrEmpty(strValidValue))
                {
                    converted = decimal.TryParse(strValidValue, out ValidValue);
                    if (converted)
                    {
                        CheckCount++;

                        if (ValidValue == Result)
                        {
                            ValidFound = true;
                        }
                    }
                }
            }

            if (CheckCount > 0 && !ValidFound)
            {
                Result = prmDefault;
            }

            // Return result;
            return Result;
        }
    }
    #endregion

    ///////////////////////////////////////////////////////////////////////////
    // String functions
    ///////////////////////////////////////////////////////////////////////////
    #region STRINGX...
    public class StringX
    {
        public string String = null;

        public StringX(string prmString)
        {
            this.String = prmString;
        }

        public override string ToString()
        {
            return this.String;
        }

        public static bool operator ==(StringX lhs, string rhs)
        {
            bool Result = false;

            if (Object.ReferenceEquals(lhs, rhs))
            {
                Result = true;
            }

            if (Object.ReferenceEquals(lhs, null) && Object.ReferenceEquals(rhs, null))
            {
                Result = true;
            }

            if (!Object.ReferenceEquals(lhs, null) && !Object.ReferenceEquals(rhs, null))
            {
                if (lhs.String == rhs || Object.ReferenceEquals(lhs.String, rhs))
                {
                    Result = true;
                }
            }

            return Result;
        }

        public static bool operator !=(StringX lhs, string rhs)
        {
            return !(lhs == rhs);
        }

        public static bool operator ==(string lhs, StringX rhs)
        {
            bool Result = false;

            if (Object.ReferenceEquals(lhs, rhs))
            {
                Result = true;
            }

            if (Object.ReferenceEquals(lhs, null) && Object.ReferenceEquals(rhs, null))
            {
                Result = true;
            }

            if (!Object.ReferenceEquals(lhs, null) && !Object.ReferenceEquals(rhs, null))
            {
                if (lhs == rhs.String || Object.ReferenceEquals(lhs, rhs.String))
                {
                    Result = true;
                }
            }

            return Result;
        }

        public static bool operator !=(string lhs, StringX rhs)
        {
            return !(lhs == rhs);
        }

        public static bool operator ==(StringX lhs, StringX rhs)
        {
            bool Result = false;

            if (Object.ReferenceEquals(lhs, rhs))
            {
                Result = true;
            }

            if (Object.ReferenceEquals(lhs, null) && Object.ReferenceEquals(rhs, null))
            {
                Result = true;
            }

            if (!Object.ReferenceEquals(lhs, null) && !Object.ReferenceEquals(rhs, null))
            {
                if (lhs.String == rhs.String)
                {
                    Result = true;
                }
            }

            return Result;
        }

        public static bool operator !=(StringX lhs, StringX rhs)
        {
            return !(lhs == rhs);
        }
        
        public static implicit operator string(StringX value)
        {
            if (Object.ReferenceEquals(value, null))
            {
                return null;
            }
            else
            {
                return value.String;
            }
        }

        public static implicit operator StringX(string value)
        {
            return new StringX(value);
        }

        public override bool Equals(object obj)
        {
            bool Result = false;

            // First, compare StringX to StringX
            if (this.GetType() == obj.GetType() && this is StringX && obj is StringX)
            {
                // Check whether we have same instance
                if (Object.ReferenceEquals(this, obj))
                {
                    Result = true;
                }

                // Check whether both objects are null
                if (Object.ReferenceEquals(this, null) && Object.ReferenceEquals(obj, null))
                {
                    Result = true;
                }

                // Check whether the "contents" are the same
                if (!Object.ReferenceEquals(this, null) && !Object.ReferenceEquals(obj, null))
                {
                    if (this.String == ((StringX)obj).String)
                    {
                        Result = true;
                    }
                }
            }

            // After that, compare StringX to String
            if (this.GetType() != obj.GetType() && this is StringX && obj is String)
            {
                // Check whether we have same instance
                if (Object.ReferenceEquals(this, obj))
                {
                    Result = true;
                }

                // Check whether both objects are null
                if (Object.ReferenceEquals(this, null) && Object.ReferenceEquals(obj, null))
                {
                    Result = true;
                }

                // Check whether the "contents" are the same
                if (!Object.ReferenceEquals(this, null) && !Object.ReferenceEquals(obj, null))
                {
                    if (this.String == ((String)obj))
                    {
                        Result = true;
                    }
                }
            }

            return Result;
        }

        public override int GetHashCode()
        {
            return this.String.GetHashCode();
        }

        public StringX Turkish2English()
        {
            string Result = this.String;

            Result = Result.Replace('Ğ', 'G');
            Result = Result.Replace('Ü', 'U');
            Result = Result.Replace('Ş', 'S');
            Result = Result.Replace('İ', 'I');
            Result = Result.Replace('Ö', 'O');
            Result = Result.Replace('Ç', 'C');

            Result = Result.Replace('ğ', 'g');
            Result = Result.Replace('ü', 'u');
            Result = Result.Replace('ş', 's');
            Result = Result.Replace('ı', 'i');
            Result = Result.Replace('ö', 'o');
            Result = Result.Replace('ç', 'c');

            return new StringX(Result);
        }

        public StringX ToLower(string CultureName = "tr")
        {
            System.Globalization.CultureInfo _CultureInfo = null;

            CultureName = CultureName.ToLower();

            switch (CultureName)
            {
                case "en":
                case "eng":
                    _CultureInfo = new System.Globalization.CultureInfo("en-US");
                    return new StringX(this.String.ToLower(_CultureInfo));

                case "tr":
                case "tur":
                    _CultureInfo = new System.Globalization.CultureInfo("tr-TR");
                    return new StringX(this.String.ToLower(_CultureInfo));

                default:
                    _CultureInfo = new System.Globalization.CultureInfo("tr-TR");
                    return new StringX(this.String.ToLower(_CultureInfo));
            }
        }

        public StringX ToUpper(string CultureName = "tr")
        {
            System.Globalization.CultureInfo _CultureInfo = null;

            CultureName = CultureName.ToLower();

            switch (CultureName)
            {
                case "en":
                case "eng":
                    _CultureInfo = new System.Globalization.CultureInfo("en-US");
                    return new StringX(this.String.ToUpper(_CultureInfo));

                case "tr":
                case "tur":
                    _CultureInfo = new System.Globalization.CultureInfo("tr-TR");
                    return new StringX(this.String.ToUpper(_CultureInfo));

                default:
                    _CultureInfo = new System.Globalization.CultureInfo("tr-TR");
                    return new StringX(this.String.ToUpper(_CultureInfo));
            }
        }

        public StringX Replace(char find, char replace)
        {
            return new StringX(this.String.Replace(find, replace));
        }

        public StringX Replace(string find, string replace)
        {
            return new StringX(this.String.Replace(find, replace));
        }

        public StringX Trim()
        {
            return new StringX(this.String.Trim());
        }

        public StringX TrimLeft()
        {
            return new StringX(this.String.TrimStart());
        }

        public StringX TrimRight()
        {
            return new StringX(this.String.TrimEnd());
        }

        public bool ToBoolean()
        {
            if (this.String == "1" || this.String == "on" || this.String == "true" || this.String == "yes" || this.String == "ok")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public StringX Quote()
        {
            return new StringX("'" + this.String.Replace("'", "''") + "'");
        }

        public StringX SqlQuote()
        {
            if (String.IsNullOrEmpty(this.String))
            {
                return new StringX("null");
            }
            else
            {
                return this.Quote();
            }
        }

        public int Length()
        {
            return this.String.Length;
        }

        public StringX SubString(int StartIndex, int Length)
        {
            return new StringX(this.String.Substring(StartIndex, Length));
        }

        public StringX SubString(int StartIndex)
        {
            return new StringX(this.String.Substring(StartIndex));
        }

        public StringX[] Split(char[] delimiter)
        {
            string[] splitted = this.String.Split(delimiter, StringSplitOptions.None);

            StringX[] result = new StringX[splitted.Length];

            for (uint counter = 0; counter < result.Length; counter++)
            {
                result[counter] = new StringX(splitted[counter]);
            }

            return result;
        }

        public StringX[] Split(string[] delimiter)
        {
            string[] splitted = this.String.Split(delimiter, StringSplitOptions.None);

            StringX[] result = new StringX[splitted.Length];

            for (uint counter = 0; counter < result.Length; counter++)
            {
                result[counter] = new StringX(splitted[counter]);
            }

            return result;
        }

        public StringX Join(char glue, string[] pieces)
        {
            string result = String.Empty;

            for (uint counter = 0; counter < pieces.Length; counter++)
            {
                if (!String.IsNullOrEmpty(pieces[counter]))
                {
                    if (String.IsNullOrEmpty(result))
                    {
                        result = pieces[counter];
                    }
                    else
                    {
                        result += glue + pieces[counter];
                    }
                }
            }

            return new StringX(result);
        }

        public StringX Join(string glue, string[] pieces)
        {
            string result = String.Empty;

            for (uint counter = 0; counter < pieces.Length; counter++)
            {
                if (!String.IsNullOrEmpty(pieces[counter]))
                {
                    if (String.IsNullOrEmpty(result))
                    {
                        result = pieces[counter];
                    }
                    else
                    {
                        result += glue + pieces[counter];
                    }
                }
            }

            return new StringX(result);
        }

        public StringX Empty()
        {
            return new StringX(String.Empty);
        }

        public bool IsEmpty()
        {
            return (this.String == String.Empty || this.String == "" || this.String == null);
        }

        public bool IsNullOrEmpty()
        {
            return String.IsNullOrEmpty(this.String);
        }

        public bool IsNullOrWhiteSpace()
        {
            string value = this.String.Trim();

            return String.IsNullOrEmpty(value);
        }

        public bool Contains(string SubString)
        {
            return this.String.Contains(SubString);
        }

        public int ScanLeft(char SubChar)
        {
            return this.String.IndexOf(SubChar);
        }

        public int ScanLeft(string SubString)
        {
            return this.String.IndexOf(SubString);
        }

        public int ScanRight(char SubChar)
        {
            return this.String.LastIndexOf(SubChar);
        }

        public int ScanRight(string SubString)
        {
            return this.String.LastIndexOf(SubString);
        }

        public char[] ToCharArray()
        {
            return this.String.ToCharArray();
        }

        public bool IsValidEmailRegex()
        {
            // Shortest possible email address: a@a.co
            if (this.IsNullOrWhiteSpace() || this.Length() < 6)
            {
                return false;
            }
            else
            {
                string strRegex = @"^((([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+(\.([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+)*)|((\x22)((((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(([\x01-\x08\x0b\x0c\x0e-\x1f\x7f]|\x21|[\x23-\x5b]|[\x5d-\x7e]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(\\([\x01-\x09\x0b\x0c\x0d-\x7f]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]))))*(((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(\x22)))@((([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.)+(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.?$";
                Regex re = new Regex(strRegex, RegexOptions.Compiled | RegexOptions.IgnoreCase);
                return re.IsMatch(this.String);
            }
        }

        public bool IsValidEmail()
        {
            // Shortest possible email address: a@a.co
            if (this.IsNullOrWhiteSpace() || this.Length() < 6)
            {
                return false;
            }
            else
            {
                int PositionOfAt = this.String.IndexOf('@');
                int PositionOfDot = this.String.LastIndexOf('.');

                return (PositionOfAt > 0 && PositionOfAt + 1 < PositionOfDot);
            }
        }

        public bool IsValidUrl()
        {
            Uri uri;

            return Uri.TryCreate(this.String, UriKind.Absolute, out uri) && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
        }
        
        protected bool IsValidAlphaNumeric(char AlphaNumeric)
        {
            bool LowerAlpha = (AlphaNumeric >= 'a' && AlphaNumeric <= 'z');
            bool UpperAlpha = (AlphaNumeric >= 'A' && AlphaNumeric <= 'Z');
            bool Numeric = (AlphaNumeric >= '0' && AlphaNumeric <= '9');
            
            return (LowerAlpha || UpperAlpha || Numeric);   
        }

        public bool IsValidAlphaNumeric()
        {
            bool result = true;

            char[] Chars = this.String.ToCharArray();

            for (uint counter = 0; counter < Chars.Length; counter++)
            {
                if (!IsValidAlphaNumeric(Chars[counter])) { result = false; break; }    
            }

            return result;
        }

        protected bool IsValidDecimal(char Decimal)
        {
            return (Decimal >= '0' && Decimal <= '9');
        }

        public bool IsValidDecimal()
        {
            bool result = true;

            char[] prmChars = this.String.ToCharArray();

            for (uint counter = 0; counter < prmChars.Length; counter++)
            {
                if (!IsValidDecimal(prmChars[counter])) { result = false; break; }
            }

            return result;
        }

        protected bool IsValidHex(char Hex)
        {
            bool IsNumeric = (Hex >= '0' && Hex <= '9');
            bool IsAlphaUpper = (Hex >= 'A' && Hex <= 'F');
            bool IsAlphaLower = (Hex >= 'a' && Hex <= 'f');

            return (IsNumeric || IsAlphaUpper || IsAlphaLower);
        }

        public bool IsValidHex()
        {
            bool result = true;

            char[] prmChars = this.String.ToUpper().ToCharArray();

            for (uint counter = 0; counter < prmChars.Length; counter++)
            {
                if (!IsValidHex(prmChars[counter])) { result = false; break; }
            }

            return result;
        }

        public bool IsValidGuid()
        {
            bool result = false;

            if (this.Length() == 32)
            {
                result = this.IsValidHex();
            }

            if (this.Length() == 36)
            {
                StringX guidShort = this.Replace("-", "");

                result = (this.Length() == 36 && guidShort.Length() == 32 && guidShort.IsValidHex() &&
                          this.SubString(8, 1) == "-" && this.SubString(13, 1) == "-" &&
                          this.SubString(18, 1) == "-" && this.SubString(23, 1) == "-");
            }

            return result;
        }

        public bool IsValidIP()
        {
            return (IsValidIPv4() || IsValidIPv6());
        }

        public bool IsValidIPv4()
        {
            bool result = true;
            byte IPv4Byte = 0;

            string[] IPv4Addresses = this.String.Split('.');

            if (IPv4Addresses.Length == 4)
            {
                for (int counter = 0; counter < IPv4Addresses.Length; counter++)
                {
                    IPv4Addresses[counter] = IPv4Addresses[counter].PadLeft(3, '0');

                    if (IPv4Addresses[counter].Length == 3)
                    {
                        if (!Byte.TryParse(IPv4Addresses[counter], out IPv4Byte))
                        {
                            result = false;
                        }
                    }
                    else
                    {
                        result = false;
                    }
                }
            }
            else
            {
                result = false;
            }

            return result;
        }

        public bool IsValidIPv6()
        {
            bool flag = false;

            if (!String.IsNullOrWhiteSpace(this.String))
            {
                IPAddress IPv6;

                if (IPAddress.TryParse(this.String, out IPv6))
                {
                    flag = (IPv6.AddressFamily == AddressFamily.InterNetworkV6);
                }
            }

            return flag;
        }

        public bool IsValidPhoneNumber()
        {
            return IsValidDecimal() && (this.String.Length == 10);
        }

        public bool IsValidPhoneCode()
        {
            return IsValidDecimal() && (this.String.Length == 6);
        }

        public bool IsValidPassword()
        {
            return (this.String.Length >= 8);
        }

        public string ToMD5()
        {
            MD5CryptoServiceProvider MD5 = new MD5CryptoServiceProvider();

            byte[] ByteArray = System.Text.Encoding.Unicode.GetBytes(this.String);
            ByteArray = MD5.ComputeHash(ByteArray);

            StringBuilder sb = new StringBuilder();

            foreach (byte Byte in ByteArray)
            {
                sb.Append(Byte.ToString("x2").ToLower());
            }

            return sb.ToString();
        }
    }
    #endregion

    ///////////////////////////////////////////////////////////////////////////
    // Date & Time functions
    ///////////////////////////////////////////////////////////////////////////
    #region DATEX...
    public class DateX
    {
        protected const short SQLYearMin = 1900;

        protected sbyte _day = -1;
        protected sbyte _month = -1;
        protected short _year = -1;
        protected sbyte _hour = -1;
        protected sbyte _minute = -1;
        protected sbyte _second = -1;
        protected short _millisecond = -1;

        protected void CheckDateLimits(string CheckType)
        {
            CheckType = CheckType.ToLower();

            if (CheckType == "short" || CheckType == "long")
            {
                if (CheckType == "short")
                {
                    if (this._day < 1 || this._day > 31)
                    {
                        this._day = -1;
                    }

                    if (this._month < 1 || this._month > 12)
                    {
                        this._month = -1;
                    }

                    if (this._year < SQLYearMin)
                    {
                        this._year = -1;
                    }

                    if (this._day == -1 || this._month == -1 || this._year == -1)
                    {
                        this._day = -1;
                        this._month = -1;
                        this._year = -1;
                    }
                }

                if (CheckType == "long")
                {
                    if (this._day < 1 || this._day > 31)
                    {
                        this._day = -1;
                    }

                    if (this._month < 1 || this._month > 12)
                    {
                        this._month = -1;
                    }

                    if (this._year < SQLYearMin)
                    {
                        this._year = -1;
                    }

                    if (this._hour < 0 || this._hour > 23)
                    {
                        this._hour = -1;
                    }

                    if (this._minute < 0 || this._minute > 59)
                    {
                        this._minute = -1;
                    }

                    if (this._second < 0 || this._second > 59)
                    {
                        this._second = -1;
                    }

                    if (this._millisecond < 0 || this._millisecond > 999)
                    {
                        this._millisecond = -1;
                    }

                    if (this._day == -1 || this._month == -1 || this._year == -1 || this._hour == -1 || this._minute == -1 || this._second == -1 || this._millisecond == -1)
                    {
                        this._day = -1;
                        this._month = -1;
                        this._year = -1;
                        this._hour = -1;
                        this._minute = -1;
                        this._second = -1;
                        this._millisecond = -1;
                    }
                }
            }
            else
            {
                this._day = -1;
                this._month = -1;
                this._year = -1;
                this._hour = -1;
                this._minute = -1;
                this._second = -1;
                this._millisecond = -1;
            }
        }
        
        public DateX()
        {
            this._day = -1;
            this._month = -1;
            this._year = -1;
            this._hour = -1;
            this._minute = -1;
            this._second = -1;
            this._millisecond = -1;
        }

        public DateX(sbyte Day, sbyte Month, short Year)
        {
            this._day = Day;
            this._month = Month;
            this._year = Year;

            this.CheckDateLimits("short");
        }

        public DateX(sbyte Day, sbyte Month, short Year, sbyte Hour, sbyte Minute, sbyte Second, short MilliSecond)
        {
            this._day = Day;
            this._month = Month;
            this._year = Year;
            this._hour = Hour;
            this._minute = Minute;
            this._second = Second;
            this._millisecond = MilliSecond;

            this.CheckDateLimits("long");
        }

        public DateX(DateTime prmDateTime)
        {
            this._day = (sbyte)prmDateTime.Day;
            this._month = (sbyte)prmDateTime.Month;
            this._year = (short)prmDateTime.Year;
            this._hour = (sbyte)prmDateTime.Hour;
            this._minute = (sbyte)prmDateTime.Minute;
            this._second = (sbyte)prmDateTime.Second;
            this._millisecond = (short)prmDateTime.Millisecond;

            this.CheckDateLimits("long");
        }

        public DateX(string SqlDate)
        {
            bool converted = false;
            DateX result = null;

            converted = DateX.TryParse(SqlDate, out result);

            if (converted)
            {
                this._year = result._year;
                this._month = result._month;
                this._day = result._day;
                this._hour = result._hour;
                this._minute = result._minute;
                this._second = result._second;
                this._millisecond = result._millisecond;
            }
            else
            {
                this._year = -1;
                this._month = -1;
                this._day = -1;
                this._hour = -1;
                this._minute = -1;
                this._second = -1;
                this._millisecond = -1;
            }

            this.CheckDateLimits("long");
        }

        public static bool TryParse(string SqlDate, out DateX result)
        {
            result = new DateX();
            bool SqlDateFormatFound = false;
            
            if (SqlDate == null)
            {
                result._year = -1;
                result._month = -1;
                result._day = -1;
                result._hour = -1;
                result._minute = -1;
                result._second = -1;
                result._millisecond = -1;

                return true;
            }
            else
            {
                //                  01234567890123456789012
                // SQL Date Format: 20130706                Length: 08 - ok
                if (SqlDate.Length == 8)
                {
                    SqlDateFormatFound = true;

                    result._year = ConvertX.ToShort(SqlDate.Substring(0, 4), 4, "", "trim,striptags", -1);
                    result._month = ConvertX.ToSByte(SqlDate.Substring(4, 2), 2, "", "trim,striptags", -1);
                    result._day = ConvertX.ToSByte(SqlDate.Substring(6, 2), 2, "", "trim,striptags", -1);
                    result._hour = 0;
                    result._minute = 0;
                    result._second = 0;
                    result._millisecond = 0;
                }

                //                  01234567890123456789012
                // SQL Date Format: 2013-07-06              Length: 10 - ok
                if (SqlDate.Length == 10)
                {
                    SqlDateFormatFound = true;

                    result._year = ConvertX.ToShort(SqlDate.Substring(0, 4), 4, "", "trim,striptags", -1);
                    result._month = ConvertX.ToSByte(SqlDate.Substring(5, 2), 2, "", "trim,striptags", -1);
                    result._day = ConvertX.ToSByte(SqlDate.Substring(8, 2), 2, "", "trim,striptags", -1);
                    result._hour = 0;
                    result._minute = 0;
                    result._second = 0;
                    result._millisecond = 0;
                }

                //                  01234567890123456789012
                // SQL Date Format: 201307061324            Length: 12 - ok 
                if (SqlDate.Length == 12)
                {
                    SqlDateFormatFound = true;

                    result._year = ConvertX.ToShort(SqlDate.Substring(0, 4), 4, "", "trim,striptags", -1);
                    result._month = ConvertX.ToSByte(SqlDate.Substring(4, 2), 2, "", "trim,striptags", -1);
                    result._day = ConvertX.ToSByte(SqlDate.Substring(6, 2), 2, "", "trim,striptags", -1);
                    result._hour = ConvertX.ToSByte(SqlDate.Substring(8, 2), 2, "", "trim,striptags", -1);
                    result._minute = ConvertX.ToSByte(SqlDate.Substring(10, 2), 2, "", "trim,striptags", -1);
                    result._second = 0;
                    result._millisecond = 0;
                }

                //                  01234567890123456789012
                // SQL Date Format: 20130706132456          Length: 14 - ok
                if (SqlDate.Length == 14)
                {
                    SqlDateFormatFound = true;

                    result._year = ConvertX.ToShort(SqlDate.Substring(0, 4), 4, "", "trim,striptags", -1);
                    result._month = ConvertX.ToSByte(SqlDate.Substring(4, 2), 2, "", "trim,striptags", -1);
                    result._day = ConvertX.ToSByte(SqlDate.Substring(6, 2), 2, "", "trim,striptags", -1);
                    result._hour = ConvertX.ToSByte(SqlDate.Substring(8, 2), 2, "", "trim,striptags", -1);
                    result._minute = ConvertX.ToSByte(SqlDate.Substring(10, 2), 2, "", "trim,striptags", -1);
                    result._second = ConvertX.ToSByte(SqlDate.Substring(12, 2), 2, "", "trim,striptags", -1);
                    result._millisecond = 0;
                }

                //                  01234567890123456789012
                // SQL Date Format: 20130706 132456         Length: 15 - ok
                if (SqlDate.Length == 15)
                {
                    SqlDateFormatFound = true;

                    result._year = ConvertX.ToShort(SqlDate.Substring(0, 4), 4, "", "trim,striptags", -1);
                    result._month = ConvertX.ToSByte(SqlDate.Substring(4, 2), 2, "", "trim,striptags", -1);
                    result._day = ConvertX.ToSByte(SqlDate.Substring(6, 2), 2, "", "trim,striptags", -1);
                    result._hour = ConvertX.ToSByte(SqlDate.Substring(9, 2), 2, "", "trim,striptags", -1);
                    result._minute = ConvertX.ToSByte(SqlDate.Substring(11, 2), 2, "", "trim,striptags", -1);
                    result._second = ConvertX.ToSByte(SqlDate.Substring(13, 2), 2, "", "trim,striptags", -1);
                    result._millisecond = 0;
                }

                //                  01234567890123456789012
                // SQL Date Format: 2013-07-06 13:24        Length: 16 - ok
                if (SqlDate.Length == 16)
                {
                    SqlDateFormatFound = true;

                    result._year = ConvertX.ToShort(SqlDate.Substring(0, 4), 4, "", "trim,striptags", -1);
                    result._month = ConvertX.ToSByte(SqlDate.Substring(5, 2), 2, "", "trim,striptags", -1);
                    result._day = ConvertX.ToSByte(SqlDate.Substring(8, 2), 2, "", "trim,striptags", -1);
                    result._hour = ConvertX.ToSByte(SqlDate.Substring(11, 2), 2, "", "trim,striptags", -1);
                    result._minute = ConvertX.ToSByte(SqlDate.Substring(14, 2), 2, "", "trim,striptags", -1);
                    result._second = 0;
                    result._millisecond = 0;
                }

                //                  01234567890123456789012
                // SQL Date Format: 20130706132456843       Length: 17 - ok
                if (SqlDate.Length == 17)
                {
                    SqlDateFormatFound = true;

                    result._year = ConvertX.ToShort(SqlDate.Substring(0, 4), 4, "", "trim,striptags", -1);
                    result._month = ConvertX.ToSByte(SqlDate.Substring(4, 2), 2, "", "trim,striptags", -1);
                    result._day = ConvertX.ToSByte(SqlDate.Substring(6, 2), 2, "", "trim,striptags", -1);
                    result._hour = ConvertX.ToSByte(SqlDate.Substring(8, 2), 2, "", "trim,striptags", -1);
                    result._minute = ConvertX.ToSByte(SqlDate.Substring(10, 2), 2, "", "trim,striptags", -1);
                    result._second = ConvertX.ToSByte(SqlDate.Substring(12, 2), 2, "", "trim,striptags", -1);
                    result._millisecond = ConvertX.ToShort(SqlDate.Substring(14, 3), 3, "", "trim,striptags", -1);
                }

                //                  01234567890123456789012
                // SQL Date Format: 20130706 132456843      Length: 18 - ok
                if (SqlDate.Length == 18)
                {
                    SqlDateFormatFound = true;

                    result._year = ConvertX.ToShort(SqlDate.Substring(0, 4), 4, "", "trim,striptags", -1);
                    result._month = ConvertX.ToSByte(SqlDate.Substring(4, 2), 2, "", "trim,striptags", -1);
                    result._day = ConvertX.ToSByte(SqlDate.Substring(6, 2), 2, "", "trim,striptags", -1);
                    result._hour = ConvertX.ToSByte(SqlDate.Substring(9, 2), 2, "", "trim,striptags", -1);
                    result._minute = ConvertX.ToSByte(SqlDate.Substring(11, 2), 2, "", "trim,striptags", -1);
                    result._second = ConvertX.ToSByte(SqlDate.Substring(13, 2), 2, "", "trim,striptags", -1);
                    result._millisecond = ConvertX.ToShort(SqlDate.Substring(15, 3), 3, "", "trim,striptags", -1);
                }

                //                  01234567890123456789012
                // SQL Date Format: 2013-07-06 13:24:56     Length: 19 - ok
                if (SqlDate.Length == 19)
                {
                    SqlDateFormatFound = true;

                    result._year = ConvertX.ToShort(SqlDate.Substring(0, 4), 4, "", "trim,striptags", -1);
                    result._month = ConvertX.ToSByte(SqlDate.Substring(5, 2), 2, "", "trim,striptags", -1);
                    result._day = ConvertX.ToSByte(SqlDate.Substring(8, 2), 2, "", "trim,striptags", -1);
                    result._hour = ConvertX.ToSByte(SqlDate.Substring(11, 2), 2, "", "trim,striptags", -1);
                    result._minute = ConvertX.ToSByte(SqlDate.Substring(14, 2), 2, "", "trim,striptags", -1);
                    result._second = ConvertX.ToSByte(SqlDate.Substring(17, 2), 2, "", "trim,striptags", -1);
                    result._millisecond = 0;
                }

                //                  01234567890123456789012
                // SQL Date Format: 2013-07-06 13:24:56.843 Length: 23 - ok
                if (SqlDate.Length == 23)
                {
                    SqlDateFormatFound = true;

                    result._year = ConvertX.ToShort(SqlDate.Substring(0, 4), 4, "", "trim,striptags", -1);
                    result._month = ConvertX.ToSByte(SqlDate.Substring(5, 2), 2, "", "trim,striptags", -1);
                    result._day = ConvertX.ToSByte(SqlDate.Substring(8, 2), 2, "", "trim,striptags", -1);
                    result._hour = ConvertX.ToSByte(SqlDate.Substring(11, 2), 2, "", "trim,striptags", -1);
                    result._minute = ConvertX.ToSByte(SqlDate.Substring(14, 2), 2, "", "trim,striptags", -1);
                    result._second = ConvertX.ToSByte(SqlDate.Substring(17, 2), 2, "", "trim,striptags", -1);
                    result._millisecond = ConvertX.ToShort(SqlDate.Substring(20, 3), 3, "", "trim,striptags", -1);
                }

                if (SqlDateFormatFound)
                {
                    if (result._day < 1 || result._day > 31)
                    {
                        result._day = -1;
                    }

                    if (result._month < 1 || result._month > 12)
                    {
                        result._month = -1;
                    }

                    if (result._year < SQLYearMin)
                    {
                        result._year = -1;
                    }

                    if (result._hour < 0 || result._hour > 23)
                    {
                        result._hour = -1;
                    }

                    if (result._minute < 0 || result._minute > 59)
                    {
                        result._minute = -1;
                    }

                    if (result._second < 0 || result._second > 59)
                    {
                        result._second = -1;
                    }

                    if (result._millisecond < 0 || result._millisecond > 999)
                    {
                        result._millisecond = -1;
                    }

                    if (result._day == -1 || result._month == -1 || result._year == -1 || result._hour == -1 || result._minute == -1 || result._second == -1 || result._millisecond == -1)
                    {
                        result = null;
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                
                }
                else
                {
                    result = null;
                    return false;
                }
            }
        }

        public DateTime ToShortDateTime()
        {
            if (this._year == -1 || this._month == -1 || this._day == -1)
            {
                return new DateTime(SQLYearMin, 1, 1);
            }
            else
            {
                return new DateTime(this._year, this._month, this._day);
            }
        }

        public DateTime ToLongDateTime()
        {
            if (this._year == -1 || this._month == -1 || this._day == -1 || this._hour == -1 || this._minute == -1 || this._second == -1 || this._millisecond == -1)
            {
                return new DateTime(SQLYearMin, 1, 1, 0, 0, 0, 0);
            }
            else
            {
                return new DateTime(this._year, this._month, this._day, this._hour, this._minute, this._second, this._millisecond);
            }
        }

        public StringX ToSQLShortDate()
        {
            if (this._day == -1 || this._month == -1 || this._year == -1)
            {
                return null;
            }
            else
            {
                string strDay = this._day.ToString().PadLeft(2, '0');
                string strMonth = this._month.ToString().PadLeft(2, '0');
                string strYear = this._year.ToString().PadLeft(4, '0');

                return strYear + "-" + strMonth + "-" + strDay;
            }
        }

        public StringX ToSQLLongDate()
        {
            if (this._day == -1 || this._month == -1 || this._year == -1 || this._hour == -1 || this._minute == -1 || this._second == -1 || this._millisecond == -1)
            {
                return null;
            }
            else
            {
                string strDay = this._day.ToString().PadLeft(2, '0');
                string strMonth = this._month.ToString().PadLeft(2, '0');
                string strYear = this._year.ToString().PadLeft(4, '0');
                string strHour = this._hour.ToString().PadLeft(2, '0');
                string strMinute = this._minute.ToString().PadLeft(2, '0');
                string strSecond = this._second.ToString().PadLeft(2, '0');
                string strMilliSecond = this._millisecond.ToString().PadLeft(3, '0');

                return strYear + "-" + strMonth + "-" + strDay + " " + strHour + ":" + strMinute + ":" + strSecond + "." + strMilliSecond;
            }
        }

        public StringX ToTurkishShortDate()
        {
            if (this._day == -1 || this._month == -1 || this._year == -1)
            {
                return null;
            }
            else
            {
                string strDay = this._day.ToString().PadLeft(2, '0');
                string strMonth = this._month.ToString().PadLeft(2, '0');
                string strYear = this._year.ToString().PadLeft(4, '0');

                return strDay + "-" + strMonth + "-" + strYear;
            }
        }

        public override string ToString()
        {
            return this.ToSQLLongDate();
        }
    }
    #endregion

    ///////////////////////////////////////////////////////////////////////////
    // HTML functions
    ///////////////////////////////////////////////////////////////////////////
    #region HTMLX...
    public static class HtmlX
    {
        ///////////////////////////////////////////////////////////////////////////
        // HTML Cleaning functions
        ///////////////////////////////////////////////////////////////////////////
        public static string StripTags(string source)
        {
            char[] array = new char[source.Length];
            int arrayIndex = 0;
            bool inside = false;

            for (int i = 0; i < source.Length; i++)
            {
                char let = source[i];

                if (let == '<')
                {
                    inside = true;
                    continue;
                }
                if (let == '>')
                {
                    inside = false;
                    continue;
                }
                if (!inside)
                {
                    array[arrayIndex] = let;
                    arrayIndex++;
                }
            }

            return new string(array, 0, arrayIndex);
        }

        public static string CleanWord(string html)
        {
            html = html.Trim();

            if (String.IsNullOrEmpty(html))
            {
                html = "";
            }

            // start by completely removing all unwanted tags     
            html = Regex.Replace(html, @"<[/]?(img|span|xml|del|ins|[ovwxp]:\w+)[^>]*?>", "", RegexOptions.IgnoreCase);
            // then run another pass over the html (twice), removing unwanted attributes     
            html = Regex.Replace(html, @"<([^>]*)(?:class|lang|style|size|face|[ovwxp]:\w+)=(?:'[^']*'|""[^""]*""|[^\s>]+)([^>]*)>", "<$1$2>", RegexOptions.IgnoreCase);
            html = Regex.Replace(html, @"<([^>]*)(?:class|lang|style|size|face|[ovwxp]:\w+)=(?:'[^']*'|""[^""]*""|[^\s>]+)([^>]*)>", "<$1$2>", RegexOptions.IgnoreCase);

            return html;
        }

        ///////////////////////////////////////////////////////////////////////////
        // HTML IsEmpty functions
        ///////////////////////////////////////////////////////////////////////////
        public static bool IsEmpty(byte value1)
        {
            return (value1 == 0);
        }

        public static bool IsEmpty(sbyte value1)
        {
            return (value1 == 0);
        }

        public static bool IsEmpty(short value1)
        {
            return (value1 == 0);
        }

        public static bool IsEmpty(ushort value1)
        {
            return (value1 == 0);
        }

        public static bool IsEmpty(int value1)
        {
            return (value1 == 0);
        }

        public static bool IsEmpty(uint value1)
        {
            return (value1 == 0);
        }

        public static bool IsEmpty(long value1)
        {
            return (value1 == 0);
        }

        public static bool IsEmpty(ulong value1)
        {
            return (value1 == 0);
        }

        public static bool IsEmpty(decimal value1)
        {
            return (value1 == 0);
        }

        public static bool IsEmpty(float value1)
        {
            return (value1 == 0);
        }

        public static bool IsEmpty(double value1)
        {
            return (value1 == 0);
        }

        public static bool IsEmpty(char value1)
        {
            return (value1 == ' ' || value1 == '\0' || value1 == '0');
        }

        public static bool IsEmpty(string value1)
        {
            value1 = value1.Trim();

            return (String.IsNullOrEmpty(value1) || value1 == "0");
        }

        public static bool IsEmpty(object value1)
        {
            return (value1 == null || value1 is DBNull);
        }

        ///////////////////////////////////////////////////////////////////////////
        // HTML Checkbox Helper
        ///////////////////////////////////////////////////////////////////////////
        public static string IsCheckBoxChecked (bool IsChecked)
        {
            if (IsChecked)
            {
                return "checked";
            }
            else
            {
                return "";
            }
        }

        public static string ConvertSubZeroFormInteger (int Value)
        {
            if (Value < 0) { return ""; }
            else { return Value.ToString(); }
        }

        public static string ConvertPackageBoolean (bool Checkbox)
        {
            if (Checkbox) { return " (ok) "; }
            else { return " (x) "; }
        }
    }
    #endregion

    ///////////////////////////////////////////////////////////////////////////
    // Internet functions
    ///////////////////////////////////////////////////////////////////////////
    #region NETX...
    public static class NetX
    {
        public static bool IsConnectedToInternet()
        {
            Ping PingGoogleDNS = new Ping();

            return ((PingGoogleDNS.Send("8.8.8.8").Status == IPStatus.Success) || (PingGoogleDNS.Send("8.8.4.4").Status == IPStatus.Success));
        }

        public static string GetClientIPAddress(HttpContext Context)
        {
            string IPAddress = null;

            IPAddress = Context.Connection.RemoteIpAddress.ToString();
            if (!string.IsNullOrEmpty(IPAddress))
            {
                return IPAddress;
            }

            IPAddress = Context.GetServerVariable("HTTP_X_FORWARDED_FOR");
            if (!string.IsNullOrEmpty(IPAddress))
            {
                string[] IPAddresses = IPAddress.Split(',');
                if (IPAddresses.Length != 0) { return IPAddresses[0]; }
            }

            IPAddress = Context.GetServerVariable("REMOTE_ADDR");
            if (!string.IsNullOrEmpty(IPAddress))
            {
                return IPAddress;
            }

            IPAddress = Context.GetServerVariable("HTTP_CLIENT_IP");
            if (!string.IsNullOrEmpty(IPAddress))
            {
                return IPAddress;
            }

            return String.Empty;

        }

        public static string GetServerIPAddress(HttpContext Context)
        {
            string IPAddress = null;

            IPAddress = Context.GetServerVariable("LOCAL_ADDR");
            if (!string.IsNullOrEmpty(IPAddress))
            {
                return IPAddress;
            }

            return String.Empty;
        }

        public static string GetDomainNameFromInternetAddress (string InternetAddress)
        {
            string Name = InternetAddress;

            if (Name.StartsWith("https://", StringComparison.InvariantCultureIgnoreCase))
            {
                Name = Name.Substring("https://".Length);
            }

            if (Name.StartsWith("http://", StringComparison.InvariantCultureIgnoreCase))
            {
                Name = Name.Substring("http://".Length);
            }

            if (Name.StartsWith("ftp://", StringComparison.InvariantCultureIgnoreCase))
            {
                Name = Name.Substring("ftp://".Length);
            }

            int SlashPos = Name.IndexOf("/", StringComparison.InvariantCultureIgnoreCase);
            if (SlashPos >= 0)
            {
                Name = Name.Substring(0, SlashPos);
            }

            while (Name.IndexOf('.') != Name.LastIndexOf('.'))
            {
                int DotPos = Name.IndexOf('.');
                Name = Name.Substring(DotPos + 1);
            }

            Name = Name.Trim().Trim('.').ToLower();

            return Name;
        }

        public static string GetTopLevelDomainFromDomainName (string DomainName)
        {
            return DomainName.Substring(DomainName.LastIndexOf(".") + 1).Trim().Trim('.').ToLower();
        }

        public static List<string> GetIPAddressesFromDomainName(string DomainName)
        {
            List<string> DomainIPs = new List<string>();

            IPHostEntry DomainInfo = Dns.GetHostEntry(DomainName);

            foreach (IPAddress DomainIP in DomainInfo.AddressList)
            {
                DomainIPs.Add(DomainIP.ToString());
            }

            return DomainIPs;
        }

        public static void MailSend(string UserFullName, string UserEmail, string MailSubject, string MailHtmlBody, List<string> Attachments)
        {
            var AppSettings = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

            string SMTPServer = AppSettings.GetValue<string>("MailSettings:SMTPServer");
            int    SMTPPort = AppSettings.GetValue<int>("MailSettings:SMTPPort");
            string SMTPEmail = AppSettings.GetValue<string>("MailSettings:SMTPEmail");
            string SMTPPassword = AppSettings.GetValue<string>("MailSettings:SMTPPassword");

            string SMTPFromName = AppSettings.GetValue<string>("MailSettings:SMTPFromName");
            string SMTPFromEmail = AppSettings.GetValue<string>("MailSettings:SMTPFromEmail");
            string SMTPReplyToName = AppSettings.GetValue<string>("MailSettings:SMTPReplyToName");
            string SMTPReplyToEmail = AppSettings.GetValue<string>("MailSettings:SMTPReplyToEmail");

            if (IsConnectedToInternet())
            {
                    var Mail = new MimeMessage();

                    Mail.From.Add(new MailboxAddress(SMTPFromName, SMTPFromEmail));
                    Mail.ReplyTo.Add(new MailboxAddress(SMTPReplyToName, SMTPReplyToEmail));
                    Mail.To.Add(new MailboxAddress(UserFullName, UserEmail));

                    Mail.Subject = MailSubject;

                    var BodyBuilder = new BodyBuilder();
                    BodyBuilder.HtmlBody = MailHtmlBody;

                    if (Attachments != null)
                    {
                        foreach (string FullFilePath in Attachments)
                        {
                            if (File.Exists(FullFilePath))
                            {
                                BodyBuilder.Attachments.Add(FullFilePath);
                            }
                        }
                    }

                    Mail.Body = BodyBuilder.ToMessageBody();

                    SmtpClient MailClient = new SmtpClient();
                    MailClient.Connect(SMTPServer, SMTPPort, SecureSocketOptions.Auto);
                    MailClient.Authenticate(SMTPEmail, SMTPPassword);
                    MailClient.Send(Mail);
                    MailClient.Disconnect(true);
            }
        }

        public static string GetYoutubeVideoId (StringX YoutubeVideoLink)
        {
            string YoutubeVideoId = null;

            if (YoutubeVideoLink.IsValidUrl())
            {
                Uri YoutubeUri = new Uri(YoutubeVideoLink);

                if (YoutubeUri.Scheme == "http" || YoutubeUri.Scheme == "https")
                {
                    if (YoutubeUri.Host == "www.youtube.com" || YoutubeUri.Host == "youtube.com")
                    {
                        var QueryStringParsed = HttpUtility.ParseQueryString(YoutubeUri.Query);

                        YoutubeVideoId = QueryStringParsed["v"];
                    }

                    if (YoutubeUri.Host == "www.youtu.be" || YoutubeUri.Host == "youtu.be")
                    {
                        if (YoutubeUri.AbsolutePath.StartsWith("/", StringComparison.InvariantCultureIgnoreCase))
                        {
                            YoutubeVideoId = YoutubeUri.AbsolutePath.Substring(1);
                        }
                        else
                        {
                            YoutubeVideoId = YoutubeUri.AbsolutePath;
                        }
                    }
                }
            }

            return YoutubeVideoId;
        }

        public static string GetYoutubeVideoLink (string YoutubeVideoId)
        {
            const string YoutubeVideoLink = "https://www.youtube.com/watch?v={*VIDEO_ID*}";

            if (!String.IsNullOrEmpty(YoutubeVideoId))
            {
                return YoutubeVideoLink.Replace("{*VIDEO_ID*}", YoutubeVideoId);
            }
            else
            {
                return "";
            }
        }

        public static string GetYoutubeVideoEmbedLink (string YoutubeVideoId)
        {
            const string YoutubeEmbedLink = "https://www.youtube.com/embed/{*VIDEO_ID*}?autoplay=0&mute=0&loop=0&controls=1";

            if (!String.IsNullOrEmpty(YoutubeVideoId))
            {
                return YoutubeEmbedLink.Replace("{*VIDEO_ID*}", YoutubeVideoId);
            }
            else
            {
                return "";
            }
        }
    }
    #endregion

    ///////////////////////////////////////////////////////////////////////////
    // API functions
    ///////////////////////////////////////////////////////////////////////////
    #region APIX...
    public static class APIX
    {
        public static string Base64Encode(byte[] ByteArray)
        {
            return Convert.ToBase64String(ByteArray);
        }

        public static string Base64Encode(string UTF8String)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(UTF8String));
        }

        public static byte[] Base64Decode(string Base64String)
        {
            return Convert.FromBase64String(Base64String);
        }

        public static byte[] GZipCompress(byte[] ByteArray)
        {
            MemoryStream StreamOriginal = new MemoryStream(ByteArray);
            MemoryStream StreamGZipped = new MemoryStream();

            GZipStream GZipCompress = new GZipStream(StreamGZipped, CompressionLevel.Optimal);
            StreamOriginal.CopyTo(GZipCompress);
            
            GZipCompress.Close();
            GZipCompress.Dispose();

            StreamOriginal.Close();
            StreamOriginal.Dispose();

            return StreamGZipped.ToArray();
        }

        public static byte[] GZipCompress(string UTF8String)
        {
            return GZipCompress(Encoding.UTF8.GetBytes(UTF8String));
        }

        public static byte[] GZipDecompress(byte[] ByteArray)
        {
            MemoryStream StreamGZipped = new MemoryStream(ByteArray);
            MemoryStream StreamGUnzipped = new MemoryStream();

            GZipStream GZipDecompressor = new GZipStream(StreamGZipped, CompressionMode.Decompress);
            GZipDecompressor.CopyTo(StreamGUnzipped);

            GZipDecompressor.Close();
            GZipDecompressor.Dispose();

            StreamGZipped.Close();
            StreamGZipped.Dispose();

            return StreamGUnzipped.ToArray();
        }

        public static string SHA_512(string Data, string Key)
        {
            var DataBytes = Encoding.UTF8.GetBytes(Data);
            var KeyBytes = Encoding.UTF8.GetBytes(Key);

            var Algorithm = new HMACSHA512(KeyBytes);
            var Hash = Algorithm.ComputeHash(DataBytes);

            return Encoding.UTF8.GetString(Hash);
        }
    }
    #endregion
}