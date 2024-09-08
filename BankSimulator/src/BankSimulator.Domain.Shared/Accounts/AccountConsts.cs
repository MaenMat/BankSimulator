namespace BankSimulator.Accounts
{
    public static class AccountConsts
    {
        private const string DefaultSorting = "{0}AccountNumber asc";

        public static string GetDefaultSorting(bool withEntityName)
        {
            return string.Format(DefaultSorting, withEntityName ? "Account." : string.Empty);
        }

    }
}