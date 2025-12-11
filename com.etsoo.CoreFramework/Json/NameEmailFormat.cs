using com.etsoo.Utils.String;
using Json.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace com.etsoo.CoreFramework.Json
{
    /// <summary>
    /// Name email format, supports "a@b.com" and "Name <a@b.com>"
    /// </summary>
    public class NameEmailFormat : Format
    {
        public NameEmailFormat() : base("name-email")
        {
        }

        public override bool Validate(JsonElement value, out string? errorMessage)
        {
            var input = value.GetString();

            if (string.IsNullOrEmpty(input))
            {
                errorMessage = "The value is null or empty so not a valid name-email format";
                return false;
            }

            var isValid = new EmailAddressAttribute().IsValid(input);

            errorMessage = isValid ? null : $"The value '{StringUtils.HideEmail(input)}' is not a valid name-email format";

            return isValid;
        }
    }
}
