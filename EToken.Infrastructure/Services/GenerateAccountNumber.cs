using System.Security.Cryptography;

namespace EToken.Infrastructure.Services;

public interface IAccountNumberGenerator
{
    string Generate(string bankCode = "000");
    bool Validate(string accountNumber, string bankCode = "000");
}

public class AccountNumberGenerator : IAccountNumberGenerator
{
    // Official NUBAN algorithm weights for (3-digit Bank Code + 9-digit Serial)
    private static readonly int[] Weights = [3, 7, 3, 3, 7, 3, 3, 7, 3, 3, 7, 3];

    /// <summary>
    /// Generates a valid 10-digit account number that starts with '0' and includes a standard check digit.
    /// </summary>
    /// <param name="bankCode">3-digit CBN institution code (defaults to "000" if generic).</param>
    public string Generate(string bankCode = "000")
    {
        if (string.IsNullOrWhiteSpace(bankCode) || bankCode.Length != 3)
        {
            throw new ArgumentException("Bank code must be exactly 3 digits.", nameof(bankCode));
        }

        // 1. Generate 8 random digits to follow the required leading '0' (making a 9-digit serial)
        byte[] randomBytes = RandomNumberGenerator.GetBytes(8);
        var serialDigits = new char[9];
        serialDigits[0] = '0'; // Enforce starting with '0'

        for (int i = 0; i < 8; i++)
        {
            serialDigits[i + 1] = (char)('0' + (randomBytes[i] % 10));
        }

        string serialNumber = new string(serialDigits); // e.g. "012345678"

        // 2. Compute the Modulo 10 check digit
        int checkDigit = CalculateCheckDigit(bankCode, serialNumber);

        // 3. 10-digit account number = 9-digit serial + 1-digit check digit
        return $"{serialNumber}{checkDigit}";
    }

    /// <summary>
    /// Validates whether a 10-digit account number has a valid check digit and starts with '0'.
    /// </summary>
    public bool Validate(string accountNumber, string bankCode = "000")
    {
        if (string.IsNullOrWhiteSpace(accountNumber) || accountNumber.Length != 10 || !accountNumber.StartsWith('0'))
        {
            return false;
        }

        if (!long.TryParse(accountNumber, out _))
        {
            return false;
        }

        string serialNumber = accountNumber[..9];
        int expectedCheckDigit = accountNumber[9] - '0';
        int calculatedCheckDigit = CalculateCheckDigit(bankCode, serialNumber);

        return expectedCheckDigit == calculatedCheckDigit;
    }

    private static int CalculateCheckDigit(string bankCode, string serialNumber)
    {
        string combined = bankCode + serialNumber; // 12 digits total

        int sum = 0;
        for (int i = 0; i < 12; i++)
        {
            int digit = combined[i] - '0';
            sum += digit * Weights[i];
        }

        int remainder = sum % 10;
        int checkDigit = (10 - remainder) % 10;

        return checkDigit;
    }
}