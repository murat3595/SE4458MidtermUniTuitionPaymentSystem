using TuitionApi.Models.Data;

namespace TuitionApi
{
    public class DummyDataContext
    {
        public List<UserModel> Users { get; set; }
        public List<PaymentModel> Payments { get; set; }
        public DummyDataContext()
        {
            this.Users = new List<UserModel> {
                new UserModel(1000,"admin","admin", "admin", "adminoğlu", 0, 0),
                new UserModel(1,"öğrenci1","öğrenci1", "mehmed1", "mehmedov1", 0, 100),
                new UserModel(2,"öğrenci2","öğrenci2", "mehmed2", "mehmedov2", 0, 100),
                new UserModel(3,"öğrenci3","öğrenci3", "mehmed3", "mehmedov3", 0, 100),
                new UserModel(4,"öğrenci4","öğrenci4", "mehmed4", "mehmedov4", 0, 100),
                new UserModel(5,"öğrenci5","öğrenci5", "mehmed5", "mehmedov5", 0, 100),
                new UserModel(6,"öğrenci6","öğrenci6", "mehmed6", "mehmedov6", 0, 100),
                new UserModel(7,"öğrenci7","öğrenci7", "mehmed7", "mehmedov7", 0, 100),
                new UserModel(8,"öğrenci8","öğrenci8", "mehmed8", "mehmedov8", 0, 100),
                new UserModel(9,"öğrenci9","öğrenci9", "mehmed9", "mehmedov9", 0, 100),
                new UserModel(10,"öğrenci10","öğrenci10", "mehmed10", "mehmedov10", 0, 100),
                new UserModel(11,"öğrenci11","öğrenci11", "mehmed11", "mehmedov11", 0, 100),
                new UserModel(12,"öğrenci12","öğrenci12", "mehmed12", "mehmedov12", 0, 100),
                new UserModel(13,"öğrenci13","öğrenci13", "mehmed13", "mehmedov13", 0, 100),
                new UserModel(14,"öğrenci14","öğrenci14", "mehmed14", "mehmedov14", 0, 100),
                new UserModel(15,"öğrenci15","öğrenci15", "mehmed15", "mehmedov15", 0, 100),
                new UserModel(16,"öğrenci16","öğrenci16", "mehmed16", "mehmedov16", 0, 100),
                new UserModel(17,"öğrenci17","öğrenci17", "mehmed17", "mehmedov17", 0, 100),
                new UserModel(18,"öğrenci18","öğrenci18", "mehmed18", "mehmedov18", 0, 100),
                new UserModel(19,"öğrenci19","öğrenci19", "mehmed19", "mehmedov19", 0, 100),
                new UserModel(20,"öğrenci20","öğrenci20", "mehmed20", "mehmedov20", 0, 100),
                new UserModel(21,"öğrenci21","öğrenci21", "mehmed21", "mehmedov21", 0, 100),
                new UserModel(22,"öğrenci22","öğrenci22", "mehmed22", "mehmedov22", 0, 100),
                new UserModel(23,"öğrenci23","öğrenci23", "mehmed23", "mehmedov23", 0, 100),
                new UserModel(24,"öğrenci24","öğrenci24", "mehmed24", "mehmedov24", 0, 100),
                new UserModel(25,"öğrenci25","öğrenci25", "mehmed25", "mehmedov25", 0, 100),
                new UserModel(26,"öğrenci26","öğrenci26", "mehmed26", "mehmedov26", 0, 100),
                new UserModel(27,"öğrenci27","öğrenci27", "mehmed27", "mehmedov27", 0, 100)
            };

            this.Payments = new List<PaymentModel>();
        }                             

        public void SaveChanges()
        {
            //doesn't do anything :D
        }
    }                                 
}                                     
                                      
                                      
                                      
                                      
                                      
                                      
                                      
                                      























