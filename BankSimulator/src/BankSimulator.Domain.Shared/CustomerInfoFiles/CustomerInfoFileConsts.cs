namespace BankSimulator.CustomerInfoFiles
{
    public static class CustomerInfoFileConsts
    {
        private const string DefaultSorting = "{0}CIFNumber asc";

        public static string GetDefaultSorting(bool withEntityName)
        {
            return string.Format(DefaultSorting, withEntityName ? "CustomerInfoFile." : string.Empty);
        }

    }
}