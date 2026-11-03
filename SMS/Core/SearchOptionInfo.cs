using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeflexSystems.SMS.Core
{
    [Serializable]
    public sealed class SearchOptionInfo<TOption>
    {
        public string Name { get; set; }
        public string Information { get; set; }
        public TOption Option { get; set; }
        public SearchValueType SearchValueType { get; set; }
        public bool Nullable { get; set; }
    }
}
