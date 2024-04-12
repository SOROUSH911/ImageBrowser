﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageBrowser.Application.Common.Models;
public class QueryResult<T>
{
    public List<T> Data { get; set; }
    public int Total { get; set; }
}
