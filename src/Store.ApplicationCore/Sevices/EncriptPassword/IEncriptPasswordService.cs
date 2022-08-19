using Store.ApplicationCore.Sevices.EncriptPassword.Cls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.ApplicationCore.Sevices.EncriptPassword
{
    public interface IEncriptPasswordService
    {
        PasswordHashResult CrearPasswordHash(string Pasword);
        bool VerificarPasswordHash(PasswordToVerificar passwordToVerificar);
    }
}
