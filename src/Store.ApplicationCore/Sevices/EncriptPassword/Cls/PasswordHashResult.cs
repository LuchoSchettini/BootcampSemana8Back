using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.ApplicationCore.Sevices.EncriptPassword.Cls
{
    public class PasswordHashResult
    {
        //public bool ResultadoEncriptacion { get; set; }
        //public string MensajeError { get; set; } 
        public string PasswordHash { get; set; }
        public string PasswordSalt { get; set; }
    }
}
