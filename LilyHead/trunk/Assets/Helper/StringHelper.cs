using System;
using System.Security.Cryptography;
using System.Text;

namespace AssemblyCSharp.Helper
{
	public static class StringHelper
	{
		public static string getMD5(this string originalString) {
			
			System.Security.Cryptography.MD5CryptoServiceProvider Mymd5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
		    byte[] MyinputBytes = System.Text.Encoding.ASCII.GetBytes (originalString);
		    byte[] MyhashBytes  = Mymd5.ComputeHash (MyinputBytes);
		    StringBuilder Mysb = new StringBuilder();
		    for (int i = 0; i < MyhashBytes.Length; i++)
		    {
		        Mysb.Append (MyhashBytes[i].ToString ("X2"));
		    }
		    return Mysb.ToString();
		}
		
		public static int Asc(char character)
	    {
		    System.Text.ASCIIEncoding asciiEncoding = new System.Text.ASCIIEncoding();
		    int intAsciiCode = (int)asciiEncoding.GetBytes(character.ToString())[0];
		    return intAsciiCode;
	    }
		
		public static string Chr(int asciiCode)
	    {
		    if (asciiCode >= 0 && asciiCode <= 255)
		    {
		        System.Text.ASCIIEncoding asciiEncoding = new System.Text.ASCIIEncoding();
		        byte[] byteArray = new byte[] { (byte)asciiCode };
		        string strCharacter = asciiEncoding.GetString(byteArray);
		        return (strCharacter);
		    }
		    else
		    {
		        throw new Exception("ASCII Code is not valid.");
		    }
		}
	}
}

