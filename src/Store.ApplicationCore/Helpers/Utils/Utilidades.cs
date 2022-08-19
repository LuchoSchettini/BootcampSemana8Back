using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.ApplicationCore.Helpers.Utils
{
    public static class Utilidades
    {


        public static int TotalEncontradosEnListaString(IEnumerable<string> Lista, string CadenaBuscar)
        {
            int Encontrados = 0;
            foreach (var cadena in Lista)
            {
                if (cadena.Contains(CadenaBuscar))
                {
                    Encontrados += 1;
                }
            }
            return Encontrados;
        }



    }
}
