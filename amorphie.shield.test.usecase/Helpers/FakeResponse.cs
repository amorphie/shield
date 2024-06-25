using amorphie.core.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace amorphie.shield.test.Helpers;
public class FakeResponse<T>
{
    public FakeResponse()
    {

    }
    public FakeResult Result { get; set; }
    public T Data { get; set; } = default!;
}

public class FakeResult
{
    public FakeResult()
    {

    }
    public string Status { get; set; }
    public string Message { get; set; } = string.Empty;
    public string MessageDetail { get; set; } = string.Empty;
}

