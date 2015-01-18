
namespace BravoVets.DomainObject.Enum
{
    /// <summary>
    /// The filter values correspond the IDs in the BravoVetsStatus table
    /// </summary>
    public enum ContentFilterEnum
    {
        All = 0,
        Hidden = 1,
        FacebookShare = 3,
        TwitterShare = 4,
        Favorites = 7,
        GenericShare = 100
    }
}
