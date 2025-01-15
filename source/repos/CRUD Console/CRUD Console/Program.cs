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
       //this is the variable for the SqlConnection
       static SqlConnection conexion;
        //In this case this is the string to get the database source, you Should to change to your 
       static string CadenaDeConexion = "";

        static void Main(string[] args)
        {
            //This method shows the menu
            MostrarMenu();
            // We need to keep the loop running to prevent the console from closing automatically.
            while (true)
            {
                //this is the variables of the user
                string nombre, correo, nickname;
                int edad,id,eleccion;
                //eleccion means choice, and is the variable for coice in the menu.
                eleccion = Int32.Parse(Console.ReadLine());
                switch (eleccion)
                {
                    case 1:
                        //Case one we will add a new user
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
                            //we open the SqlConnection and make the insert
                            SqlCommand Komander = new SqlCommand("insert into Usuario(Nombre,correo,nickname,edad)" +
                                "values(@nombre,@correo,@nickname,@edad)", conexion);
                            //we will use referenced parameters.
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
                            //in case of any issue we will close the conection and show the message.
                            conexion.Close();
                            Console.WriteLine(e.Message);
                        }
                        break;
                    case 2:
                        //case 2 is for edit or update an user, and first we need to see the users table
                        LlamarTabla();
                        //and then we make an selection
                        Console.WriteLine("Seleccione usuario a modificar escribiendo su numero de id");
                        id = Int32.Parse(Console.ReadLine());
                        //ComprobarRegistro is a method to verify that an user exist in the database
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
                        //if the user doesn't exist we tell that to the user.
                        else
                        {
                            Console.WriteLine("Registro no encontrado");
                            Console.ReadKey();
                        }
                        
                        Console.Clear();
                        MostrarMenu();
                        break;
                    case 3:
                        //This method just show the existing users
                        Console.Clear();
                        LlamarTabla();
                        Console.WriteLine("");
                        Console.WriteLine("Presione enter para volver");
                        Console.ReadKey();
                        Console.Clear();
                        MostrarMenu();
                        break;
                    case 4:
                        //This method is for delete an user
                        LlamarTabla();
                        string decision;
                        Console.WriteLine("¿Está seguro que desea eliminar este registro? No se podra recuperar");
                        Console.WriteLine("Presione Y para continuar, o enter para volver");
                        decision = Console.ReadLine();
                        //For that the user firstly need to pres Y means yes, to feel sure of that user will be deleted.
                        if (decision == "y" || decision == "Y")
                        {
                            Console.WriteLine("Seleccione usuario a eliminar escribiendo su numero de id");
                            id = Int32.Parse(Console.ReadLine());
                            if (ComprobarRegistro(id) >= 1)
                            {
                                try
                                {
                                    //we just will need the ID
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
                            //Obviusly the program need to find the record
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
                        //This is for exit of the program
                        Environment.Exit(0);
                        break;
                    default:
                        //In case the user press an non existin option this will invalidate it.
                        Console.WriteLine("Elija una opción valida");
                        Console.Clear();
                        MostrarMenu();
                        break;
                }
            }
           
        }
        //This is the user menu, is easier to show it with a method than write it a lot of times
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
        //This is the method that verify that an record exist
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
        //This method show the registred users
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


