﻿using BPHN.BusinessLayer.IServices;
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
            if (pageIndex < 1) pageIndex = 1;
            if (pageSize <= 0 || pageSize > 100) pageSize = 50;

            var resultCountPaging = _pitchRepository.GetCountPaging(pageIndex, pageSize, txtSearch);
            return new ServiceResultModel()
            {
                Success = true,
                Data = resultCountPaging
            };
        }

        public ServiceResultModel GetPaging(int pageIndex, int pageSize, string txtSearch)
        {
            if (pageIndex < 1) pageIndex = 1;
            if (pageSize <= 0 || pageSize > 100) pageSize = 50;

            var resultPaging = _pitchRepository.GetPaging(pageIndex, pageSize, txtSearch);
            return new ServiceResultModel()
            {
                Success = true,
                Data = resultPaging
            };
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