using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedKernel.DTOs;

// Result class to match SP return: SELECT @Id AS Id
public class IdResult
{
    public Guid Id { get; set; }
}
