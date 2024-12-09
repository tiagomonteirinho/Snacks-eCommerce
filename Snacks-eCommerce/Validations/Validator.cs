using System.Text.RegularExpressions;

namespace Snacks_eCommerce.Validations;

public class Validator : IValidator
{
    public string NameError { get; set; } = "";
    public string EmailError { get; set; } = "";
    public string PhoneNumberError { get; set; } = "";
    public string PasswordError { get; set; } = "";

    private const string EmptyNameErrorMessage = "Name is required.";
    private const string InvalidNameErrorMessage = "Name is invalid.";
    private const string EmptyEmailErrorMessage = "Email is required.";
    private const string InvalidEmailErrorMessage = "Email is invalid.";
    private const string EmptyPhoneNumberErrorMessage = "Phone number is required.";
    private const string InvalidPhoneNumberErrorMessage = "Phone number is invalid.";
    private const string EmptyPasswordErrorMessage = "Password is required.";
    private const string InvalidPasswordErrorMessage = "Password must contain at least 8 characters and include letters and digits.";

    public Task<bool> Validate(string name, string email, string phoneNumber, string password)
    {
        var isNameValid = ValidateName(name);
        var isEmailValid = ValidateEmail(email);
        var isPhoneNumberValid = ValidatePhoneNumber(phoneNumber);
        var isPasswordValid = ValidatePassword(password);
        return Task.FromResult(isNameValid && isEmailValid && isPhoneNumberValid && isPasswordValid);
    }

    private bool ValidateName(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            NameError = EmptyNameErrorMessage;
            return false;
        }

        if (name.Length < 3)
        {
            NameError = InvalidNameErrorMessage;
            return false;
        }

        NameError = "";
        return true;
    }

    private bool ValidateEmail(string email)
    {
        if (string.IsNullOrEmpty(email))
        {
            EmailError = EmptyEmailErrorMessage;
            return false;
        }

        if (!Regex.IsMatch(email, @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$"))
        {
            EmailError = InvalidEmailErrorMessage;
            return false;
        }

        EmailError = "";
        return true;
    }

    private bool ValidatePhoneNumber(string phoneNumber)
    {
        if (string.IsNullOrEmpty(phoneNumber))
        {
            PhoneNumberError = EmptyPhoneNumberErrorMessage;
            return false;
        }

        if (phoneNumber.Length < 9)
        {
            PhoneNumberError = InvalidPhoneNumberErrorMessage;
            return false;
        }

        PhoneNumberError = "";
        return true;
    }

    private bool ValidatePassword(string password)
    {
        if (string.IsNullOrEmpty(password))
        {
            PasswordError = EmptyPasswordErrorMessage;
            return false;
        }

        if (password.Length < 8 || !Regex.IsMatch(password, @"[a-zA-Z]") || !Regex.IsMatch(password, @"\d"))
        {
            PasswordError = InvalidPasswordErrorMessage;
            return false;
        }

        PasswordError = "";
        return true;
    }
}
