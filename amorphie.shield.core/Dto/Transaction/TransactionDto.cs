using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using amorphie.core.Base;

namespace amorphie.shield.core.Dto.Transaction;

public class TransactionDto : DtoBase
{
    public string EncryptData { get; set; } = default!;

}
