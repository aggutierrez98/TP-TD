using System.Data;
using BE;

namespace DAL.Mappers
{
    public class UsuarioMapper
    {
        public Usuario Map(DataRow row)
        {
            return new Usuario
            {
                Id = (int)row["Id"],
                Username = row["Username"].ToString(),
                PasswordHash = row["PasswordHash"].ToString()
            };
        }
    }
}
