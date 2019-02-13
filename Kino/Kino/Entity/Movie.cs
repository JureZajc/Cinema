using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate.Mapping.Attributes;

namespace Kino.Entity
{
    [Serializable]
    [Class(Schema ="cinema", Table ="movie", NameType =typeof(Movie))]
    public class Movie
    {
        [Id(Name = "Id", Column = "ID_movie", Type = "int"), Generator(1, Class = "native")]
        public virtual int Id { get; set; }

        [Property(Column = "movie_name", Type = "string", NotNull = true, Length = 250)]
        public virtual string movie_name { get; set; }

        [Property(Column = "visitors", Type = "string", NotNull = true, Length = 255)]
        public virtual string visitors { get; set; }
    }
}

