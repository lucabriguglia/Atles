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

        public Pagination(int? currentPage)
        {
            CurrentPage = currentPage ?? 1;
        }
    }

    public class PaginatedData<T> where T : class
    {
        public int TotalPages { get; set; }
        public int TotalRecords { get; set; }
        //public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 2;

        public int PagerSize { get; set; } = 10;
        //public int StartPage { get; set; }
        //public int EndPage { get; set; }

        //public int Skip => (CurrentPage - 1) * PageSize;

        public IList<T> Items { get; set; } = new List<T>();

        public PaginatedData()
        {
            
        }

        public PaginatedData(IList<T> items, int totalRecords)
        {
            Items = items;
            TotalRecords = totalRecords;
            TotalPages = (int)Math.Ceiling(TotalRecords / (decimal)PageSize);
        }
    }
}
