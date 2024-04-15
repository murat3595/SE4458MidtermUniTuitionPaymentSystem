




public class UserModel
{


    public UserModel (int userId, string userName, string password, string name, string surname, float balance, float tuitionTotal)
    {
        this.UserId = userId;
        this.Password = password;
        this.Username = userName;
        this.Balance = balance;
        this.TuitionTotal = tuitionTotal;
        this.Name = name;
        this.Surname = surname;
    }

    public int UserId { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public float TuitionTotal { get; set; }
    public float Balance { get; set; }
}