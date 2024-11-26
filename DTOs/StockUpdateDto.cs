using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.DTOs
{
    public class StockUpdateDto
{
    public Guid ProductId { get; set; }
    public int QuantityToSubtract { get; set; }
}

}