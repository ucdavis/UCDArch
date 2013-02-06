namespace UCDArch.Web.ActionResults
{
    /// <summary>
    /// Possible serialization strategies for dates in JSON
    /// </summary>
    /// <remarks>
    /// Default: MS Dates, like \/Date(1234656000000)\/
    /// JavaScript: new Date(1234656000000)
    /// Iso: "2009-02-15T00:00:00Z"
    /// </remarks>
    public enum JsonDateConversionStrategy
    {
        Default,
        JavaScript,
        Microsoft,
        Iso
    }
}