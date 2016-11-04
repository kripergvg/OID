namespace OID.Core.HashGenerator
{
    public interface IHashGenerator
    {
        string Generate(string input, bool hexString);
    }
}
