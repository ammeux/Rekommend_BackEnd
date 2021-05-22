namespace Rekommend.IDP.Entities
{
    interface IConcurrencyAware
    {
        string ConcurrencyStamp { get; set; }
    }
}
