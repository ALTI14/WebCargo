namespace WebCargo.Services
{
    public class TcValidator
    {

        public static string ValidateTCNumber(
    string tcNumber)
        {
            if (string.IsNullOrWhiteSpace(
        tcNumber))
            {
                return "TC Kimlik No zorunludur.";
            }
            if (tcNumber.Length != 11)
                return "TC Kimlik No 11 haneli olmalıdır.";

            if (!tcNumber.All(char.IsDigit))
                return "TC Kimlik No sadece rakamlardan oluşmalıdır.";

            if (tcNumber[0] == '0')
                return "TC Kimlik No 0 ile başlayamaz.";

            int[] d = new int[11];

            for (int i = 0; i < 11; i++)
            {
                d[i] = tcNumber[i] - '0';
            }

            int oddSum =
                d[0] + d[2] + d[4] +
                d[6] + d[8];

            int evenSum =
                d[1] + d[3] +
                d[5] + d[7];

            if (((oddSum * 7) - evenSum) % 10
                != d[9])
            {
                return "TC Kimlik No geçersiz.";
            }

            int firstTenSum = 0;

            for (int i = 0; i < 10; i++)
            {
                firstTenSum += d[i];
            }

            if (firstTenSum % 10 != d[10])
            {
                return "TC Kimlik No geçersiz.";
            }

            return null!;
        }

    }
}
