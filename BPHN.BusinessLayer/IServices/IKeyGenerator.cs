using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPHN.BusinessLayer.IServices
{
    public interface IKeyGenerator
    {
        string Encryption(string input);
        string Decryption(string input);
    }
}
