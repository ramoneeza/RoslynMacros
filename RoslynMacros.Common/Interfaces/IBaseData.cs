namespace RoslynMacros.Common.Interfaces
{
    public interface IBaseData
    {
        string MODIFIER { get; }
        string RETURNTYPE { get; }
        string NAME { get; }
        string WIDENAME { get; }
        string[] GENERIC { get; }
        IAttrData[] AttributesData { get; }
        string[] AttibutesLists { get; }
    }
}