using BPHN.BusinessLayer.IServices;
using BPHN.DataLayer.IRepositories;
using BPHN.ModelLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPHN.BusinessLayer.ImpServices
{
    public class PitchService : IPitchService
    {
        private readonly IPitchRepository _pitchRepository;
        public PitchService(IPitchRepository pitchRepository)
        {
            _pitchRepository = pitchRepository;
        }

        public ServiceResultModel GetCountPaging(int pageIndex, int pageSize, string txtSearch)
        {
            throw new NotImplementedException();
        }

        public ServiceResultModel GetPaging(int pageIndex, int pageSize, string txtSearch)
        {
            throw new NotImplementedException();
        }

        public ServiceResultModel Insert(Pitch pitch)
        {
            throw new NotImplementedException();
        }

        public ServiceResultModel Update(Pitch pitch)
        {
            throw new NotImplementedException();
        }
    }
}
