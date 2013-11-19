﻿using System.Collections.Generic;

namespace MdxClient
{
    internal class ResultSet
    {
        public List<Column> Columns { get; set; }
        public List<Row> Rows { get; set; }

        public ResultSet()
        {
            Columns = new List<Column>();
            Rows = new List<Row>();
        }
    }
}
