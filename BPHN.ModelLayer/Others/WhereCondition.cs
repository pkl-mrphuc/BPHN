namespace BPHN.ModelLayer.Others
{
    public class WhereCondition
    {
        public string Column { get; set; }
        public string Operator { get; set; }
        public object Value { get; set; }
        public object[] Values { get; set; }
    }
}
