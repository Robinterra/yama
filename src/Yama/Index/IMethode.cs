using System.Collections.Generic;
using Yama.Parser;

namespace Yama.Index
{
    public interface IMethode : IParent
    {

        List<IndexVariabelnReference> References
        {
            get;
            set;
        }

        string KeyName
        {
            get;
        }

        IndexKlassenDeklaration Klasse
        {
            get;
            set;
        }

        bool Mappen();
    }
}