using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using amorphie.core.Base;

namespace amorphie.shield.core.Model;

public class Certificate : EntityBase
{
    public string SerialNumber { get; set; } = default!;
    public string CertificateData { get; set; } = default!;
    public string PrivateKey { get; set; } = default!;
    public string Status { get; set; } = default!;
    public DateTime RevocationDate { get; set; }
    public DateTime ExpirationDate { get; set; }

}
