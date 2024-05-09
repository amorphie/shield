using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using amorphie.core.Base;

namespace amorphie.shield.core.Model;

public class Transaction : EntityBase
{
    public string EncryptedData { get; set; } = default!;
    public Certificate Certificate { get; set; } = default!;
}
