/* Author: David Carroll
 * Copyright (c) 2008, 2009 Bellevue Baptist Church 
 * Licensed under the GNU General Public License (GPL v2)
 * you may not use this code except in compliance with the License.
 * You may obtain a copy of the License at http://bvcms.codeplex.com/license 
 */
using System;
using System.Dynamic;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.ComponentModel;
using System.Linq;
using System.Configuration;
using System.Net.Mail;
using System.IO;
using System.Security.Cryptography;
using System.Globalization;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace UtilityExtensions
{
    public static partial class Util
    {
        public static bool IsNull(this object o)
        {
            return o == null;
        }
        public static bool IsNotNull(this object o)
        {
            return o != null;
        }
        public static int? IntOrNull(this string s)
        {
            int? i = null;
            if (s.HasValue())
                i = int.Parse(s);
            return i;
        }
        public static bool Has(this object obj, string propertyName)
        {
            var dynamic = obj as DynamicObject;
            if (dynamic == null) return false;
            return dynamic.GetDynamicMemberNames().Contains(propertyName);
        }
        public static bool HasValue(this string s)
        {
            return !string.IsNullOrWhiteSpace(s);
        }
        public static bool Equal(this string s, string s2)
        {
            return string.Compare(s, s2, StringComparison.OrdinalIgnoreCase) == 0;
        }

        public static bool NotEqual(this string s, string s2)
        {
            return !s.Equal(s2);
        }
        public static bool AllDigits(this string str)
        {
            if (!str.HasValue())
                return false;
            var patt = new Regex("[^0-9]");
            return !(patt.IsMatch(str));
        }
        private const string STR_Culture = "Culture";
        public static string Culture
        {
            get
            {
                if (HttpContext.Current != null)
                    if (HttpContext.Current.Items[STR_Culture] != null)
                        return (string)HttpContext.Current.Items[STR_Culture];
                return null;
            }
            set
            {
                HttpContext.Current.Items[STR_Culture] = value;
            }
        }

        public static string CultureDateFormat
        {
            get
            {
                var cultureDateFormat = "MM/DD/YYYY";
                if (Culture != "en-US")
                {
                    var c = new CultureInfo(Culture);
                    cultureDateFormat = c.DateTimeFormat.ShortDatePattern.ToUpper();
                }
                return cultureDateFormat;
            }
        }
        // h:mm A
        public static string CultureDateFormatAlt
        {
            get
            {
                var cultureDateFormat = "MM/DD/YY";
                if (Culture != "en-US")
                {
                    cultureDateFormat = string.Empty;
                }
                return cultureDateFormat;
            }
        }

        public static string CultureDateTimeFormat
        {
            get
            {
                return "{0} {1}".Fmt(CultureDateFormat, "h:mm A");
            }
        }

        public static string CultureDateTimeFormatAlt
        {
            get
            {
                var cultureDateTimeFormat = "MM/DD/YY";
                if (Culture != "en-US")
                {
                    cultureDateTimeFormat = string.Empty;
                }
                if (!string.IsNullOrEmpty(cultureDateTimeFormat))
                    cultureDateTimeFormat = "{0} {1}".Fmt(cultureDateTimeFormat, "h:mm A");

                return cultureDateTimeFormat;
            }
        }

        public static string jQueryDateFormat
        {
            get
            {
                var dt = new DateTime(2002, 1, 30);
                var s = dt.ToShortDateString();
                if (s.StartsWith("30"))
                    return "d/m/yy";
                return "m/d/yy";
            }
        }
        public static string jQueryDateValidation
        {
            get
            {
                var dt = new DateTime(2002, 1, 30);
                var s = dt.ToShortDateString();
                if (s.StartsWith("30"))
                    return @"^\d\d?[-/](0?[1-9]|1[012])[-/]\d\d(\d\d)?$";
                if (s.StartsWith("2002"))
                    return @"^\d\d\d\d[-/]\d\d[-/](0[1-9]|1[012])$";
                return @"^(0?[1-9]|1[012])[-/]\d\d?[-/]\d\d(\d\d)?$";
            }
        }
        public static string jQueryDateFormat2
        {
            get
            {
                var dt = new DateTime(2002, 1, 30);
                var s = dt.ToShortDateString();
                var sep = s.Contains("-") ? "-" : "/";
                if (s.StartsWith("30"))
                    return "d{0}m{0}yyyy".Fmt(sep);
                if (s.StartsWith("2002"))
                    return "yyyy{0}mm{0}dd".Fmt(sep);
                return "m{0}d{0}yyyy".Fmt(sep);
            }
        }
        public static string jQueryDateFormat2WithTime
        {
            get { return jQueryDateFormat2 + " H:ii P"; }
        }
        public static string jQueryNumberValidation
        {
            get
            {
                const decimal c = 1.23M;
                if (c.ToString("c").StartsWith("1,23"))
                    return @"^-?(?:\d+)?(?:,\d+)?$";
                return @"^-?(?:\d+)?(?:\.\d+)?$";
            }
        }
        public static bool IsInRole(string role)
        {
            if (HttpContext.Current != null)
                return HttpContext.Current.User.IsInRole(role);
            return false;
        }
    }
}

