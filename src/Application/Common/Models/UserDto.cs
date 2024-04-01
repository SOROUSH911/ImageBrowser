using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageBrowser.Application.Common.Models;
public class UserDto
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string ImageUrl { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    //public string UserName { get; set; }
    public int? ImageId { get; set; }
    public List<string> Roles { get; set; }
}

public class DeleteUserDto
{
    public string PhoneNumber { get; set; }
    public string Email { get; set; }
}
public class CreateUserDto : UserDto
{
    //public string UserName { get; set; }
    public string Password { get; set; }
}
