using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Ledger.Domain.Enums
{
    public enum Direction
    {
        [Display(Name = "debit")]
        Debit,
        [Display(Name = "credit")]
        Credit
    }
}
