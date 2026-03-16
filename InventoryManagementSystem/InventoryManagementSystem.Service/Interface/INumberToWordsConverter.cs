namespace InventoryManagementSystem.Service.Interface
{
    /// <summary>
    /// Number to Words Converter Interface
    /// Converts numbers to words for invoice amounts (SRP compliance)
    /// </summary>
    public interface INumberToWordsConverter
    {
        /// <summary>
        /// Convert a number to words
        /// </summary>
        /// <param name="number">The number to convert</param>
        /// <param name="currency">The currency name (default: Rupees)</param>
        /// <returns>The number in words</returns>
        string Convert(decimal number, string currency = "Rupees");
    }
}