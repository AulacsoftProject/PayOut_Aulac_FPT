using PayOut_Aulac_FPT.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayOut_Aulac_FPT.Core.Utils.Models
{
    public class EmailConfigInfo
    {
        public int Port { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? Security { get; set; }
        public string? Host { get; set; }

        public static string EMAIL_PASSWORD = "EMAIL_PASSWORD";
        public static string EMAIL_USERNAME = "EMAIL_USERNAME";
        public static string EMAIL_SECURITY = "EMAIL_SECURITY";
        public static string EMAIL_HOST = "EMAIL_HOST";
        public static string EMAIL_PORT = "EMAIL_PORT";
        public EmailConfigInfo() { }

        //public EmailConfigInfo(CauHinh[] cauHinh)
        //{
        //    try
        //    {
        //        foreach (CauHinh _cauHinh in cauHinh)
        //        {
        //            if (_cauHinh?.Ma == EMAIL_PASSWORD)
        //            {
        //                Password = _cauHinh.GiaTri;
        //            }
        //            else if (_cauHinh?.Ma == EMAIL_SECURITY)
        //            {
        //                Security = _cauHinh.GiaTri;
        //            }
        //            else if (_cauHinh?.Ma == EMAIL_USERNAME)
        //            {
        //                Username = _cauHinh.GiaTri;
        //            }
        //            else if (_cauHinh?.Ma == EMAIL_HOST)
        //            {
        //                Host = _cauHinh.GiaTri;
        //            }
        //            else if (_cauHinh?.Ma == EMAIL_PORT)
        //            {
        //                Port = int.Parse(_cauHinh.GiaTri ?? "0");
        //            }
        //        }
        //    }
        //    catch (Exception)
        //    {

        //    }
        //}
    }
}
