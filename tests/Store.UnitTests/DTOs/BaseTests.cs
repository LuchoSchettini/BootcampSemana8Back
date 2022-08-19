using System.ComponentModel.DataAnnotations;

namespace Store.UnitTests.DTOs
{
    public abstract class BaseTests
    {
        public IList<ValidationResult> ValidateModel(object model)
        {
            var validationResults = new List<ValidationResult>();
            var ctx = new ValidationContext(model, null, null);
            Validator.TryValidateObject(model, ctx, validationResults, true);

            //Devuelve una lista con los errores en base a las anotacones de las propiedades.
            //Si no hay errores devuelve una lista vacia.
            return validationResults;
        }
    }
}
