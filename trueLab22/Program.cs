using System;
using System.Data.SqlClient;
using System.Data;
using System.Threading;

namespace itirod_lab2
{
    class Program
    {
        static SqlConnection connection = new SqlConnection(SQLConnect.SqlConnection());
        static int id = 0;
        static string name;
        
        static void Main(string[] args)
        {
            try
            {
                connection.Open();
                Initialization();
            
            id = GetLastId();
            
            Console.WriteLine("ID : " + id);

            Thread myThread = new Thread(new ThreadStart(Count));
            myThread.Start();
            Console.WriteLine("enter u name: ");
            name = Console.ReadLine();

            while (true)
            {
                AddMessage(name);
            }
            }
            catch
            {
                Console.WriteLine("Ошибка подключения к БД");
                Console.ReadLine();
            }
        }

        static void Count()
        {
            while (true)
            {
                int id1 = GetLastId();

                if (id1 > id)
                {
                    OutputLines(id1 - id);
                }

                id = id1;

                Thread.Sleep(500);
            }
        }

        /// <summary>
        ///  Create a dependency connection.
        /// </summary>
        static void Initialization()
        {
            SqlDependency.Start(SQLConnect.SqlConnection(), null);
        }

        /// <summary>
        /// add messege in DB
        /// </summary>
        /// <param name="login">user name</param>
        static void AddMessage(string login)
        {
            string message = Console.ReadLine();
            

            string sql = "INSERT INTO dbo.Data(users, text) VALUES(@users, @text)";

            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = sql;

            // Добавить параметр 
            cmd.Parameters.Add("@users", SqlDbType.VarChar).Value = login;
            cmd.Parameters.Add("@text", SqlDbType.VarChar).Value = message;

            // Выполнить Command (Используется для delete, insert, update).
            int rowCount = cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// return last id in DB
        /// </summary>
        /// <returns></returns>
        static int GetLastId()
        {
            string id2 = "";

            SqlDataAdapter da = new SqlDataAdapter(@"WITH SRC AS (SELECT TOP (1) messegeID, users, " +
                "text FROM Data ORDER BY messegeID DESC) SELECT * FROM SRC ORDER BY messegeID", connection);
            DataSet ds = new DataSet();
            da.Fill(ds);

            foreach (DataTable dt in ds.Tables)
            {
                // перебор всех строк таблицы
                foreach (DataRow row in dt.Rows)
                {
                    // получаем все ячейки строки
                    var cells = row.ItemArray;
                    int i = 0;
                    foreach (object cell in cells)
                    {
                        if (i == 0)
                        {
                            id2 = cell.ToString();
                        }
                        i++;
                    }
                }
            }

            return Convert.ToInt32(id2);
        }

        /// <summary>
        /// whire new messege from DB
        /// </summary>
        /// <param name="count"></param>
        static void OutputLines(int count)
        {
            SqlDataAdapter da = new SqlDataAdapter(@"WITH SRC AS (SELECT TOP (" + count + ") messegeID, users, " +
                "text FROM Data ORDER BY messegeID DESC) SELECT * FROM SRC ORDER BY messegeID", connection);
            DataSet ds = new DataSet();
            da.Fill(ds);

            foreach (DataTable dt in ds.Tables)
            {
                // перебор всех строк таблицы
                foreach (DataRow row in dt.Rows)
                {
                    // получаем все ячейки строки
                    var cells = row.ItemArray;
                    int i = 0;
                    foreach (object cell in cells)
                    {
                        if (i != 0)
                        {
                            Console.Write("{0}", cell);
                        }
                        if (i == 1)
                        {
                            Console.Write(": ");
                        }
                        i++;
                    }
                    Console.WriteLine();
                }
            }
        }
    }
}
