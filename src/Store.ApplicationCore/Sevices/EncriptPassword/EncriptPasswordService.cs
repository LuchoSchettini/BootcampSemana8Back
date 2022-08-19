using Store.ApplicationCore.Sevices.EncriptPassword.Cls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Store.ApplicationCore.Sevices.EncriptPassword;

public class EncriptPasswordService : IEncriptPasswordService
{

    public PasswordHashResult CrearPasswordHash(string Pasword)
    {
        using (var hmac = new HMACSHA512())
        {
            //passwordSalt = hmac.Key;
            //passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));



            PasswordHashResult passwordYaEncriptado = new PasswordHashResult();
            passwordYaEncriptado.PasswordSalt = Convert.ToBase64String(hmac.Key);
            passwordYaEncriptado.PasswordHash = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(Pasword)));
            return passwordYaEncriptado;


            //try
            //{
            //    passwordYaEncriptado.PasswordSalt = Convert.ToBase64String(hmac.Key);
            //    passwordYaEncriptado.PasswordHash = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(Pasword)));
            //    passwordYaEncriptado.ResultadoEncriptacion = true;
            //    //passwordYaEncriptado.MensajeError = "";
            //}
            //catch (Exception ex)
            //{
            //    passwordYaEncriptado.ResultadoEncriptacion = false;
            //    passwordYaEncriptado.MensajeError = ex.Message;
            //}
            //return passwordYaEncriptado;
        }
    }


    public bool VerificarPasswordHash(PasswordToVerificar passwordToVerificar)
    {


        byte[] PasswordHash = Convert.FromBase64String(passwordToVerificar.PasswordHash);
        byte[] PasswordSalt = Convert.FromBase64String(passwordToVerificar.PasswordSalt);

        using (var hmac = new HMACSHA512(PasswordSalt))
        {
            //este lanaza el error 500
            //throw new System.Exception();
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(passwordToVerificar.Password));
            for (int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != PasswordHash[i])
                {
                    return false;
                }
            }
            return true;
        }




            //PasswordVerificarResult passwordVerificarResult = new PasswordVerificarResult();
            //try
            //{
            //    var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(passwordToVerificar.Password));
            //    for (int i = 0; i < computedHash.Length; i++)
            //    {

            //        if (computedHash[i] != PasswordHash[i])
            //        {
            //            passwordVerificarResult.ResultadoVerificacion = false;
            //            passwordVerificarResult.HayError = false;
            //            passwordVerificarResult.MensajeError = "";
            //            return passwordVerificarResult;
            //        }
            //    }
            //    passwordVerificarResult.ResultadoVerificacion = true;
            //    passwordVerificarResult.HayError = false;
            //    passwordVerificarResult.MensajeError = "";
            //    return passwordVerificarResult;
            //}
            //catch (Exception ex)
            //{
            //    //return false;
            //    passwordVerificarResult.ResultadoVerificacion = false;
            //    passwordVerificarResult.HayError = true;
            //    passwordVerificarResult.MensajeError = ex.Message;
            //    return passwordVerificarResult;

            //}

        }


}

//devuelve un string  -> return Convert.ToBase64String(ArregloByte)   // pasar a hash a string

//devuelve un string  -> byte[] miHash = Convert.FromBase64String(StringBase64miHash)   // pasar a string el Hash
