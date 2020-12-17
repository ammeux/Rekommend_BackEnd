
namespace Rekommend_BackEnd.ResourceParameters
{
    public class ResourceParametersAbstract
    {
        const int maxPageSize = 20;
        private int _pageSize = 10;

        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > maxPageSize) ? maxPageSize : value;
        }

        public int PageNumber { get; set; } = 1;

        public string SearchQuery { get; set; }
        public string Fields { get; set; }
    }
}
