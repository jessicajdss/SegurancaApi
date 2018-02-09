using System;
using System.Linq;
using modelobasicoefjwt.Models;

namespace modelobasicoefjwt.Repositorio
{
    public class CodeFirstBanco
    {
        public static void Inicializar(AutenticacaoContext contexto){

            if(contexto.Usuarios.Any()) return;

            var usuario = new Usuario(){
                Nome = "Fernando",
                Email = "fernando.guerra@corujasdev.com.br",
                Senha = "123456",
                DataNascimento = Convert.ToDateTime("10-10-1978"),
                Cpf = "211.222.111-45"
            };

            contexto.Usuarios.Add(usuario);

            var permissao = new Permissao(){
                Nome = "Financeiro"
            };

            contexto.Permissoes.Add(permissao);

            var usuariopermissao = new UsuarioPermissao(){
                IdUsuario = usuario.IdUsuario,
                IdPermissao = permissao.IdPermissao
            };

            contexto.UsuariosPermissoes.Add(usuariopermissao);
            contexto.SaveChanges();
            
        }
    }
}