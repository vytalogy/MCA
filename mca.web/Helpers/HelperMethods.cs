using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace mca.web.Helpers
{
    public static class HelperMethods
    {
        public static bool IsGUID(this object expression)
        {
            try
            {
                if (expression == null)
                    return false;

                Guid output = Guid.Parse(expression.ToString());
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool IsNumeric(this object expression)
        {
            try
            {
                if (expression == null)
                    return false;

                double result2 = Convert.ToDouble(expression);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool IsDateTime(this object expression)
        {
            try
            {
                DateTime result2 = Convert.ToDateTime(expression.ToString());
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool IsBoolean(this object expression)
        {
            try
            {
                bool result2 = Convert.ToBoolean(expression);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static string ConvertToString(this object expression)
        {
            try
            {
                if (expression != "" || expression != null)
                    return Convert.ToString(expression);
                else
                    return string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }

        public static Guid ConvertToGUID(this object expression)
        {
            try
            {
                if (expression != null)
                    return Guid.Parse(expression.ToString());
                else
                    return Guid.NewGuid();
            }
            catch
            {
                return Guid.NewGuid();
            }
        }

        public static bool ConvertToBool(this object expression)
        {
            try
            {
                if (expression != "")
                    return Convert.ToBoolean(expression);
                else
                    return false;
            }
            catch
            {
                return false;
            }
        }

        public static int ConvertToInt(this object expression)
        {
            try
            {
                if (expression != "")
                    return Convert.ToInt32(expression);
                else
                    return 0;
            }
            catch
            {
                return 0;
            }
        }

        public static double ConvertToDouble(this object expression)
        {
            try
            {
                if (expression != "")
                    return Convert.ToDouble(expression);
                else
                    return 0;
            }
            catch
            {
                return 0;
            }
        }

        public static long ConvertToLong(this object expression)
        {
            try
            {
                if (expression != "")
                    return Convert.ToInt64(expression);
                else
                    return 0;
            }
            catch
            {
                return 0;
            }
        }

        public static decimal ConvertToDecimal(this object expression)
        {
            try
            {
                if (expression != "")
                    return Convert.ToDecimal(expression);
                else
                    return 0;
            }
            catch
            {
                return 0;
            }
        }

        public static DateTime ConvertToDateTime(this object expression)
        {
            try
            {
                if (expression != "")
                    return Convert.ToDateTime(expression);
                else
                    return DateTime.Now;
            }
            catch
            {
                return DateTime.Now;
            }
        }

        public static DateTime ConvertToDate(this object expression)
        {
            try
            {
                if (expression != "")
                    return Convert.ToDateTime(expression);
                else
                    return DateTime.Now.Date;
            }
            catch
            {
                return DateTime.Now.Date;
            }
        }

        public static string UppercaseFirstLetter(string str)
        {
            var firstChar = str[0];
            var UpperCaseFirstCharacter = char.ToUpper(firstChar);
            var convertedFirstCharToUpper = UpperCaseFirstCharacter + str.Substring(1);
            return convertedFirstCharToUpper;
        }

        public static DateTime ConvertCustomUTC(this string dateTimeString)
        {
            var dateformats = new string[] {
            @"MM dd yyyy",
            @"MM/dd/yyyy",
            @"yyyy-MM-dd'T'HH:mm:ss",
            @"yyyy-MM-dd'T'HH:mm:ss'Z'",
            @"yyyy-MM-dd'T'HH:mm:ss.fff",
            @"yyyy-MM-dd", @"MM/dd/yyyy",
            @"MM/dd/yyyy HH:mm:ss",
            @"dddd, MMMM dd, yyyy",
            @"MM/dd/yyyy HH:mm tt",
            @"dd MMM yyyy",
            @"dd MMM yyyy HH:mm:ss tt"};
            return ConvertCustomDate(dateTimeString, dateformats);
        }

        public static DateTime ConvertCustomDate(this string dateTimeString, string[] dateFormats)
        {
            try
            {
                var dateTimeOffset = new DateTimeOffset();
                var dateTimeResult = new DateTime();

                var isDateTimeParseable = DateTimeOffset.TryParseExact(dateTimeString, dateFormats, CultureInfo.InvariantCulture,
                    DateTimeStyles.AssumeUniversal, out dateTimeOffset);

                if (isDateTimeParseable)
                {
                    dateTimeResult = dateTimeOffset.UtcDateTime;
                }
                else
                {
                    throw new ArgumentException("DateTime format doesnt match the UTC pattern " + string.Join(" , ", dateFormats));
                }

                return dateTimeResult;
            }
            catch { return new DateTime(); }
        }

        public static string TrimStartEnd(this string str)
        {
            try
            {
                if (!string.IsNullOrEmpty(str))
                {
                    str = str.TrimStart().TrimEnd();
                    return str;
                }
                else
                    return str;
            }
            catch { return str; }
        }

        public static string VectorValue(this object expression)
        {
            string str = string.Empty;
            try
            {                              
                if (expression != null)
                {
                    System.Type type = expression.GetType();

                    if (expression.GetType() == typeof(int) || expression.GetType() == typeof(long))
                    {
                        var value = expression.ConvertToLong();
                        if (value >= 0)
                        {
                            str = string.Format("{0:N0}", value);
                        }
                        else
                        {
                            str = "(" + string.Format("{0:N0}", value) + ")";
                            str = str.Replace("-", "");
                        }
                    }                    
                    else if (expression.GetType() == typeof(float) || expression.GetType() == typeof(double))
                    {
                        var value = expression.ConvertToDouble();
                        if (value >= 0)
                        {
                            str = string.Format("{0:N0}", value);
                        }
                        else
                        {
                            str = "(" + string.Format("{0:N0}", value) + ")";
                            str = str.Replace("-", "");
                        }

                    }
                    else if (expression.GetType() == typeof(decimal))
                    {
                        var value = expression.ConvertToDecimal();
                        if (value >= 0)
                        {
                            str = string.Format("{0:N0}", value);
                        }
                        else
                        {
                            str = "(" + string.Format("{0:N0}", value) + ")";
                            str = str.Replace("-", "");
                        }
                    }
                }
                return str;           
            }
            catch { return str; }
        }
    }
}