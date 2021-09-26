namespace com.etsoo.Utils.Database
{
    /// <summary>
    /// Command builder delegate
    /// </summary>
    /// <param name="identifier">Identifier</param>
    /// <param name="parts">Parts</param>
    /// <returns>Command</returns>
    public delegate string CommandBuilderDelegate(string identifier, IEnumerable<string> parts);
}
