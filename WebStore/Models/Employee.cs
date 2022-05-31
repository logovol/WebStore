using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebStore.Models
{
    public class Employee
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Patronymic { get; set; } = null!;
        public int Age { get; set; }

        public override string ToString() => $"(id:{Id}){LastName}{Name}{Patronymic} - age:{Age}";
    }
}
