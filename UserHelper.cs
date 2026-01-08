using System.Net.WebSockets;
using System.Reflection.Metadata.Ecma335;
using Npgsql;

namespace MVC;

public class UserHelper
{
    public readonly string _connectionstring = "Server=cipg01;port=6432;Database=Dhruv_intern123;Username=postgres;Password=123456";

    public bool Register(t_User user)
    {
        using var conn=new NpgsqlConnection(_connectionstring);

        conn.Open();

        string qry="INSERT INTO t_b_user(Name,Email,Password,Gender,Mobile)VALUES(@nm,@em,@pwd,@gen,@mo)";

        using var cmd=new NpgsqlCommand(qry,conn);

        cmd.Parameters.AddWithValue("@nm",user.Name);
        cmd.Parameters.AddWithValue("@em",user.Email);
        cmd.Parameters.AddWithValue("@pwd",user.Password);
        cmd.Parameters.AddWithValue("@gen",user.Gender);
         cmd.Parameters.AddWithValue("@mo",user.Mobile);

        return cmd.ExecuteNonQuery()>0;
    }

    public t_User Login(string Email,string Password)
    {
        using var conn=new NpgsqlConnection(_connectionstring);


        conn.Open();

        string qry="SELECT * FROM t_b_user WHERE Email=@em AND Password=@pwd";

        using var cmd=new NpgsqlCommand(qry,conn);

        cmd.Parameters.AddWithValue("@em",Email);
        cmd.Parameters.AddWithValue("@pwd",Password);

        using var cd=cmd.ExecuteReader();

        if (cd.Read())
        {
            return new t_User{

                CustomerId=Convert.ToInt32(cd["CustomerId"]),
                Email=cd["Email"].ToString(),
                
                
            };
        }

        return null;
    }

    public bool AddBus(t_Bus bus)
    {
        
        using var conn=new NpgsqlConnection(_connectionstring);

        conn.Open();

        string qry="INSERT INTO t_bus(BusName,Source,Destination,TravelDate,TotalSeat)VALUES(@bnm,@sou,@des,@tdate,@tseat)";

        using var cmd=new NpgsqlCommand(qry,conn);

        cmd.Parameters.AddWithValue("@bnm",bus.BusName);
        cmd.Parameters.AddWithValue("@sou",bus.Source);
        cmd.Parameters.AddWithValue("@des",bus.Destination);
        cmd.Parameters.AddWithValue("@tdate",bus.TravelDate);
        cmd.Parameters.AddWithValue("@tseat",bus.TotalSeat);

        return cmd.ExecuteNonQuery()>0;
    }

    public List<t_Bus> AllBus()
    {
        List<t_Bus> list=new List<t_Bus>();

        using var conn=new NpgsqlConnection(_connectionstring);

        conn.Open();


        string qry="SELECT * FROM t_bus WHERE TO_DATE(TravelDate, 'MM-DD-YYYY') >= CURRENT_DATE";

        using var cmd=new NpgsqlCommand(qry,conn);

        using var cd=cmd.ExecuteReader();

        while (cd.Read())
        {
            list.Add(

                new t_Bus
                {
                    BusId=Convert.ToInt32(cd["BusId"]),
                    BusName=cd["BusName"].ToString(),
                    Source=cd["Source"].ToString(),
                    Destination=cd["Destination"].ToString(),
                    TravelDate=cd["TravelDate"].ToString(),
                    TotalSeat=Convert.ToInt32(cd["TotalSeat"]),
                    Price=Convert.ToInt32(cd["Price"])
                }
            );
        }
        return list;
    }

    public bool DeleteBus(int bid)
    {
        using var conn=new NpgsqlConnection(_connectionstring);

        conn.Open();

        string qry="DELETE FROM t_bus WHERE BusId=@bid";

        using var cmd=new NpgsqlCommand(qry,conn);
        cmd.Parameters.AddWithValue("@bid",bid);

        return cmd.ExecuteNonQuery()>0;
    }

    public bool UpdateBus(t_Bus bus)
    {
        using var conn=new NpgsqlConnection(_connectionstring);

        conn.Open();

        string qry="UPDATE t_bus SET BusName=@bnm,Source=@so,Destination=@de,TravelDate=@tdate,TotalSeats=@ts WHERE BusId=@bid ";

        using var cmd=new NpgsqlCommand(qry,conn);

        cmd.Parameters.AddWithValue("@bnm",bus.BusName);
        cmd.Parameters.AddWithValue("@so",bus.Source);
        cmd.Parameters.AddWithValue("@de",bus.Destination);
        cmd.Parameters.AddWithValue("@tdate",bus.TravelDate);
        cmd.Parameters.AddWithValue("@ts",bus.TotalSeat);
        cmd.Parameters.AddWithValue("@bid",bus.BusId);

        return  cmd.ExecuteNonQuery()>0;
    }

    public bool AddBook(t_Booking book)
    {
        using var conn=new NpgsqlConnection(_connectionstring);
        
        conn.Open();

        string totalSeatQ = "SELECT TotalSeat FROM t_bus WHERE BusId=@bid";
    using var cmd1 = new NpgsqlCommand(totalSeatQ, conn);
    cmd1.Parameters.AddWithValue("@bid", book.BusId);
    int totalSeats = Convert.ToInt32(cmd1.ExecuteScalar());


    string bookedSeatQ = "SELECT COALESCE(SUM(Seat),0) FROM t_b_book WHERE BusId=@bid AND Status='Booked'";
    using var cmd2 = new NpgsqlCommand(bookedSeatQ, conn);
    cmd2.Parameters.AddWithValue("@bid", book.BusId);
    int bookedSeats = Convert.ToInt32(cmd2.ExecuteScalar());


 int availableSeats = totalSeats - bookedSeats;
    if (book.Seat > availableSeats)
        return false;

        string qry="INSERT INTO t_b_book(CustomerId,BusId,BusName,Seat,Bdate,Status)VALUES(@cd,@bd,@bnm,@st,@bdate,'Booked')";

        using var cmd=new NpgsqlCommand(qry,conn);

        cmd.Parameters.AddWithValue("@cd",book.CustomerId);
        cmd.Parameters.AddWithValue("@bd",book.BusId);
        cmd.Parameters.AddWithValue("@bnm",book.BusName);
        cmd.Parameters.AddWithValue("@st",book.Seat);
        cmd.Parameters.AddWithValue("@bdate",book.Bdate);

        return cmd.ExecuteNonQuery()>0;
    }

    public List<t_Booking> AllMyBook(int CustomerId)
    {
        List<t_Booking> list=new List<t_Booking>();

        using var conn=new NpgsqlConnection(_connectionstring);

        conn.Open();

        string qry="SELECT * FROM t_b_book WHERE CustomerId=@cd";

        using var cmd=new NpgsqlCommand(qry,conn);
        cmd.Parameters.AddWithValue("@cd",CustomerId);

        using var cd=cmd.ExecuteReader();

        while (cd.Read())
        {
            list.Add(
                new t_Booking
                {
                    BookId=Convert.ToInt32(cd["BookId"]),
                    CustomerId=Convert.ToInt32(cd["CustomerId"]),
                    BusName=cd["BusName"].ToString(),
                    Bdate=cd["Bdate"].ToString(),
                    Seat=Convert.ToInt32(cd["Seat"]),
                    Status=cd["Status"].ToString()
                }
            );
        }
        return list;
    }

    public bool UpdateSt(int BookingId)
    {
        using var conn=new NpgsqlConnection(_connectionstring);

        conn.Open();

        string qry="Update t_b_book SET Status='Canceled' WHERE BookId=@bd";

        using var cmd=new NpgsqlCommand(qry,conn);

        cmd.Parameters.AddWithValue("@bd",BookingId);

        return cmd.ExecuteNonQuery()>0;
    }

    public bool AddExpense(t_Expense expense)
    {
        
        using var conn=new NpgsqlConnection(_connectionstring);

        conn.Open();

        string qry="INSERT INTO t_b_expense(BusId,Title,Price)VALUES(@bid,@tt,@pr)";

        using var cmd=new NpgsqlCommand(qry,conn);

        cmd.Parameters.AddWithValue("@bid",expense.BusId);
        cmd.Parameters.AddWithValue("@tt",expense.Title);
        cmd.Parameters.AddWithValue("@pr",expense.Price);

        return cmd.ExecuteNonQuery()>0;
    }

    public int totalExpense(int BusId)
    {
        using var conn=new NpgsqlConnection(_connectionstring);

        conn.Open();

        string qry="SELECT SUM(Price) FROM t_b_expense WHERE BusId=@bid";

        using var cmd=new NpgsqlCommand(qry,conn);
        cmd.Parameters.AddWithValue("@bid",BusId);

         return Convert.ToInt32(cmd.ExecuteScalar());
    }

    public int totalincome(int BusId)
    {
        using var conn=new NpgsqlConnection(_connectionstring);

        conn.Open();

        string qry="SELECT SUM(t_bus.Price) FROM t_b_book JOIN t_bus on t_b_book.BusId=t_b_book.BusId WHERE t_b_book.BusId=@bid";

        using var cmd=new NpgsqlCommand(qry,conn);
        cmd.Parameters.AddWithValue("@bid",BusId);

         return Convert.ToInt32(cmd.ExecuteScalar());
    }
    

    public int calculateProfit(int BusId)
    {
        int totalIncome = totalincome(BusId);
        int totalExpense1 = totalExpense(BusId);
        return totalIncome - totalExpense1;
    }

    public int viewavailableseat(int BusId)
    {
        using var conn=new NpgsqlConnection(_connectionstring);

        conn.Open();

    string bookedSeatQ = "SELECT COALESCE(SUM(Seat),0) FROM t_b_book WHERE BusId=@bid AND Status='Booked'";
    using var cmd2 = new NpgsqlCommand(bookedSeatQ, conn);
    cmd2.Parameters.AddWithValue("@bid",BusId);
    int bookedSeats = Convert.ToInt32(cmd2.ExecuteScalar());

     string totalSeatQ = "SELECT TotalSeat FROM t_bus WHERE BusId=@bid";
    using var cmd1 = new NpgsqlCommand(totalSeatQ, conn);
    cmd1.Parameters.AddWithValue("@bid", BusId);
    int totalSeats = Convert.ToInt32(cmd1.ExecuteScalar());

    int availableSeats = totalSeats - bookedSeats;

    
    return availableSeats;

    }







  
}
