﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DynamicTyped.Data.MdxClient
{
    internal class Tuple
    {
        public string Axis { get; set; }
        public List<Member> Members { get; set; }
    }
}