using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace GiacenzaSorterRm.AppCode
{
    public static class CentroAppartenenza
    {
        /// <summary>
        /// Return idCentro by role ADMIN and centro selected by select list
        /// </summary>
        /// <param name="user"></param>
        /// <param name="selectedCentro"></param>
        /// <returns></returns>
        public static int SetCentroByRoleADMIN(ClaimsPrincipal user, int selectedCentro)
        {
            int CentroID = 0;

            if (user.IsInRole("ADMIN"))
            {
               CentroID = selectedCentro;
            }
            else
            {
                CentroID = Convert.ToInt32(user.FindFirstValue("idCentro"));
            }

            return CentroID;
        }


        /// <summary>
        /// return idCentro by role ADMIN or Supervisor and centro selected by select list
        /// </summary>
        /// <param name="user"></param>
        /// <param name="selectedCentro"></param>
        /// <returns></returns>
        public static int SetCentroByRoleADMINSupervisor(ClaimsPrincipal user, int selectedCentro)
        {
            int CentroID = 0;

            if (user.IsInRole("ADMIN") || user.IsInRole("SUPERVISOR"))
            {
                CentroID = selectedCentro;
            }
            else
            {
                CentroID = Convert.ToInt32(user.FindFirstValue("idCentro"));
            }

            return CentroID;
        }



        /// <summary>
        /// Return idCentro by user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static int SetCentroByUser(ClaimsPrincipal user)
        {
            int CentroID = Convert.ToInt32(user.FindFirstValue("idCentro"));
            return CentroID;
        }


        /// <summary>
        /// Get Centro bu user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static string GetCentroLavorazioneByUser(ClaimsPrincipal user)
        {
            string centro = user.FindFirst("Centro").Value; 
            return centro;
        }
    }
}
