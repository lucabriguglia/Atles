﻿using System;
using System.Collections.Generic;

namespace Atles.Reporting.Models.Shared
{
    public class PaginatedData<T> where T : class
    {
        public int TotalPages { get; set; }
        public int TotalRecords { get; set; }

        public IList<T> Items { get; set; } = new List<T>();

        public PaginatedData()
        {
            
        }

        public PaginatedData(IList<T> items, int totalRecords, int pageSize)
        {
            Items = items;
            TotalRecords = totalRecords;
            TotalPages = (int)Math.Ceiling(TotalRecords / (decimal)pageSize);
        }
    }
}