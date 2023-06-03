using BPHN.ModelLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPHN.BusinessLayer.IServices
{
    public interface IPitchService
    {
        ServiceResultModel Insert(Pitch pitch);
        ServiceResultModel Update(Pitch pitch);
        ServiceResultModel GetInstance(string id);
        ServiceResultModel GetPaging(int pageIndex, int pageSize, string txtSearch);
        ServiceResultModel GetCountPaging(int pageIndex, int pageSize, string txtSearch);
    }
}
