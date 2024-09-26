public class Program
{

    public static void Main(string[] args)
    {
        int ordersCompleted = 0;
        bool statusUpdate = false;
        bool statusComplete = false;
        string messageComplete = "";
        string messageUpdate = "";
        var builder = WebApplication.CreateBuilder(args);
        var app = builder.Build();
        Repository repository = new();

        app.MapGet("/", () => 
        {
            if (statusUpdate || statusComplete) {
                return Results.Json(new Message(repository.orders, messageUpdate, messageComplete, ordersCompleted));
            }
            else 
            {
                return Results.Json(repository.orders);
            }
        });
        app.MapGet("/{id}", (int id) => repository.orders.Find(o => o.Id == id));
        app.MapGet("/filter/{param}", (string param)
            => repository.orders.FindAll(o =>
                o.Device == param
                || o.Problem == param
                || o.Description == param
                || o.Client == param
                || o.Status == param
                || o.Master == param)
        );
        app.MapPost("/", (Order order) => repository.orders.Add(order));
        app.MapPut("/{id}", (OrderDTO orderDTO, int id) 
            => 
            {
                Order order = repository.orders.Find(o => o.Id == id);
                
                if(order == null) 
                    return;

                statusUpdate = true;
                messageUpdate = "Status updated";
                if(orderDTO.status == "Complete")
                {
                    statusComplete = true;
                    messageComplete = "Status completed";
                    ordersCompleted++;
                }
                order.Status = orderDTO.status;
                order.Description = orderDTO.description;
                order.Master = orderDTO.master;
            }
        );
        app.MapPut("/comment/{id}", (int id, Comment comment) => 
            { 
                var order = repository.orders.Find(o => o.Id == id);
                if (order == null)
                    return;
                Console.WriteLine("Order: " + comment.CommentText);

                order.Comment = comment.CommentText;
            }
        );
        app.Run();
    }
}

public class Repository
{
    public List<Order> orders = [new Order(2, DateTime.Now, "Samsung", "Broken screen", "FIX PLEASE", "J", "Start", "V"), new Order(0, DateTime.Now, "Notebook", "Broken keyboard", "dsadas", "Z", "Start", "O")];
}

public class OrderDTO
{
    public string status;
    public string description;
    public string master;

    public string Status { get => status; set { status = value; } }
    public string Description { get => description; set { description = value; } }
    public string Master { get => master; set { master = value; } }
}

public class Order
{
    private int _id;
    private DateTime _date;
    private string _device;
    private string _problem;
    private string _description;
    private string _client;
    private string _status;
    private string _master;
    private string _comment;

    public int Id { get => _id; set => _id = value; }
    public DateTime Date { get => _date; set => _date = value; }
    public string Device { get => _device; set => _device = value; }
    public string Problem { get => _problem; set => _problem = value; }
    public string Description { get => _description; set => _description = value; }
    public string Client { get => _client; set => _client = value; }
    public string Status { get => _status; set => _status = value; }
    public string Master { get => _master; set => _master = value; }
    public string Comment { get => _comment; set => _comment = value; }

    public Order(int id, DateTime date, string device, string problem, string description, string client, string status, string master)
    {
        this.Id = id;
        this.Date = date;
        this.Device = device;
        this.Problem = problem;
        this.Description = description;
        this.Client = client;
        this.Status = status;
        this.Master = master;
    }
}

public record class Message(List<Order> orders, string messageUpdate, string messageComplete, int ordersCompleted);

public class Comment
{
    public string _commentText;
    public string CommentText { get => _commentText; set => _commentText = value; }
}
