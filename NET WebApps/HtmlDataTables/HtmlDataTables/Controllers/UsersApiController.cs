using Bogus;
using System;
using System.Collections.Generic;
using System.Web.Http;
namespace HtmlDataTables.Controllers
{
    public class UsersApiController : ApiController
    {
        [HttpGet]
        public IHttpActionResult PostGetAll()
        {
            var testUsers = new Faker<UserDTO>()
                .RuleFor(p => p.Id, f => Guid.NewGuid())
                .RuleFor(p => p.FirstName, f => f.Name.FirstName())
                .RuleFor(p => p.LastName, f => f.Name.LastName())
                .RuleFor(p => p.Email, f => f.Person.Email);

            var users = testUsers.Generate(200);
            var result = new GetUsersResult(users, 1, users.Count);
            return Json(result);
        }
    }

    public class GetUsersResult : GetResult<UserDTO>
    {
        public GetUsersResult(List<UserDTO> dto, int current, int page)
        {
            this.rows = dto.ToArray();
            this.current = current;
            this.rowCount = rows.Length/page;
            total = rows.Length;
        }
    }
    public class GetResult<T> where T : class, new()
    {
        public int current { get; protected set; }
        public int rowCount { get; protected set; }
        public T[] rows { get; protected set; }
        public int total { get; protected set; }
    }
    public class UserDTO
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
    }
}
