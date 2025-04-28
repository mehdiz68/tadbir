namespace Domain.ViewModel
{
    public class PagingViewModel
    {
        public int Count { get; set; }
        public int CurentPage { get; set; }
        public int PerPage { get; set; }
        public string RawUrl { get; set; }
    }
}
