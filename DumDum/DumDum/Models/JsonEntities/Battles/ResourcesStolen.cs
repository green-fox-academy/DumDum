using Castle.Components.DictionaryAdapter;
using Microsoft.EntityFrameworkCore;

namespace DumDum.Models.JsonEntities.Battles
{
    [Keyless]
    public class ResourcesStolen
    {
        public int Gold { get; set; }
        public int Food { get; set; }
        
    }
}