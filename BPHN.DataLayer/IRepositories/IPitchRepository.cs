using BPHN.ModelLayer;
using BPHN.ModelLayer.Others;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPHN.DataLayer.IRepositories
{
    public interface IPitchRepository
    {
        List<Pitch> GetPaging(int pageIndex, int pageSize, List<WhereCondition> where);
        object GetCountPaging(int pageIndex, int pageSize, List<WhereCondition> where);
        Pitch? GetById(string id);
        bool Insert(Pitch pitch);
        bool Update(Pitch pitch);
    }
}
