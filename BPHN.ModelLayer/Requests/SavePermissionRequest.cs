﻿namespace BPHN.ModelLayer.Requests
{
    public class SavePermissionRequest
    {
        public Guid Id { get; set; }
        public int FunctionType { get; set; }
        public bool Allow { get; set; }
    }
}
