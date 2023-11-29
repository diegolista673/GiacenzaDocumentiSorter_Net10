using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GiacenzaSorterRm.AppCode
{
    public static class MyConnections
    {

        ////test 
        //public static string GiacenzaSorterRmContext { get; } = "Server=LAPTOP-JUJ7V1JK\\SQLEXPRESS;Database=GIACENZA_SORTER_RM;Trusted_Connection=True;";

        public static string CnxnMondo { get; } = "Server=VEVRFL1M031H\\SQLEXPRESS;Database=MONDO_TEST;TrustServerCertificate=true;Trusted_Connection=True;";

        public static string GiacenzaSorterRmContext { get; } = "Server=VEVRFL1M031H\\SQLEXPRESS;Database=GIACENZA_SORTER_RM_TEST;TrustServerCertificate=true;Trusted_Connection=True;";


        /////produzione
        //public static string CnxnMondo { get; } = "Server=172.30.122.206;Database = RHM_POSTEL; User Id=read_user_db;Password=read_user_db;";
        //public static string GiacenzaSorterRmContext { get; } = "Server=SRVR-000EDP02.postel.it; Database=GIACENZA_SORTER_RM; User Id=UserProduzioneGed;Password=UserProduzioneGed2022!;TrustServerCertificate=true;Trusted_Connection=false;";
    }
}
