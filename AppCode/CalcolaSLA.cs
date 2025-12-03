using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GiacenzaSorterRm 
{
    public static class CalcolaSla
    {

        public static DateTime CalcolaDataJ(DateTime data, int giorni)
        {
            List<string> giorniFestivi = new List<string>();
            giorniFestivi.Add(("01/01" + String.Format("{0:/yyyy}", data)));
            giorniFestivi.Add(("06/01" + String.Format("{0:/yyyy}", data)));
            giorniFestivi.Add(("25/04" + String.Format("{0:/yyyy}", data)));
            giorniFestivi.Add(("01/05" + String.Format("{0:/yyyy}", data)));
            giorniFestivi.Add(("02/06" + String.Format("{0:/yyyy}", data)));
            giorniFestivi.Add(("15/08" + String.Format("{0:/yyyy}", data)));
            giorniFestivi.Add(("01/11" + String.Format("{0:/yyyy}", data)));
            giorniFestivi.Add(("08/12" + String.Format("{0:/yyyy}", data)));
            giorniFestivi.Add(("25/12" + String.Format("{0:/yyyy}", data)));
            giorniFestivi.Add(("26/12" + String.Format("{0:/yyyy}", data)));
            giorniFestivi.Add("20/04/2014");
            giorniFestivi.Add("21/04/2014");
            giorniFestivi.Add("05/04/2015");
            giorniFestivi.Add("06/04/2015");
            giorniFestivi.Add("27/03/2016");
            giorniFestivi.Add("28/03/2016");
            giorniFestivi.Add("16/04/2017");
            giorniFestivi.Add("17/04/2017");
            giorniFestivi.Add("01/04/2018");
            giorniFestivi.Add("02/04/2018");
            giorniFestivi.Add("21/04/2019");
            giorniFestivi.Add("22/04/2019");
            giorniFestivi.Add("12/04/2020");
            giorniFestivi.Add("13/04/2020");
            giorniFestivi.Add("04/04/2021");
            giorniFestivi.Add("05/04/2021");
            giorniFestivi.Add("17/04/2022");
            giorniFestivi.Add("18/04/2022");
            giorniFestivi.Add("09/04/2023");
            giorniFestivi.Add("10/04/2023");
            giorniFestivi.Add("31/03/2024");
            giorniFestivi.Add("01/04/2024");
            giorniFestivi.Add("20/04/2025");
            giorniFestivi.Add("21/04/2025");
            giorniFestivi.Add("05/04/2026");
            giorniFestivi.Add("06/04/2026");
            giorniFestivi.Add("28/03/2027");
            giorniFestivi.Add("29/03/2027");
            // primo giorno dell'anno successivo
            giorniFestivi.Add(("01/01" + String.Format("{0:/yyyy}", data.AddYears(1))));

            try
            {

                for (int i = 1; i <= giorni; i++)
                {
                    data = data.AddDays(1);
                    if (data.DayOfWeek == DayOfWeek.Saturday)
                    {
                        // sabato
                        i = i - 1;

                    }
                    else if (data.DayOfWeek == DayOfWeek.Sunday)
                    {
                        // domenica
                        i = i - 1;
                    }

                    string d = String.Format("{0:dd/MM/yyyy}", data);
                    if (giorniFestivi.Contains(d))
                    {
                        i = i - 1;
                    }

                }

                return data;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
                //return "#N/D";
            }


        }


    }
}
