using System;
using System.Collections.Generic;

namespace SFERS.Models.Entities;

public class RoomType
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }

}
