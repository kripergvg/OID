namespace OID.SoapDataProvider.HashGenerator
{
    public interface IHashGenerator
    {
        string Generate(string input, bool hexString);
    }
}
