namespace Store.ApplicationCore.Helpers.Auth;
public class Auth_Roles
{
    public enum Roles
    {
        Administrador,
        Gerente,
        Empleado
    }

    public const Roles rol_predeterminado = Roles.Empleado;
}
