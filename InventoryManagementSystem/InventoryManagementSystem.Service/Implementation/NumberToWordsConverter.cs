using InventoryManagementSystem.Service.Interface;

namespace InventoryManagementSystem.Service.Implementation
{
    /// <summary>
    /// Number to Words Converter Implementation
    /// Converts numbers to words for invoice amounts (SRP compliance)
    /// </summary>
    public class NumberToWordsConverter : INumberToWordsConverter
    {
        private static readonly string[] Units = { "", "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine" };
        private static readonly string[] Teens = { "Ten", "Eleven", "Twelve", "Thirteen", "Fourteen", "Fifteen", "Sixteen", "Seventeen", "Eighteen", "Nineteen" };
        private static readonly string[] Tens = { "", "", "Twenty", "Thirty", "Forty", "Fifty", "Sixty", "Seventy", "Eighty", "Ninety" };

        public string Convert(decimal number, string currency = "Rupees")
        {
            var rupees = (long)number;
            var paise = (int)((number - rupees) * 100);

            if (rupees == 0)
                return $"Zero {currency}";

            var words = "";

            // Crores
            var crores = rupees / 10000000;
            rupees %= 10000000;
            if (crores > 0)
            {
                words += ConvertUnderCrore(crores) + " Crore ";
            }

            // Lakhs
            var lakhs = rupees / 100000;
            rupees %= 100000;
            if (lakhs > 0)
            {
                words += ConvertUnderCrore(lakhs) + " Lakh ";
            }

            // Thousands
            var thousands = rupees / 1000;
            rupees %= 1000;
            if (thousands > 0)
            {
                words += ConvertUnderCrore(thousands) + " Thousand ";
            }

            // Hundreds
            var hundreds = rupees / 100;
            rupees %= 100;
            if (hundreds > 0)
            {
                words += Units[hundreds] + " Hundred ";
            }

            // Tens and units
            if (rupees >= 20)
            {
                words += Tens[rupees / 10] + " " + Units[rupees % 10];
            }
            else if (rupees >= 10)
            {
                words += Teens[rupees - 10];
            }
            else if (rupees > 0)
            {
                words += Units[rupees];
            }

            words += currency;

            // Paise
            if (paise > 0)
            {
                if (paise >= 20)
                {
                    words += " and " + Tens[paise / 10] + " " + Units[paise % 10] + " Paise";
                }
                else if (paise >= 10)
                {
                    words += " and " + Teens[paise - 10] + " Paise";
                }
                else
                {
                    words += " and " + Units[paise] + " Paise";
                }
            }

            return words.Trim();
        }

        private static string ConvertUnderCrore(long number)
        {
            var words = "";

            var thousands = number / 1000;
            number %= 1000;
            if (thousands > 0)
            {
                words += Units[thousands] + " Thousand ";
            }

            var hundreds = number / 100;
            number %= 100;
            if (hundreds > 0)
            {
                words += Units[hundreds] + " Hundred ";
            }

            if (number >= 20)
            {
                words += Tens[number / 10] + " " + Units[number % 10];
            }
            else if (number >= 10)
            {
                words += Teens[number - 10];
            }
            else if (number > 0)
            {
                words += Units[number];
            }

            return words.Trim();
        }
    }
}