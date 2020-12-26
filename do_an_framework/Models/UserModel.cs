using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace do_an_framework.Models
{
    public class UserModel
    {

        public int user_id { get; set; }

        public string user_name { get; set; }

        public string user_phone { get; set; }

        public int user_type { get; set; }

        public string user_email { get; set; }

        public string user_password { get; set; }

        public string user_image {get; set;}

        public DateTime user_last_login { get; set; }

        public DateTime user_created_at { get; set; }

        public DateTime user_updated_at { get; set; }
    }
}
