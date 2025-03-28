namespace BPHN.ModelLayer
{
    public class Location
    {
        public IEnumerable<Province> Provinces { get; set; }
        public IEnumerable<District> Districts { get; set; }
        public IEnumerable<Ward> Wards { get; set; }
    }

    public class Province
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class District
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class Ward
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }   
}
