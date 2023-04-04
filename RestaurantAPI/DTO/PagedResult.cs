namespace RestaurantAPI.DTO
{
    /// <summary>
    /// Ustalamy generyczną klasę ponieważ nie chcemy hardcode'owac tego tylko do klasy Restaurant DTO poniewaz inne typy moga z tej klasy i zostac zwrocone w podobny spoosb
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PagedResult<T>
    {
        public List<T> Items { get; set; }
        public int TotalPages { get; set; }
        public int ItemFrom { get; set; }
        public int ItemsTo { get; set; }
        public int TotalItemsCount { get; set; }

        public PagedResult(List<T> items, int totalCount, int pageSize, int pageNumber)
        {
            Items = items;
            TotalItemsCount = totalCount;
            ItemFrom = pageSize * (pageNumber - 1) + 1;
            ItemsTo = ItemFrom + pageSize - 1;
            TotalPages = (int)Math.Ceiling(totalCount /(double) pageSize);
        }

    }
}
