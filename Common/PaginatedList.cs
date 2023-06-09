namespace QuanLyNhac.Common
{
    public class PaginatedList<T> : List<T>
    {
        /*
         *  Thong tin phan trang:
         *  1. So luong ban ghi/trang (page size). Vi du: 10 items/page
           
         *  2. Tong so ban ghi: 121 record (count)
              => Tong so trang (TotalPages) = Tong so ban ghi(count)/ PageSize
           Vi du: 121/10 = 12.1 => Lam tron len (ceil/round/floor) 
            => ceil(12.1) = 13  => Lay gia tri nay =>trang 13 = 1 item/page
            => floor(12.1) = 12
            => round(12.1) = 12
         
         *  3. Trang hien tai: pageIndex = 3
           => 1,2,3,... ,121
           => page = 3 => co nhung ban ghi nao???
           => page 1=> [1,2,3,..10]
              page 2=> [11,20]
              page 3 =>[21,30]

        => (pageIndex-1)*pageSize + 1 => From
           To = PageIndex*pageSize

            cong thuc: items = [Loại bỏ các bản ghi của (pageIndex-1)*pageSize] -> Lấy pageSize từ pageIndex
           = [Loại bỏ bản ghi của (3-1)*10 = 20 bản ghi => lấy PageSize = 10 bản ghi tiếp theo]
         * */
        public int PageIndex { get; private set; }
        public int TotalPages { get; private set; }
        public int PageSize { get; set; }
        public int TotalRecords { get; set; }
        public PaginatedList(List<T> items, int count, int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            PageSize= pageSize;
            TotalRecords= count;

            TotalPages = (int)Math.Ceiling((double)count / pageSize);

            this.AddRange(items);
        }

        public bool HasPreviousPage => PageIndex > 1;
        public bool HasNextPage => PageIndex < TotalPages;

        public static async Task<PaginatedList<T>> CreateAsync(IEnumerable<T> source, int pageIndex, int pageSize)
        {
            var count = source.Count();
            // Skip: bỏ qua số lượng bản ghi = (pageIndex - 1) * pageSize
            // Take: lấy = pageSize bản ghi

            var items = source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            return new PaginatedList<T>(items, count, pageIndex, pageSize);
        }
    }
}
