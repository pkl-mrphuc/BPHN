namespace BPHN.ModelLayer.ViewModels
{
    public class MailVm<T>
    {
        public dynamic ViewBag { get; set; }
        public T Model { get; set; }
    }
}
