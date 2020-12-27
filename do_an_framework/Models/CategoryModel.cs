using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Configuration;

namespace do_an_framework.Models
{
    public class CategoryModel
    {
        public int id { get; set; }

        public string name { get; set; }

        public string description { get; set; }

        public CategoryModel()
        {
        }

        public CategoryModel(int Id, string Name, string des)
        {
            this.id = Id;
            this.name = Name;
            this.description = des;
        }
    }
}
