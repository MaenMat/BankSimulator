namespace BankSimulator.Otps
{
    public static class OtpConsts
    {
        private const string DefaultSorting = "{0}TransactionNumber asc";

        public static string GetDefaultSorting(bool withEntityName)
        {
            return string.Format(DefaultSorting, withEntityName ? "Otp." : string.Empty);
        }

    }
}