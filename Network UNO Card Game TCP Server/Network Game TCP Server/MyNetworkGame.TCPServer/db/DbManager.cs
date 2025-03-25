using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
//using MySql.Data.MySqlClient;
using MySqlConnector;
using Newtonsoft.Json;

namespace MyNetworkGame.TCPServer
{
    public enum SqlResultType
    {
        illegal = 1,
        error = 2,
        yes = 3,
        no = 4,
    }

    public class DbManager
    {
        public static MySqlConnection mysql;

        public static bool Connect(string db, string ip, int port, string user, string pw)
        {
            mysql = new MySqlConnection();
            string s = string.Format("Database={0};Data Source={1};port={2};User Id={3};Password={4}",
                db, ip, port, user, pw);
            mysql.ConnectionString = s;

            try
            {
                mysql.Open();
                Console.WriteLine("[mysql]连接成功");
                /*
                在Net8.0（<TargetFramework>net8.0</TargetFramework>）项目中，使用MySql.Data.dll，
                需要在命令行手动执行下面4个命令，安装这4个包：
                dotnet add package System.Drawing.Common --version 8.0.0
                dotnet add package System.Security.Permissions --version 9.0.0
                dotnet add package System.Configuration.ConfigurationManager --version 8.0.0
                dotnet add package System.Management --version 8.0.0 

                上面使用Mysql.Data.dll的方案放弃，这个只能用于windows平台，
                改为使用MySqlConnector更方便，也是跨平台（还可用于linux），具体方法：
                1、终端/命令行中，切换 cd到 项目路径（解决方案的子目录），dotnet add package MySqlConnector
                2、代码里，using MySqlConnector; 删掉 using MySql.Data.MySqlClient;
                两部就ok了

                同样的，Newtonsoft.json.dll库，也不通过手动添加dll的方式，
                而是改为终端中执行： dotnet add package Newtonsoft.Json
                */
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("[mysql]连接失败 " + ex.ToString());
                return false;
            }
        }

        //判定安全字符串
        /*
        IsSafeString 方法试图通过正则表达式过滤掉一些特殊字符，但这并不是一个安全的解决方案，
        因为：1. 正则表达式过滤不全面：即使过滤掉了一些特殊字符，仍然可能存在其他注入风险。
        2. 限制了用户输入：用户可能需要输入包含特殊字符的内容（如逗号、感叹号、分号等），
        但这些内容是合法的用户输入。
        
        更安全的解决方案为了安全地处理用户输入并防止 SQL 注入，
        应该使用 参数化查询（Parameterized Queries）。参数化查询是防止 SQL 注入的最佳实践，
        它将用户输入作为参数传递给 SQL 查询，而不是直接拼接到 SQL 语句中。
        */
        private static bool IsSafeString(string str)
        {
            return !Regex.IsMatch(str, @"[-|;|,|\/|\(|\)|\[|\]|\}|\{|%|@|\*|!|\']");
        }

        public static SqlResultType IsAccountExist(string id)
        {
            if(!DbManager.IsSafeString(id)) return SqlResultType.illegal;

            string s = string.Format("select * from account where id='{0}';", id);
            try
            {
                MySqlCommand cmd = new MySqlCommand(s, mysql);
                MySqlDataReader dataReader = cmd.ExecuteReader();
                
                SqlResultType result = dataReader.HasRows ? SqlResultType.yes : SqlResultType.no;
                dataReader.Close();
                return result;
            }
            catch(Exception ex)
            {
                Console.WriteLine("[mysql] IsAccountExist 错误 " + ex.Message);
                return SqlResultType.error;
            }
        }

        public static void TestIsAccountExist()
        {
            SqlResultType result = DbManager.IsAccountExist("p3");
            Console.WriteLine("result 1 " + result);

            SqlResultType result2 = DbManager.IsAccountExist("p1");
            Console.WriteLine("result 2 " + result2);
        }


        public static SqlResultType Register(string id, string password)
        {
            if(!DbManager.IsSafeString(id) || !IsSafeString(password))
            {
                Console.WriteLine("[mysql]注册失败，非法的id或password");
                return SqlResultType.illegal;
            }

            if(IsAccountExist(id) == SqlResultType.yes)
            {
                Console.WriteLine("[mysql]注册失败，id已存在");
                return SqlResultType.no;
            }

            string sql = string.Format("insert into account set id='{0}',pw='{1}';", id,password);
            try
            {
                MySqlCommand cmd = new MySqlCommand(sql, mysql);
                cmd.ExecuteNonQuery();
                return SqlResultType.yes;
            }
            catch (Exception ex)
            {
                Console.WriteLine("[mysql]注册失败 " + ex.Message);
                return SqlResultType.error;
            }
        }
        public static void TestRegister()
        {
            SqlResultType result1 = Register("p1", "123");
            Console.WriteLine($"register result 1 {result1}");

            SqlResultType result2 = Register("p1;delete * from account;", "123");
            Console.WriteLine($"register result 2 {result2}");

            SqlResultType result3 = Register("p3", "123");
            Console.WriteLine($"register result 3 {result3}");
        }

        public static SqlResultType CreatePlayer(string id)
        {
            if (!IsSafeString(id))
            {
                Console.WriteLine("[mysql]非法ID");
                return SqlResultType.illegal;
            }

            PlayerData playerData = new PlayerData();
            string data = JsonConvert.SerializeObject(playerData);
            string sql = string.Format("insert into player set id='{0}',data='{1}';", id, data);
            try
            {
                MySqlCommand cmd = new MySqlCommand(sql, mysql);
                cmd.ExecuteNonQuery();
                return SqlResultType.yes;
            }
            catch (Exception ex)
            {
                Console.WriteLine("[mysql]注册（创建玩家数据）失败"+ ex.Message);
                return SqlResultType.error;
            }
        }


        public static void TestRegisterAndCreatePlayer()
        {
            SqlResultType result1 = CreatePlayer("p1");
            Console.WriteLine($"create player result 1 {result1}");

            SqlResultType result2 = CreatePlayer("p1;delete * from account;" );
            Console.WriteLine($"create player  result 2 {result2}");

            SqlResultType result3 = CreatePlayer("p3");
            Console.WriteLine($"create player  result 3 {result3}");
        }

        public static SqlResultType CheckPassword(string id, string password)
        {
            if(!IsSafeString(id) || !IsSafeString(password))
            {
                Console.WriteLine("[mysql]非法的id或密码");
                return SqlResultType.illegal;
            }

            string sql = $"select * from account where id='{id}' and pw='{password}';";
            try
            {
                MySqlCommand cmd = new MySqlCommand(sql, mysql);
                MySqlDataReader dataReader = cmd.ExecuteReader();

                SqlResultType result = dataReader.HasRows ? SqlResultType.yes : SqlResultType.no;
                dataReader.Close();
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[mysql]验证密码报错 {ex.Message}");
                return SqlResultType.error;
            }
        }
        public static void TestCheckPassword()
        {
            SqlResultType r1 = CheckPassword("p1", "123");
            Console.WriteLine($"check pw r1 {r1}");

            SqlResultType r2 = CheckPassword("p1", "1234");
            Console.WriteLine($"check pw r2 {r2}");

            SqlResultType r3 = CheckPassword("p1;delete from account;", "123");
            Console.WriteLine($"check pw r3 {r3}");

            SqlResultType r4 = CheckPassword("p4", "123");
            Console.WriteLine($"check pw r4 {r4}");
        }


        public static PlayerData? GetPlayerData(string id)
        {
            if (!IsSafeString(id))
            {
                Console.WriteLine("[MYSQL]非法的Id");
                return null;
            }

            string sql = $"select * from player where id='{id}';";
            try
            {
                MySqlCommand cmd = new MySqlCommand(sql, mysql);
                MySqlDataReader dataReader = cmd.ExecuteReader();
                if (dataReader.HasRows)
                {
                    dataReader.Read();
                    string dataStr = dataReader.GetString("data");

                    PlayerData? data = JsonConvert.DeserializeObject<PlayerData>(dataStr);

                    dataReader.Close();
                    return data;
                }
                else
                {
                    dataReader.Close ();
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"[mysql]GetPlayerData报错 {ex.Message}");
            }

            return null;
        }
        public static void TestGetPlayerData()
        {
            PlayerData? data1 = GetPlayerData("p1");
            if(data1 != null)
            {
                Console.WriteLine($"data1 {data1.coin} {data1.text}");
            }
            else
            {
                Console.WriteLine("data1 is null");
            }

            PlayerData? data2 = GetPlayerData("p4");
            if (data2 != null)
            {
                Console.WriteLine($"data2 {data2.coin} {data2.text}");
            }
            else
            {
                Console.WriteLine("data2 is null");
            }
        }

        public static SqlResultType UpdatePlayerData(string id, PlayerData data)
        {
            if(!IsSafeString(id)
                //|| !IsSafeString(data.text)
            )
            {
                Console.WriteLine($"[mysql]更新玩家数据 id或者data中含有非法字符");
                return SqlResultType.illegal;
            }

            string str = JsonConvert.SerializeObject(data);
            //string sql = $"update player set data='{str}' where id='{id}';";
            string sql = "update player set data=@jsonData where id=@id;";//参数化查询（Parameterized Queries） 防止 SQL 注入
            try
            {
                MySqlCommand cmd = new MySqlCommand(sql, mysql);
                //参数化查询（Parameterized Queries） 防止 SQL 注入
                cmd.Parameters.AddWithValue("@jsonData", str);
                cmd.Parameters.AddWithValue("@id", id);

                cmd.ExecuteNonQuery();
                return SqlResultType.yes;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[mysql]更新玩家数据 报错 {ex.Message}");
                return SqlResultType.error;
            }
        }

        public static void TestUpdatePlayerData()
        {
            PlayerData? data1 = GetPlayerData("p1");
            if(data1 == null )
            {
                CreatePlayer("p1");
                data1 = GetPlayerData("p1");
            }
            data1.text = "hello,player 1";
            data1.coin += 123;

            SqlResultType r1 = UpdatePlayerData("p2", data1);
            Console.WriteLine($"update player data result 1 {r1}");

            SqlResultType r2 = UpdatePlayerData("p1", data1);
            Console.WriteLine($"update player data result 2 {r2}");
        }
    }

}
