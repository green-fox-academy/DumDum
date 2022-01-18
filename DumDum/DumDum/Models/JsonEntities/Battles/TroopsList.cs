using Microsoft.EntityFrameworkCore;

namespace DumDum.Models.JsonEntities.Battles
{
    [Keyless]
    public class TroopsList
    {
        public string Type { get; set; }
        public int Quantity { get; set; }
    }
}