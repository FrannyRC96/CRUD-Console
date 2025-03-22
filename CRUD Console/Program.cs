using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace CRUD_Console
{
    //Esta versión utiliza la metodologia SOLID que es una forma de programar.
    
    //Esta clase representa al usuario (Single Responsability Principle) 
    //El principio SRP significa que una clase debe encargarse solo de una cosa, en este caso solo define al usuario.
    public class Usuario
    {
        public int ID { get; set; }
        public string Nombre { get; set; }
        public string correo { get; set; }
        public string nickname { get; set; }
        public int edad { get; set; }
    }
    //Aquí se integra la capa de datos, aplicando dos principios:
    //Interface Segregation Principle, y Dependency Inversion principle
    /**Es decir que aquí definimos que propiedades y metodos va a tener esta interface... si bien, pudiera dividirlo 
     * en interfaces más pequeñas, decidi dejar las 4 operaciones del cruden esta interface.
     * Ademas Usuario nodependera de otra clase sino de esta interface
    */
    public interface IUsuarioRepository
    {
        void Create(Usuario usuario);
        List<Usuario> Read();
        void Update(Usuario usuario);
        void Delete(int id);
        bool Existe(int id);

    }
    /*Aquí volvemos al principio de responsabilidad unica (SRP) pues, 
     la clase Usuario se encarga de definir los atributos del usuario, pero esta
    implementa los metodos de la interface que interactuan con la base de datos*/
    public class UsuarioRepository : IUsuarioRepository
    {
        //A esta variable le pasaremos la cadena de conexión
        private readonly string _cadenaConexion;
        public UsuarioRepository(string cadenaConexion)
        {
            _cadenaConexion = cadenaConexion;
        }

        private SqlConnection ObtenerConexion() => new SqlConnection(_cadenaConexion);
        //Create es para agregar usuarios
        public void Create(Usuario usuario)
        {
            using (var conexion = ObtenerConexion())
            {
                var query = "insert into Usuario(Nombre,correo,nickname,edad)values(@nombre,@correo,@nickname,@edad)";
                conexion.Open();
                using (var komander = new SqlCommand(query, conexion))
                {
                    komander.Parameters.AddWithValue("@nombre", usuario.Nombre);
                    komander.Parameters.AddWithValue("@correo", usuario.correo);
                    komander.Parameters.AddWithValue("@nickname", usuario.nickname);
                    komander.Parameters.AddWithValue("@edad", usuario.edad);
                    komander.ExecuteNonQuery();
                }
            }
        }
        //Esta metodo es para actualizar un usuario
        public void Update(Usuario usuario)
        {
            using (var conexion = ObtenerConexion())
            {
                var query = "UPDATE Usuario SET Nombre = @nombre, correo = @correo, nickname = @nickname, edad = @edad Where ID = @id";
                conexion.Open();
                using (var komander = new SqlCommand(query, conexion))
                {
                    komander.Parameters.AddWithValue("@id", usuario.ID);
                    komander.Parameters.AddWithValue("@nombre", usuario.Nombre);
                    komander.Parameters.AddWithValue("@correo", usuario.correo);
                    komander.Parameters.AddWithValue("@nickname", usuario.nickname);
                    komander.Parameters.AddWithValue("@edad", usuario.edad);
                    komander.ExecuteNonQuery();
                }
            }
        }
        //Este metodo es para eliminar un registro
        public void Delete(int id)
        {
            using (var conexion = ObtenerConexion())
            {
                var query = "DELETE FROM Usuario where ID = @id";
                conexion.Open();
                using (var komander = new SqlCommand(query, conexion))
                {
                    komander.Parameters.AddWithValue("@id", id);
                    komander.ExecuteNonQuery();
                }
            }
        }
        //Aquí lo usamos para mostrar la lista de usuarios existentes.
        public List<Usuario> Read()
        {
            var usuarios = new List<Usuario>();
            using (var conexion = ObtenerConexion())
            {
                conexion.Open();
                var query = "Select * from Usuario";
                using (var komander = new SqlCommand(query, conexion))
                using (var potter = komander.ExecuteReader())
                {
                    while (potter.Read())
                    {
                        usuarios.Add(new Usuario
                        {
                            ID = Convert.ToInt32(potter["ID"]),
                            Nombre = potter["Nombre"].ToString(),
                            correo = potter["correo"].ToString(),
                            nickname = potter["nickname"].ToString(),
                            edad = Convert.ToInt32(potter["edad"])
                        });
                    }
                }

            }
            return usuarios;
        }
        //Este metodo comprueba que un registro exista, de no hacerlo no lo puedes ni editar ni eliminar.
        public bool Existe(int id)
        {
            using (var conexion = ObtenerConexion())
            {
                conexion.Open();
                var query = "SELECT COUNT(*) FROM Usuario WHERE ID = @Id";
                using (var cmd = new SqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    return (int)cmd.ExecuteScalar() > 0;
                }
            }
        }

    }
    /*Aquí de nuevo utilizamos el principio de responsabilidad unica y de inversión de dependencias
     Esto se da porque aquí los datos para hacer uso del CRUD, ya sea agregar, editar, eliminar o ver todos los usuarios
     ademas, mandamos los parametros a la interface de usuario y de ese modo los procesa.*/
   
    public class UsuarioService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        public UsuarioService(IUsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }
        public void AgregarUsuario()
        {
            Console.WriteLine("Ingrese el nombre:");
            string nombre = Console.ReadLine();
            Console.WriteLine("Ingrese el correo:");
            string correo = Console.ReadLine();
            Console.WriteLine("Ingrese el nickname:");
            string nickname = Console.ReadLine();
            Console.WriteLine("Ingrese la edad:");
            int edad = int.Parse(Console.ReadLine());

            _usuarioRepository.Create(new Usuario { Nombre = nombre, correo = correo, nickname = nickname, edad = edad });
            Console.WriteLine("Usuario agregado con éxito.");
        }
        public void ActualizarUsuario()
        {
            Console.WriteLine("Ingrese el ID del usuario que va a actualizar");
            int id = int.Parse(Console.ReadLine());
            if (!_usuarioRepository.Existe(id))
            {
                Console.WriteLine("El usuario no existe");
                return;
            }
            Console.WriteLine("Ingrese el nuevo nombre: ");
            string nombre = Console.ReadLine();
            Console.WriteLine("Ingrese el nuevo correo: ");
            string correo = Console.ReadLine();
            Console.WriteLine("Ingrese el nuevo nickname: ");
            string nickname = Console.ReadLine();
            Console.WriteLine("Ingrese la edad: ");
            int edad = int.Parse(Console.ReadLine());

            _usuarioRepository.Update(new Usuario { ID = id, Nombre = nombre, correo = correo, nickname = nickname, edad = edad });
            Console.WriteLine("Usuario actualizado con éxito.");
        }
        public void MostrarUsuarios()
        {
            var usuarios = _usuarioRepository.Read();
            foreach (var usuario in usuarios)
            {
                Console.WriteLine($"{usuario.ID} | {usuario.Nombre} | {usuario.correo} | {usuario.nickname} | {usuario.edad}");
            }
        }
        public void EliminarUsuario()
        {
            Console.WriteLine("Ingrese el ID del usuario a eliminar:");
            int id = int.Parse(Console.ReadLine());
            if (_usuarioRepository.Existe(id))
            {
                _usuarioRepository.Delete(id);
                Console.WriteLine("Usuario eliminado con éxito.");
            }
            else
            {
                Console.WriteLine("El usuario no existe.");
            }
        }
    }
    //Este es el MAIN 
    class Program
    {
        static void Main(string[] args)
        {
            //Esta es la conexion sql, aquí debes poner tu conexión SQL a tu base de datos propia
            string cadenaConexion = "";
            IUsuarioRepository usuarioRepo = new UsuarioRepository(cadenaConexion);
            UsuarioService usuarioService = new UsuarioService(usuarioRepo);

            while (true)
            {
                Console.Clear();
                Console.WriteLine("1. Agregar Usuario");
                Console.WriteLine("2. Editar Usuario");
                Console.WriteLine("3. Ver Usuarios");
                Console.WriteLine("4. Eliminar Usuario");
                Console.WriteLine("5. Salir");
                Console.Write("Seleccione una opción: ");

                switch (Console.ReadLine())
                {
                    case "1":
                        usuarioService.AgregarUsuario();
                        break;
                    case "2":
                        usuarioService.ActualizarUsuario();               
                        break;
                    case "3":
                        usuarioService.MostrarUsuarios();
                        break;
                    case "4":
                        usuarioService.EliminarUsuario();
                        break;
                    case "5":
                        return;
                    default:
                        Console.WriteLine("Opción inválida.");
                        break;
                }
                Console.WriteLine("\nPresione una tecla para continuar...");
                Console.ReadKey();
            }
        }
    }
}


