using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Mapping.Attributes;
using System.Threading.Tasks;

namespace Kino.Entity
{
    [Serializable]
    [Class(Schema = "cinema", Table = "employees", NameType = typeof(Employees))]
    public class Employees
    {
        [Id(Name = "Id", Column = "ID_employee", Type = "int"), Generator(1, Class = "native")]
        public virtual int Id { get; set; }

        [Property(Column = "email", Type = "string", NotNull = true, Length = 250, Unique =true)]
        public virtual string email { get; set; }

        [Property(Column = "pass", Type = "string", NotNull = true, Length = 255)]
        public virtual string pass { get; set; }
    }
}
