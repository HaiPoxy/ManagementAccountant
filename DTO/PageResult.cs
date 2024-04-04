namespace AccountManagermnet.DTO
{
    public class PageResult<T> 
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int Pos { get; set; }
        public int total_count { get; set; }
        public List<T> Data { get; set; }

        public PageResult(int page, int pageSize, int pos, int totalCount, List<T> datas)
        {
            this.Page = page;
            this.PageSize = pageSize;
            this.Pos = pos;
            this.total_count = totalCount;
            this.Data = datas;
        }

        public PageResult()
        {
        }
    }

}

