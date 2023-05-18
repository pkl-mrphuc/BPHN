using BPHN.DataLayer.IRepositories;
using BPHN.ModelLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPHN.DataLayer.ImpRepositories
{
    public class PitchRepository : IPitchRepository
    {
        public object GetCountPaging(int pageIndex, int pageSize, string txtSearch)
        {
            return new { TotalPage = 1, TotalRecordCurrentPage = 10, TotalAllRecords = 100 };
        }

        public List<Pitch> GetPaging(int pageIndex, int pageSize, string txtSearch)
        {
            return new List<Pitch>();
        }
    }
}
