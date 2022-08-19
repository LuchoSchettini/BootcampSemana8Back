﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Store.ApplicationCore.Helpers.Utils
{
    public class DateUtil
    {
        public static DateTime GetCurrentDate()
        {
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.Local);
        }
    }
}
