using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Demo.DDD.WithEFCore.API.Extensions
{
    public static class Conversions
    {
        public static Entities.Order ToEntity(this DTO.Order dto) 
        { 
            // ToDo: Impplement conversion
            return new Entities.Order{};
        }

        public static DTO.Order ToDto(this Entities.Order entity) 
        {
            // ToDo: Impplement conversion
            return new DTO.Order{};

        }
    }
}
