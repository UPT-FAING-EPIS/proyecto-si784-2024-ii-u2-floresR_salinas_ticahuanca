
using System.Collections.Generic;
using NegocioPDF.Models;

namespace NegocioPDF.Repositories
{
public interface IUsuarioRepository
{
    Usuario Login(string correo, string password);
    void RegistrarUsuario(Usuario usuario);
    IEnumerable<Usuario> ObtenerUsuarios();
    Usuario ObtenerUsuarioPorId(int idUsuario);
}
}