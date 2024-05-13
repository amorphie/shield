using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace amorphie.shield.core.Constant;
public class CertificateStatus
{
    protected CertificateStatus()
    {

    }
    public const string Active ="Active";
    public const string Passive = "Passive";
    public const string Revoked = "Revoked";
    public const string Expired = "Expired";
}

