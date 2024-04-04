namespace AccountManagermnet.DTO
{
    public class ProductCategoryDTO
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Unit { get; set; }
        public string RevenueAcc { get; set; }
        public string GoodsAcc { get; set; }
        public string GOGSAcc { get; set; }
        public List<string> GoodReceivedNoteDetails { get; set; }

    }
}
