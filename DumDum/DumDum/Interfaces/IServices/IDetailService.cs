using DumDum.Models.JsonEntities.Kingdom;

namespace DumDum.Interfaces
{
    public interface IDetailService
    {
        KingdomDetailsResponse KingdomInformation(int kingdomId, string authorization, out int statusCode);
    }
}
