using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace compiler.Scanners
{
    public interface IScanner
    {
        bool NextToken();
    }
}
