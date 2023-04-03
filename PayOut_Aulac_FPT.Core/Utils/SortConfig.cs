using Microsoft.AspNetCore.DataProtection.KeyManagement;
using PayOut_Aulac_FPT.Core.Entities;
using PayOut_Aulac_FPT.Core.Interfaces;
using PayOut_Aulac_FPT.Core.Utils.Enums;
using PayOut_Aulac_FPT.Core.Utils.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayOut_Aulac_FPT.Core.Utils
{
    public class SortConfig<T> where T : IEntity
    {
        private List<string> _config;

        public SortConfig()
        {
            _config = new List<string>();
        }

        public SortConfig(BaseSort? sort)
        {
            _config = new List<string>();
            Add(sort);
        }

        public bool Add(string propertyName, ETypeSort typeSort)
        {
            if (typeof(T).GetProperty(propertyName) != null)
            {
                _config.Add($"{propertyName} {typeSort}");
                return true;
            }
            return false;
        }

        public void Add(Dictionary<string, ETypeSort> config)
        {
            foreach (var key in config.Keys)
            {
                Add(key, config[key]);
            }
        }

        public void Add(BaseSort? sort)
        {
            if (sort == null) return;
            var properties = sort.GetType().GetProperties();
            foreach (var property in properties)
            {
                var value = property.GetValue(sort);
                if (value != null && value.GetType() == typeof(ETypeSort))
                {
                    Add(property.Name, (ETypeSort)value);
                }
            }
        }

        public string? GetSort()
        {
            if (_config.Count == 0)
            {
                return null;
            }
            return String.Join(", ", _config);
        }
    }
}
