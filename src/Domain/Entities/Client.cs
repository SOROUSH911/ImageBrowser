using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageBrowser.Domain.Entities;
public partial class Client
{
    [Key]
    [StringLength(128)]
    //[Column("id")]
    public string Id { get; set; }
    [Required]
    public string Secret { get; set; }
    [Required]
    [StringLength(100)]
    public string? Name { get; set; }
    public int ApplicationType { get; set; }
    public bool Active { get; set; }
    public int RefreshTokenLifeTime { get; set; }
    public int AccessTokenLifeTime { get; set; }
    [StringLength(100)]
    public string? AllowedOrigin { get; set; }
}
