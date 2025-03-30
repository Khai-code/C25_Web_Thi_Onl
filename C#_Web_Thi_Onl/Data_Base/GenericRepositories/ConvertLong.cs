using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_Base.GenericRepositories
{
    public class ConvertLong
    {
        public static DateTime ConvertLongToDateTime(long dateTimeLong)
        {
            string dateTimeStr = dateTimeLong.ToString();

            if (dateTimeStr.Length != 14)
                throw new ArgumentException("Chuỗi ngày giờ không hợp lệ!");

            int year = int.Parse(dateTimeStr.Substring(0, 4));
            int month = int.Parse(dateTimeStr.Substring(4, 2));
            int day = int.Parse(dateTimeStr.Substring(6, 2));
            int hour = int.Parse(dateTimeStr.Substring(8, 2));
            int minute = int.Parse(dateTimeStr.Substring(10, 2));
            int second = int.Parse(dateTimeStr.Substring(12, 2));

            if (hour == 0 && minute == 0 && second == 0)
            {
                second = 1;
            }

            return new DateTime(year, month, day, hour, minute, second);
        }

        public static DateTime ConvertLongToDateOnly(long dateTimeLong)
        {
            string dateTimeStr = dateTimeLong.ToString();

            if (dateTimeStr.Length != 14)
                throw new ArgumentException("Chuỗi ngày giờ không hợp lệ!");

            int year = int.Parse(dateTimeStr.Substring(0, 4));
            int month = int.Parse(dateTimeStr.Substring(4, 2));
            int day = int.Parse(dateTimeStr.Substring(6, 2));

            return new DateTime(year, month, day);
        }

        public static long ConvertDateTimeToLong(DateTime dateTime)
        {
            // Kiểm tra xem giờ, phút, giây có khác 0 không
            bool hasTime = dateTime.Hour != 0 || dateTime.Minute != 0 || dateTime.Second != 0;

            // Chuyển đổi sang định dạng yyyyMMddHHmmss
            string formattedLong = dateTime.ToString("yyyyMMdd") + (hasTime ? dateTime.ToString("HHmmss") : "000000");

            return long.Parse(formattedLong);
        }
    }
}
