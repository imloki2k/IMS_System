using System.Security.Cryptography;
using System.Text;

namespace IMS_System.Extentions
{
    public static class EncodingSHA256
    {
        public static string GetSHA256(this string str)
        {
			using (SHA256 sha256 = SHA256.Create())
			{
				byte[] bytes = Encoding.UTF8.GetBytes(str);
				byte[] hashBytes = sha256.ComputeHash(bytes);

				StringBuilder builder = new StringBuilder();
				for (int i = 0; i < hashBytes.Length; i++)
				{
					builder.Append(hashBytes[i].ToString("x2"));
				}

				return builder.ToString();
			}
		}
    }
}
