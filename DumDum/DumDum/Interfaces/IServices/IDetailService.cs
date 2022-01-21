using System.Threading.Tasks;
using DumDum.Models.JsonEntities.Kingdom;

namespace DumDum.Interfaces.IServices
{
    public interface IDetailService
    {
        Task<(KingdomDetailsResponse, int)> KingdomInformation(int kingdomId, string authorization);
    }
}
