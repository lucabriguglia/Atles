using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Atlas.Models
{
    public class Pagination
    {
        //public int TotalPages => (int)Math.Ceiling(TotalRecords / (decimal)PageSize);
        //public int TotalRecords { get; set; }
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 2;
        //public int PagerSize { get; set; }
        //public int StartPage { get; set; }
        //public int EndPage { get; set; }

        public int Skip => (CurrentPage - 1) * PageSize;

        public Pagination()
        {
            
        }

        public Pagination(int currentPage)
        {
            CurrentPage = currentPage;
        }
    }

    public class PaginatedData<T> where T : class
    {
        public int TotalPages => (int)Math.Ceiling(TotalRecords / (decimal)PageSize);
        public int TotalRecords { get; }
        //public int CurrentPage { get; set; } = 1;
        public int PageSize { get; } = 2;
        //public int PagerSize { get; set; }
        //public int StartPage { get; set; }
        //public int EndPage { get; set; }

        //public int Skip => (CurrentPage - 1) * PageSize;

        private readonly List<T> _items;
        public ReadOnlyCollection<T> Items => _items.AsReadOnly();

        public PaginatedData()
        {
            
        }

        public PaginatedData(List<T> items, int totalRecords)
        {
            _items = items;
            TotalRecords = totalRecords;
        }
    }
}
