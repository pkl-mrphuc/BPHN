﻿using System.ComponentModel.DataAnnotations;

namespace BPHN.ModelLayer
{
    public class Config : BaseModel
    {
        [MaxLength(255)]
        public string Key { get; set; }
        [MaxLength(int.MaxValue)]
        public string Value { get; set; }
        public Guid AccountId { get; set; }
    }
}
