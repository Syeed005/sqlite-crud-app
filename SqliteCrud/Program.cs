using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace SqliteCrud
{
    class Program
    {
        static void Main(string[] args)
        {
            SQLiteConnection sqliteConnection;
            sqliteConnection = CreateConnection();

            //SaveSampleUserData(sqliteConnection);
            //SaveSampleProductData(sqliteConnection);

            //SaveOrderLineData(sqliteConnection);
            SaveOrderData(sqliteConnection);

            sqliteConnection.Close();
        }

        private static void SaveOrderData(SQLiteConnection sqliteConnection)
        {
            Random random = new Random();
            Order order = new Order();
            
            for (int i = 1; i <= 10499; i++)
            {
                order.OrderID = i;
                order.CustomerID = random.Next(1, 1000);
                string month = random.Next(1, 13).ToString();
                string date = random.Next(1, 28).ToString();
                if (month.Count() < 2)
                {
                    month = "0" + month;
                }
                if (date.Count() < 2)
                {
                    date = "0" + date;
                }
                order.OrderDate = random.Next(2020, 2022).ToString() + "-" + month + "-" + date;
                order.TotalCost = TotalCostFromOrderLine(sqliteConnection, i);
                order.TotalTax = order.TotalCost * .15;
                InsertOrderData(sqliteConnection, order);
            }
        }

        private static void InsertOrderData(SQLiteConnection conn, Order order)
        {
            try
            {
                SQLiteCommand sqliteCommand;
                sqliteCommand = conn.CreateCommand();
                sqliteCommand.CommandText = $"INSERT INTO Orders(OrderID, OrderDate, CustomerID, TotalCost, TotalTax) VALUES ({order.OrderID},\'{order.OrderDate}\',{order.CustomerID}, {order.TotalCost}, {order.TotalTax});";
                sqliteCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static double TotalCostFromOrderLine(SQLiteConnection conn, int i)
        {
            SQLiteDataReader sqliteReader;
            SQLiteCommand sqliteCommand;
            sqliteCommand = conn.CreateCommand();
            sqliteCommand.CommandText = $"select sum(Cost) from OrderLine group by OrderID having OrderID = {i};";
            sqliteReader = sqliteCommand.ExecuteReader();
            double readPrice = 0.0;
            while (sqliteReader.Read())
            {
                readPrice = (double)sqliteReader.GetValue(0);
            }
            return readPrice;
        }

        private static void SaveOrderLineData(SQLiteConnection sqliteConnection)
        {
            Random random = new Random();
            int count = 0;
            int orderId = 1;
            while (orderId < 10500)
            {
                OrderLine orderLine = new OrderLine();
                orderLine.OrderID = orderId++;
                int orderCount = random.Next(1, 15);
                for (int i = 0; i < orderCount; i++)
                {                                       
                    orderLine.ProductID = random.Next(1, 2400);
                    orderLine.Quantity = random.Next(1, 20);
                    double cost = CostFromDB(sqliteConnection, orderLine.ProductID);
                    orderLine.Cost = orderLine.Quantity * cost;
                    InsertOrderLineData(sqliteConnection, orderLine);
                    count++;
                }
            }            
        }

        private static void InsertOrderLineData(SQLiteConnection conn, OrderLine orderLine)
        {
            try
            {
                SQLiteCommand sqliteCommand;
                sqliteCommand = conn.CreateCommand();
                sqliteCommand.CommandText = $"INSERT INTO OrderLine(OrderID, ProductID, Quantity, Cost) VALUES ({orderLine.OrderID},{orderLine.ProductID},{orderLine.Quantity}, {orderLine.Cost});";
                sqliteCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            
        }

        private static double CostFromDB(SQLiteConnection conn, int productId)
        {                       
            SQLiteDataReader sqliteReader;
            SQLiteCommand sqliteCommand;
            sqliteCommand = conn.CreateCommand();
            sqliteCommand.CommandText = $"select Price from Products where ProductID = {productId};";
            sqliteReader = sqliteCommand.ExecuteReader();
            double readPrice = 0.0;
            while (sqliteReader.Read())
            {
                readPrice = (double)sqliteReader.GetValue(0);
            }
            return readPrice;
        }

        //This function will load, create randomly and save 1000 user data
        private static void SaveSampleUserData(SQLiteConnection sqliteConnection)
        {
            string userFile = "User.xml";
            XmlDocument doc = new XmlDocument();
            doc.Load(userFile);
            XmlNodeList users = doc.DocumentElement.SelectNodes("//Record");
            foreach (XmlNode item in users)
            {
                User user = new User();
                var childNotes = item.ChildNodes;
                user.DisplayName = childNotes.Item(0).InnerText;
                user.PhoneNumber = childNotes.Item(1).InnerText;
                user.UserName = childNotes.Item(2).InnerText;
                user.Email = childNotes.Item(3).InnerText;
                user.Password = childNotes.Item(4).InnerText;
                user.Address = childNotes.Item(5).InnerText;
                InsertUserData(sqliteConnection,user);
            }
        }

        //This function will load, create randomly and save 2400 product data
        private static void SaveSampleProductData(SQLiteConnection sqliteConnection)
        {
            Random random = new Random();
            string equipmentFile = "Product.xml";
            XmlDocument doc = new XmlDocument();
            doc.Load(equipmentFile);
            XmlNodeList equipments = doc.DocumentElement.SelectNodes("//Record");
            List<Product> products = new List<Product>();
            while (products.Count < 2400)
            {
                foreach (XmlNode item in equipments)
                {
                    Product product = new Product();
                    var childNotes = item.ChildNodes;
                    product.Name = childNotes.Item(0).InnerText;
                    product.Quantity = random.Next(1, 500);
                    product.Price = double.Parse((random.NextDouble() * (400 - 1) + 1).ToString("n2"));
                    product.SellerId = random.Next(1, 1000);
                    products.Add(product);
                }
            }

            foreach (var item in products)
            {
                InsertProductData(sqliteConnection, item);
            }
        }


        static SQLiteConnection CreateConnection()
        {
            SQLiteConnection sqliteConn;
            sqliteConn = new SQLiteConnection("Data Source=store.db; Version = 3; New = False; Compress = True;");
            try
            {
                sqliteConn.Open();
            }
            catch
            {

            }
            return sqliteConn;
        }
        static void ReadData(SQLiteConnection conn)
        {
            SQLiteDataReader sqliteReader;
            SQLiteCommand sqliteCommand;
            sqliteCommand = conn.CreateCommand();
            sqliteCommand.CommandText = "SELECT * FROM Users";
            sqliteReader = sqliteCommand.ExecuteReader();
            while (sqliteReader.Read())
            {
                var readerString1 = sqliteReader.GetValue(0);
                string readerString = sqliteReader.GetString(1);
                Console.WriteLine(readerString);
            }
            conn.Close();
        }

        static void CreateTable(SQLiteConnection conn)
        {
            SQLiteCommand sqliteCommand;
            string createSQL = "CREATE TABLE SampleTable(Col1 VARCHAR(20), Col2 INT)";
            sqliteCommand = conn.CreateCommand();
            sqliteCommand.CommandText = createSQL;
            sqliteCommand.ExecuteNonQuery();
        }

        static void InsertUserData(SQLiteConnection conn, User user)
        {
            SQLiteCommand sqliteCommand;
            sqliteCommand = conn.CreateCommand();
            sqliteCommand.CommandText = $"INSERT INTO Users(UserName, Password, DisplayName, Address, PhoneNumber, Email) VALUES (\'{user.UserName}\',\'{user.Password}\',\'{user.DisplayName}\', \'{user.Address}\', \'{user.PhoneNumber}\', \'{user.Email}\');";
            sqliteCommand.ExecuteNonQuery();
            conn.Close();
        }

        static void InsertProductData(SQLiteConnection conn, Product user) {
            SQLiteCommand sqliteCommand;
            sqliteCommand = conn.CreateCommand();
            sqliteCommand.CommandText = $"INSERT INTO Products(Name, Price, Quantity, SellerID, Description) VALUES (\'{user.Name}\',\'{user.Price}\',\'{user.Quantity}\', \'{user.SellerId}\',\'\');";
            sqliteCommand.ExecuteNonQuery();
            conn.Close();
        }
    }
}
