using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace CRUD_Console
{
    class Program
    {
       static SqlConnection conexion;
       static string CadenaDeConexion = $"Data Source =LAPTOP-10610VU7\\SQLEXPRESS;Initial Catalog=missa;Integrated Security=True;";

        static void Main(string[] args)
        {
            MostrarMenu();
            while (true)
            {
                
                string nombre, correo, nickname;
                int edad,id,eleccion;
                eleccion = Int32.Parse(Console.ReadLine());
                switch (eleccion)
                {
                    case 1:
                        Console.Clear();
                        Console.WriteLine("Agrege el nombre del usuario");
                        nombre = Console.ReadLine();
                        Console.WriteLine("Agrege el correo del usuario");
                        correo = Console.ReadLine();
                        Console.WriteLine("Agrege el nickname de usuario");
                        nickname = Console.ReadLine();
                        Console.WriteLine("Agrege la edad del usuario");
                        edad = Int32.Parse(Console.ReadLine());
                        try
                        {
                            conexion = new SqlConnection(CadenaDeConexion);
                            conexion.Open();
                            SqlCommand Komander = new SqlCommand("insert into Usuario(Nombre,correo,nickname,edad)" +
                                "values(@nombre,@correo,@nickname,@edad)", conexion);
                            Komander.Parameters.AddWithValue("@nombre", nombre);
                            Komander.Parameters.AddWithValue("@correo", correo);
                            Komander.Parameters.AddWithValue("@nickname", nickname);
                            Komander.Parameters.AddWithValue("@edad", edad);
                            Komander.ExecuteNonQuery();
                            conexion.Close();
                            Console.WriteLine("");
                            Console.WriteLine("Usuario agregado con exito, presione enter para continuar");
                            Console.ReadKey();
                            Console.Clear();
                            MostrarMenu();

                        }
                        catch (Exception e)
                        {
                            conexion.Close();
                            Console.WriteLine(e.Message);
                        }
                        break;
                    case 2:
                        LlamarTabla();
                        Console.WriteLine("Seleccione usuario a modificar escribiendo su numero de id");
                        id = Int32.Parse(Console.ReadLine());
                        if (ComprobarRegistro(id) >= 1)
                        {
                            Console.WriteLine("Escriba el nuevo nombre del usuario");
                            nombre = Console.ReadLine();
                            Console.WriteLine("Escriba el nuevo correo del usuario");
                            correo = Console.ReadLine();
                            Console.WriteLine("Escriba el nuevo nickname de usuario");
                            nickname = Console.ReadLine();
                            Console.WriteLine("Escriba la edad actual del usuario");
                            edad = Int32.Parse(Console.ReadLine());
                            try
                            {
                                conexion = new SqlConnection(CadenaDeConexion);
                                conexion.Open();
                                SqlCommand komander = new SqlCommand("UPDATE Usuario SET " +
                                    "Nombre = @nombre, correo = @correo, nickname = @nickname, edad = @edad" +
                                    " Where ID = @id",conexion);
                                komander.Parameters.AddWithValue("@nombre",nombre);
                                komander.Parameters.AddWithValue("@correo", correo);
                                komander.Parameters.AddWithValue("@nickname",nickname);
                                komander.Parameters.AddWithValue("@edad",edad);
                                komander.Parameters.AddWithValue("@id",id);
                                komander.ExecuteNonQuery();
                                conexion.Close();
                                Console.WriteLine("");
                                Console.WriteLine("Usuario editado con exito, presione enter para continuar");

                            }
                            catch (Exception e)
                            {
                                conexion.Close();
                                Console.WriteLine(e.Message);
                                Console.ReadKey();
                            }
                        }
                        else
                        {
                            Console.WriteLine("Registro no encontrado");
                            Console.ReadKey();
                        }
                        
                        Console.Clear();
                        MostrarMenu();
                        break;
                    case 3:
                        Console.Clear();
                        LlamarTabla();
                        Console.WriteLine("");
                        Console.WriteLine("Presione enter para volver");
                        Console.ReadKey();
                        Console.Clear();
                        MostrarMenu();
                        break;
                    case 4:
                        LlamarTabla();
                        string decision;
                        Console.WriteLine("¿Está seguro que desea eliminar este registro? No se podra recuperar");
                        Console.WriteLine("Presione Y para continuar, o enter para volver");
                        decision = Console.ReadLine();
                        if (decision == "y" || decision == "Y")
                        {
                            Console.WriteLine("Seleccione usuario a eliminar escribiendo su numero de id");
                            id = Int32.Parse(Console.ReadLine());
                            if (ComprobarRegistro(id) >= 1)
                            {
                                try
                                {
                                    conexion = new SqlConnection(CadenaDeConexion);
                                    conexion.Open();
                                    SqlCommand komander = new SqlCommand("DELETE FROM Usuario where ID = @id", conexion);
                                    komander.Parameters.AddWithValue("@id", id);
                                    komander.ExecuteNonQuery();
                                    conexion.Close();
                                    Console.WriteLine("");
                                    Console.WriteLine("Usuario eliminado con exito, presione enter para continuar");

                                }
                                catch (Exception e)
                                {
                                    conexion.Close();
                                    Console.WriteLine(e.Message);
                                    Console.ReadKey();
                                }
                            }
                            else
                            {
                                Console.WriteLine("Registro no encontrado");
                                Console.ReadKey();
                            }

                        }

                        Console.Clear();
                        MostrarMenu();
                        break;
                    case 5:
                        Environment.Exit(0);
                        break;      
                }
            }
           
        }
        public static void MostrarMenu()
        {
            Console.WriteLine("Bien venido al CRUD en consola");
            Console.WriteLine("Elije una opción presionando el numero que se indica");
            Console.WriteLine("1. Agregar usuario");
            Console.WriteLine("2. Modificar Usuario");
            Console.WriteLine("3. Ver Usuarios");
            Console.WriteLine("4. Eliminar Usuario");
            Console.WriteLine("5. Salir");
        }
        public static int ComprobarRegistro(int Madison)
        {
            int Registro = 0;
            DataTable tb = new DataTable();
            try
            {
                conexion = new SqlConnection(CadenaDeConexion);
                conexion.Open();
                string query = "Select * from Usuario where ID ="+Madison.ToString()+"";
                SqlCommand komander = new SqlCommand(query,conexion);
                SqlDataAdapter adp = new SqlDataAdapter(komander);
                adp.Fill(tb);
                Registro = tb.Rows.Count;
                conexion.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);

            }
            return Registro;
        }

        public static void LlamarTabla()
        {  
            try
            {
                conexion = new SqlConnection(CadenaDeConexion);
                conexion.Open();
                SqlCommand komander = new SqlCommand("Select * From Usuario", conexion);
                SqlDataReader Potter = komander.ExecuteReader();
                Console.WriteLine("ID\tNombre\tcorreo\tnickname\tedad");
                while (Potter.Read())
                {
                    Console.WriteLine($"{Potter["ID"]}\t{Potter["Nombre"]}\t{Potter["correo"]}\t{Potter["nickname"]}\t{Potter["edad"]}");
                }
                Potter.Close();
                conexion.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                conexion.Close();
            }
            
        }
    }
}


