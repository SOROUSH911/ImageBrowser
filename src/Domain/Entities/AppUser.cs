using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageBrowser.Domain.Entities;
public class User : BaseAuditableEntity
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public int? ImageId { get; set; }
    public File? Image { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public DateTime? LastTimeSent { get; set; }
    public bool IsDeleted { get; set; }


}
